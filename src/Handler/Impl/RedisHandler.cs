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

using StackExchange.Redis;

namespace Sitwjn.DataCom
{
    internal class RedisHandler : AbstractHandler, IHandler, INoSHandler
    {
        

        private object ObjList;

        private IDatabase RedisDb
        {
            get
            {
                var conn = this.Conn as RedisConn;
                return conn != null ? conn.Instance.GetDatabase() : null;
            }
        }

        public RedisHandler(IConn conn)
        {
            this.Conn = conn;
        }

        private void Fetch<T>(string key, long index)
        {
            var val = RedisDb.ListGetByIndex(key, index);
            ObjList = new List<T>();
            (ObjList as List<T>).Add(base.JsonObj<T>(val.ToString()));
        }

        private void Fetch<T>(string key, long start = 0, long end = -1)
        {
            var vals = RedisDb.ListRange(key, start, end);
            ObjList = new List<T>();
            foreach (var v in vals)
            {
                (ObjList as List<T>).Add(base.JsonObj<T>(v.ToString()));
            }
        }

        private int SaveOrDelete<T>(string key, T obj, string[] keys, bool isSave)
        {
            int result = 0;

            if (keys != null && keys.Length > 0)
            {
                for (int i = 0; i < RedisDb.ListLength(key); i++)
                {
                    string str = RedisDb.ListGetByIndex(key, i).ToString();
                    T jsonObj = this.JsonObj<T>(str);
                    var vals = GetValues(keys, obj);
                    //    ////    tran.AddCondition(Condition.ListIndexNotEqual(key, i, str));
                    //    ////    if (tran.Execute())
                    ////    var tran = RedisDb.CreateTransaction();
                    ////    tran.AddCondition(Condition.ListIndexEqual(key, i, str));
                    ////    if (tran.Execute())


                    if (Compare(jsonObj, vals))
                    {
                        if (isSave)
                        {
                            RedisDb.ListSetByIndex(key, i, base.JsonStr(obj));
                            result++;
                        }
                        else
                        {
                            RedisDb.ListRemove(key, str);
                            result++;
                        }
                    }
                } 
            }
            if (result == 0 && isSave)
            {
                RedisDb.ListRightPush(key, base.JsonStr(obj));
                result ++;
            }
            return result;
        }

        public void GetHandle(string key)
        {
            var val = RedisDb.StringGet(key);
            this.Scalar = val.HasValue ? val.ToString() : null;
        }

        public void SetHandle(string key)
        {
            if (!base.Scalar.GetType().IsValueType 
                && !(base.Scalar.GetType() == typeof(string)) 
                && !(base.Scalar.GetType() == typeof(DateTime)))
                throw new InvalidCastException(base.Scalar.GetType().FullName);
            this.NoQResult = RedisDb.StringSet(key, this.Scalar.ToString()) ? 1 : 0;
        }

        public void FetchHandle<T>(string key, params Param[] parameters)
        {
            var p = parameters.FirstOrDefault(m => m.Name == "index");
            if (p != null)
            {
                this.Fetch<T>(key, (long)p.Value);
                return;
            }
            var s = parameters.First(m => m.Name == "start");
            var e = parameters.First(m => m.Name == "end");
            this.Fetch<T>(key, s != null ? (long)s.Value : 0, e != null ? (long)e.Value : -1);
        }

        public void InsertHandle<T>(string key, T[] objs)
        {
            this.NoQResult = 0;
            foreach (var item in objs)
            {
                this.NoQResult += this.SaveOrDelete(key, item, new string[0], true);
            }
        }

        public void UpdateHandle<T>(string key, T[] objs, string[] keys)
        {
            this.NoQResult = 0;
            foreach (var item in objs)
            {
                this.NoQResult += this.SaveOrDelete(key, item, keys, true);
            }
        }

        public void DeleteHandle<T>(string key, T[] objs, string[] keys)
        {
            this.NoQResult = 0;
            foreach (var item in objs)
            {
                this.NoQResult += this.SaveOrDelete(key, item, keys, false);
            }
        }

        public void InsertHandle<T>(string key, T obj)
        {
            this.NoQResult = this.SaveOrDelete(key, obj, new string[0], true);
        }

        public void UpdateHandle<T>(string key, T obj, string[] keys)
        {
            this.NoQResult = this.SaveOrDelete(key, obj, keys, true);
        }

        public void DeleteHandle<T>(string key, T obj, string[] keys)
        {
            this.NoQResult = this.SaveOrDelete(key, obj, keys, false);
        }


        public void FillDtHandle(string key)
        {
            byte[] bytes = RedisDb.StringGet(key);
            if (bytes != null)
            {
                DataSet ds = new DataSet();
                ds.ReadXml(new MemoryStream(bytes));
                this.Dt = ds.Tables[0];
            }
            else
            {
                this.Dt = null;
            }
        }

        public void UpdateDtHandle(string key)
        {
            if (this.Dt == null)
                throw new NullReferenceException(typeof(DataTable).FullName);
            DataSet ds = new DataSet();
            ds.Tables.Add(this.Dt);
            var ms = new MemoryStream();
            ds.WriteXml(ms);
            RedisDb.StringSet(key, ms.ToArray());
        }

        new public IList<T> ResultList<T>()
        {
            return ObjList as List<T>;
        }
    }
}
