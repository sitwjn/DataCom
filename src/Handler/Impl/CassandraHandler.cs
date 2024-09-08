/* 
   Copyright 2024 Jianai Wang

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Reflection;
using System.IO;

using Cassandra;
using Cassandra.Mapping;

namespace Sitwjn.DataCom
{
    internal class CassandraHandler : AbstractHandler, IHandler, INoSHandler
    {
        private const string KEY_NAME = "Key";

        private object ObjList;

        private ISession Session
        {
            get
            {
                var conn = this.Conn as CassandraConn;
                return conn != null ? conn.Instance : null;
            }
        }

        public CassandraHandler(IConn conn)
        {
            this.Conn = conn;
        }

        private IMapper GetMapper<T>(string destName)
        {
            MappingConfiguration cfg = new MappingConfiguration();
            cfg.Define(new Map<T>().TableName(destName));
            IMapper mapper = new Mapper(this.Session, cfg);
            return mapper;
        }

        private static IDictionary<string, string> GetVals<T>(T obj, PropertyInfo[] props)
        {
            IDictionary<string, string> vals = new Dictionary<string, string>();
            foreach (var item in props)
            {
                if (item.PropertyType.IsValueType)
                    vals.Add(item.Name, item.GetValue(obj, null).ToString());
                else
                    vals.Add(item.Name, string.Format("'{0}'", item.GetValue(obj, null).ToString()));
            }

            return vals;
        }

        private static string GetUpdateCql<T>(T obj, string[] keys)
        {
            var props = typeof(T).GetProperties();
            IDictionary<string, string> vals = GetVals(obj, props);
            string cql = string.Format("SET {0} WHERE {1}"
                , string.Join(",", vals.Where(m => !keys.Contains(m.Key)).Select(m => string.Format("{0}={1}", m.Key, m.Value)))
                , string.Join(" and ", keys.Select(m => string.Format("{0}={1}", m, vals.First(a => a.Key == m).Value))));
            return cql;
        }

        private static string GetDelCql<T>(T obj, string[] keys)
        {
            var props = typeof(T).GetProperties();
            IDictionary<string, string> vals = GetVals(obj, props);
            string cql = string.Format("WHERE {0}"
                , string.Join(" and ", keys.Select(m => string.Format("{0}={1}", m, vals.First(a => a.Key == m).Value))));
            return cql;
        }

        private void Update<T>(string destName, T obj, string[] keys)
        {
            if (keys == null || keys.Length == 0)
            {
                GetMapper<T>(destName).Update(obj);
            }
            else
            {
                GetMapper<T>(destName).Update<T>(GetUpdateCql(obj, keys));
            }
        }

        private void Delete<T>(string destName, T obj, string[] keys)
        {
            if (keys == null || keys.Length == 0)
            {
                GetMapper<T>(destName).Delete(obj);
            }
            else
            {
                GetMapper<T>(destName).Delete<T>(GetDelCql(obj, keys));
            }
        }



        public void GetHandle(string key)
        {
            RowSet set = Session.Execute(string.Format("select * from {0}", key));
            var lst = set.ToHashSet();
            var cols = set.Columns;
            if (lst != null && lst.Count() > 0)
            {
                IList<Dictionary<string, object>> list = new List<Dictionary<string, object>>();
                foreach (var r in lst)
                {
                    Dictionary<string, object> dic = new Dictionary<string, object>();
                    for (int i = 0; i < cols.Length; i++)
                    {
                        dic.Add(cols[i].Name, r[i]);
                    }
                    list.Add(dic);
                }
                this.Scalar = base.JsonStr(list);
            } else
                this.Scalar = null;
        }

        public void SetHandle(string key)
        {
            if (!(base.Scalar.GetType() == typeof(string)))
                throw new InvalidCastException(base.Scalar.GetType().FullName);
            var list = base.JsonList(this.Scalar.ToString());
            if (list != null && list.Count > 0)
            {
                this.NoQResult = 0;
                foreach (Dictionary<string, object> item in list)
                {
                    Session.Execute(string.Format("insert into {0}.{1} ({2}) values ({3})", Session.Keyspace, key
                        , string.Join(",", item.Keys)
                        , string.Join(",", item.Values.Select(m => m.GetType().IsValueType ? m.ToString() : string.Format("'{0}'", m)))));
                    this.NoQResult++;
                }
            }
        }

        public void FetchHandle<T>(string destName, params Param[] parameters)
        {
            var props = typeof(T).GetProperties();
            if (parameters.Length == 0)
            {
                this.ObjList = GetMapper<T>(destName).Fetch<T>();
            }
            else
            {
                string cql = string.Format("WHERE {0}"
                    , string.Join(" and ", parameters.Select(m => string.Format("{0}='{1}'", m.Name, m.Value))));
                this.ObjList = GetMapper<T>(destName).Fetch<T>(cql);
            }
        }

        public void InsertHandle<T>(string destName, T[] objs)
        {
            this.NoQResult = 0;
            foreach (var item in objs)
            {
                GetMapper<T>(destName).Insert<T>(item);
                this.NoQResult++;
            }
        }

        public void UpdateHandle<T>(string destName, T[] objs, string[] keys)
        {
            this.NoQResult = 0;
            foreach (var item in objs)
            {
                Update(destName, item, keys);
                this.NoQResult++;
            }
        }

        public void DeleteHandle<T>(string destName, T[] objs, string[] keys)
        {
            this.NoQResult = 0;
            foreach (var item in objs)
            {
                Delete(destName, item, keys);
                this.NoQResult++;
            }
        }

        public void InsertHandle<T>(string destName, T obj)
        {
            GetMapper<T>(destName).Insert(obj);
            this.NoQResult = 1;
        }

        public void UpdateHandle<T>(string destName, T obj, string[] keys)
        {
            Update(destName, obj, keys);
            this.NoQResult = 1;
        }

        public void DeleteHandle<T>(string destName, T obj, string[] keys)
        {
            Delete(destName, obj, keys);
            this.NoQResult = 1;
        }

        public void FillDtHandle(string key)
        {
            RowSet rs = Session.Execute(string.Format("select * from {0}.{1}", Session.Keyspace, key));

            DataTable dt = new DataTable();
            foreach(var col in rs.Columns)
            {
                if (col.Type.IsValueType || col.Type == typeof(string) || col.Type == typeof(DateTime))
                    dt.Columns.Add(col.Name, col.Type);
                else
                    throw new InvalidCastException(col.Type.Name);
            }
            foreach (var r in rs)
            {
                DataRow dr = dt.NewRow();
                foreach (var col in rs.Columns)
                {
                    dr[col.Name] = r[col.Name];
                }
                dt.Rows.Add(dr);
            }
            this.Dt = dt;
        }

        public void UpdateDtHandle(string key)
        {
            if (this.Dt == null)
                throw new NullReferenceException(typeof(DataTable).FullName);
            string[] cols = new string[this.Dt.Columns.Count];
            for(int i = 0; i < cols.Length; i++)
            {
                cols[i] = this.Dt.Columns[i].ColumnName;
            }
            
            RowSet rs = Session.Execute(string.Format("select * from {0}.{1}", Session.Keyspace, key));
            var lst = rs.ToHashSet();
            string insert = string.Format("insert into {0}.{1} ({2}) values ({3})", Session.Keyspace, key,
                 string.Join(",", cols), string.Join(",", cols.Select(m => "?")));
            string delete = string.Format("delete from {0}.{1} where {2}", Session.Keyspace, key,
                 string.Join(" and ", Dt.PrimaryKey.Select(m => string.Format("{0}=?", m))));
            this.NoQResult = 0;
            foreach (DataRow row in Dt.Rows)
            {
                object[] objs = new object[Dt.Columns.Count];

                for (int i = 0; i < Dt.Columns.Count; i++)
                {
                    objs[i] = row[Dt.Columns[i].ColumnName];
                }
                bool isInsert = false;
                foreach(var r in lst)
                {
                    foreach(var col in Dt.PrimaryKey)
                    {
                        if (r[col.ColumnName] == null && row[col.ColumnName] == null)
                            continue;
                        else if (r[col.ColumnName] == null || row[col.ColumnName] == null ||
                            string.Compare(r[col.ColumnName].ToString(), row[col.ColumnName].ToString()) != 0)
                        {
                            isInsert = true;
                            break;
                        }
                    }
                }
                if (isInsert || lst.Count == 0)
                {
                    Session.Execute(new SimpleStatement(insert, objs));
                    this.NoQResult++;
                } 
            }
            foreach(var r in lst)
            {
                bool isDelete = false;
                for (int i = 0; i < Dt.Rows.Count; i++)
                {
                    foreach (var col in rs.Columns)
                    {
                        if (r[col.Name] == null && Dt.Rows[i][col.Name] == null)
                            continue;
                        else if (r[col.Name] == null || Dt.Rows[i][col.Name] == null ||
                            string.Compare(r[col.Name].ToString(), Dt.Rows[i][col.Name].ToString()) != 0)
                        { 
                            isDelete = true; 
                            break; 
                        }
                    }
                }
                if(isDelete)
                {
                    object[] objs = new object[Dt.PrimaryKey.Length];
                    for(int i = 0; i < Dt.PrimaryKey.Length; i++)
                    {
                        objs[i] = r[Dt.PrimaryKey[i].ColumnName];
                    }
                    Session.Execute(new SimpleStatement(delete, objs));
                }
            }
        }

        new public IList<T> ResultList<T>()
        {
            return ObjList as List<T>;
        }
    }
}
