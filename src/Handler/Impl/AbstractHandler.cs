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
using System.Web.Script.Serialization;
using System.Collections;

namespace Sitwjn.DataCom
{
    internal abstract class AbstractHandler
    {
        protected enum HandleType { Insert, Update, Delete }
        
        protected IConn Conn { set; get; }

        protected IList<IDictionary<string, object>> Dics { get; set; }
        
        protected DataTable Dt { get; set; }

        protected object Scalar { get; set; }

        protected int NoQResult { get; set; }

        protected AbstractHandler() { }

        public AbstractHandler(IConn conn)
        {
            this.Conn = conn;
        }

        private string InsertStr<T>(string tableName)
        {
            var props = typeof(T).GetProperties();
            return string.Format("insert into {0} ({1}) values ({2})"
                , tableName
                , string.Join(",", props.Select(m => m.Name))
                , string.Join(",", props.Select(m => string.Format("@{0}", m.Name)))
                );
        }

        private string UpdateStr<T>(string tableName, string[] keys)
        {
            if (keys == null)
                throw new ArgumentNullException(typeof(string[]).FullName);
            var props = typeof(T).GetProperties();
            return string.Format("update {0} set {1} where {2}", tableName
                , string.Join(",", props.Select(m => string.Format("{0}=@{1}", m.Name, m.Name)))
                , string.Join(" and ", keys.Select(m => string.Format("{0}=@{1}", m, m)))
                );
        }

        private string DeleteStr(string tablename, string[] keys)
        {
            if (keys == null)
                throw new ArgumentNullException(typeof(string[]).FullName);
            return string.Format("delete from {0} where {1}", tablename
                , string.Join(" and ", keys.Select(m => string.Format("{0}=@{1}", m, m)))
                );
        }

        private DCommand ToCommands<T>(T obj, string tableName, string[] keys, HandleType hType)
        {
            if (obj == null)
                throw new ArgumentNullException(typeof(T).FullName);
            if (tableName == null)
                throw new ArgumentNullException("tableName can not be null");
            if (typeof(T).IsValueType || obj is Enumerable)
                throw new InvalidCastException("Obj can not be value type or enumerable.");
            DCommand cmd = new DCommand();
            switch (hType)
            {
                case HandleType.Insert:
                    cmd.CmdStr = this.InsertStr<T>(tableName);
                    break;
                case HandleType.Update:
                    cmd.CmdStr = this.UpdateStr<T>(tableName, keys);
                    break;
                case HandleType.Delete:
                    cmd.CmdStr = this.DeleteStr(tableName, keys);
                    break;
                default:
                    break;
            }
            var props = typeof(T).GetProperties();
            cmd.Params = new Param[props.Count()];
            for (int i = 0; i < cmd.Params.Length; i++)
            {
                var val = props[i].GetValue(obj, null);
                cmd.Params[i] = new Param(props[i].Name, val != null ? val : string.Empty);
            }
            return cmd;
        }

        private DCommand[] ToCommands<T>(IEnumerable<T> collection, string tableName, string[] keys, HandleType hType)
        {
            if (collection == null || collection.Count() == 0)
                throw new ArgumentNullException(typeof(IEnumerable<T>).FullName);
            DCommand[] result = new DCommand[collection.Count()];
            int i = 0;
            foreach (var item in collection)
            {
                result[i++] = ToCommands(item, tableName, keys, hType);
            }
            return result;
        }


        protected DCommand GetInsertCmd<T>(T obj, string tableName)
        {
            return this.ToCommands(obj, tableName, null, HandleType.Insert);
        }

        protected DCommand GetUpdateCmd<T>(T obj, string tableName, string[] keys)
        {
            return this.ToCommands(obj, tableName, keys, HandleType.Update);
        }

        protected DCommand GetDeleteCmd<T>(T obj, string tableName, string[] keys)
        {
            return this.ToCommands(obj, tableName, keys, HandleType.Delete);
        }

        protected DCommand[] GetInsertArrayCmd<T>(IEnumerable<T> collection, string tableName)
        {
            return this.ToCommands(collection, tableName, null, HandleType.Insert);
        }

        protected DCommand[] GetUpdateArrayCmd<T>(IEnumerable<T> collection, string tableName, string[] keys)
        {
            return this.ToCommands(collection, tableName, keys, HandleType.Update);
        }

        protected DCommand[] GetDeleteArrayCmd<T>(IEnumerable<T> collection, string tableName, string[] keys)
        {
            return this.ToCommands(collection, tableName, keys, HandleType.Delete);
        }



        protected string JsonStr(object obj)
        {
            return new JavaScriptSerializer().Serialize(obj);
        }

        protected T JsonObj<T>(string str)
        {
            return new JavaScriptSerializer().Deserialize<T>(str);
        }

        protected IList<T> JsonList<T>(string str)
        {
            return new JavaScriptSerializer().Deserialize<IList<T>>(str);
        }

        protected ArrayList JsonList(string str)
        {
            return new JavaScriptSerializer().Deserialize<ArrayList>(str);
        }

        public IConn GetConn()
        {
            return this.Conn;
        }

        public void UpdateDt(DataTable dt)
        {
            this.Dt = dt;
        }

        public DataTable ResultDt()
        {
            return this.Dt;
        }

        public T ResultScalar<T>()
        {
            return (T)this.Scalar;
        }

        public int ResultNoQ()
        {
            return this.NoQResult;
        }

        public void Set(string val)
        {
            this.Scalar = val;
        }

        public IList<T> ResultList<T>()
        {
            if (this.Dics == null) return null;
            IList<T> lst = new List<T>();
            foreach (var r in this.Dics)
            {
                T _t = (T)Activator.CreateInstance(typeof(T));
                PropertyInfo[] propertys = _t.GetType().GetProperties();
                foreach (var pro in propertys)
                {
                    if (r[pro.Name] != null && pro.PropertyType == r[pro.Name].GetType())
                    {
                        pro.SetValue(_t, r[pro.Name], null);
                    }
                }
                lst.Add(_t);
            }
            return lst;
        }


        protected bool ObjCompare<T>(T obj1, T obj2)
        {
            if (obj1 == null || obj2 == null)
                throw new ArgumentNullException(typeof(T).FullName);
            if (obj1 is Enumerable || obj2 is Enumerable)
                throw new InvalidCastException("Obj can not be enumerable.");
            if (typeof(T).GetProperties().Length == 0)
            {
                return string.Compare(obj1.ToString(), obj2.ToString()) == 0;
            }
            else
            {
                var props = typeof(T).GetProperties();
                foreach (var prop in props)
                {
                    if (!ObjCompare(prop.GetValue(obj1, null), prop.GetValue(obj2, null)))
                        return false;
                }
                return true;
            }
        }



        protected bool Compare<T>(T obj, IDictionary<string, object> vals)
        {
            if (obj == null)
                throw new ArgumentNullException(typeof(T).FullName);
            if (vals == null)
                throw new ArgumentNullException(typeof(IDictionary<string, object>).FullName);
            if (vals.Count == 0)
                return false;
            var props = typeof(T).GetProperties();
            foreach (var val in vals)
            {
                var prop = props.Where(m => m.Name == val.Key).FirstOrDefault();
                if (prop == null || !ObjCompare(prop.GetValue(obj, null), val.Value))
                {
                    return false;
                }
            }
            return true;
        }

        protected IDictionary<string, object> GetValues<T>(string[] keys, T obj)
        {
            if (keys == null || keys.Length == 0)
                throw new ArgumentNullException(typeof(string[]).FullName);
            if (obj == null)
                throw new ArgumentNullException(typeof(T).FullName); ;
            if (typeof(T).IsValueType)
                throw new TypeAccessException(typeof(T).Name);
            IDictionary<string, object> result = new Dictionary<string, object>();
            var props = typeof(T).GetProperties();
            foreach (var k in keys)
            {
                var p = props.AsEnumerable().Where(m => m.Name == k).FirstOrDefault();
                if (p != null)
                {
                    result.Add(k, p.GetValue(obj, null));
                }
            }
            return result;
        }
    }
}
