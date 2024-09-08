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

using dotnet_etcd;

namespace Sitwjn.DataCom
{
    internal class EtcdHandler : AbstractHandler, IHandler, INoSHandler
    {
        

        private object ObjList;

        private EtcdClient Client
        {
            get
            {
                var conn = this.Conn as EtcdConn;
                return conn != null ? conn.Instance : null;
            }
        }

        private Grpc.Core.Metadata Token
        {
            get 
            {
                var conn = this.Conn as EtcdConn;
                return conn != null ? conn.Token : null;
            }
        }


        public EtcdHandler(IConn conn)
        {
            this.Conn = conn;
        }

        private void Fetch<T>(string key, long index)
        {
            var lst = base.JsonList<T>(Client.GetVal(key, this.Token));
            ObjList = new List<T>();
            (ObjList as List<T>).Add(lst[(int)index]);
        }

        private void Fetch<T>(string key, long start = 0, long end = -1)
        {
            var lst = base.JsonList<T>(Client.GetVal(key, this.Token));
            ObjList = new List<T>();
            int len = end == -1 ? lst.Count : (int)end;
            for(int i = (int)start; i < len; i++)
            {
                (ObjList as List<T>).Add(lst[i]);
            }
        }

        private int SaveOrDelete<T>(string key, T obj, string[] keys, bool isSave)
        {
            int result = 0;
            IList<int> ids = new List<int>();
            var lst = base.JsonList<T>(Client.GetVal(key, this.Token));
            if (keys != null && keys.Length > 0)
            {
                var vals = GetValues(keys, obj);
                for (int i = 0; i < lst.Count; i++)
                {
                    if (Compare(lst[i], vals))
                    {
                        if (isSave)
                        {
                            lst[i] = obj;
                        }
                        else
                        {
                            ids.Add(i);
                        }
                        result++;
                    }
                } 
                for (int i = ids.Count - 1; i >= 0; i--)
                    lst.RemoveAt(ids[i]);
            }
            if (result == 0 && isSave)
            {
                if (lst == null)
                    lst = new List<T>();
                lst.Add(obj);
                result ++;
            }
            base.Set(base.JsonStr(lst));
            this.SetHandle(key);
            return result;
        }

        public void GetHandle(string key)
        {
            this.Scalar = Client.GetVal(key, this.Token);
        }

        public void SetHandle(string key)
        {
            if (!base.Scalar.GetType().IsValueType 
                && !(base.Scalar.GetType() == typeof(string)) 
                && !(base.Scalar.GetType() == typeof(DateTime)))
                throw new InvalidCastException(base.Scalar.GetType().FullName);
            Client.Put(key, this.Scalar.ToString(), this.Token);
            this.NoQResult = 1;
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
            byte[] bytes = Encoding.UTF8.GetBytes(Client.GetVal(key, this.Token));
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
            Client.Put(key, Encoding.UTF8.GetString(ms.ToArray()), this.Token);
        }

        new public IList<T> ResultList<T>()
        {
            return ObjList as List<T>;
        }
    }
}
