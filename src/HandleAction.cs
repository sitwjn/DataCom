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

namespace Sitwjn.DataCom
{
    /// <summary>
    /// Type of Database support in the library
    /// </summary>
    public enum DataComType { SqlServer, Odbc, OleDb, Sqlite3, PgSql, MySql, Redis, Cassandra, Etcd }
    
    internal enum HandleType { GetDt, UpdateDt, GetScalar, GetList, NoQuery, NoQueryTran, Insert, Update, Delete, Get, Set, Fetch }
    
    internal delegate void OperHandle();

    internal delegate void OperCmdHandle(string cmd, params Param[] parameters);

    internal delegate void OperTranHandle(params DCommand[] cmds);

    internal delegate void OperSingleHandle<T>(string destName, T obj);

    internal delegate void OperSingleKeysHandle<T>(string destName, T obj, string[] keys);

    internal delegate void OperArrayHandle<T>(string destName, T[] objs);

    internal delegate void OperArrayKeysHandle<T>(string destName, T[] objs, string[] keys);

    internal delegate void OperKeyHandle(string key);

    internal delegate void OperFetchHandle<T>(string key, params Param[] parameters);


    internal class HandleAction
    {
        public string CmdStr { get; set; }

        public Param[] Params { get; set; }

        public DCommand[] Cmds { get; set; }

        public object ObjList { get; set; }

        public object Obj { get; set; }

        public string[] ObjKeys { get; set; }

        public string DestName { get; set; }


        public void LaunchRD(IRDHandler handler, HandleType hType)
        {
            switch (hType)
            {
                case HandleType.GetDt:
                    this.Do(handler.FillDtHandle);
                    break;
                case HandleType.UpdateDt:
                    this.Do(handler.UpdateDtHandle);
                    break;
                case HandleType.GetScalar:
                    this.Do(handler.ScalarHandle);
                    break;
                case HandleType.GetList:
                    this.Do(handler.ReaderHandle);
                    break;
                case HandleType.NoQuery:
                    this.Do(handler.NoQueryHandle);
                    break;
                case HandleType.NoQueryTran:
                    this.Do(handler.NoQueryHandleTran);
                    break;
                default:
                    break;
            }
        }

        public void LaunchNoS(INoSHandler handler, HandleType hType)
        {
            switch (hType)
            {
                case HandleType.GetDt:
                    this.Do(handler.FillDtHandle);
                    break;
                case HandleType.UpdateDt:
                    this.Do(handler.UpdateDtHandle);
                    break;
                case HandleType.Get:
                    this.Do(handler.GetHandle);
                    break;
                case HandleType.Set:
                    this.Do(handler.SetHandle);
                    break;
                default:
                    break;
            }
        }

        public void LaunchNoS<T>(INoSHandler handler, HandleType hType)
        {
            switch (hType)
            {
                case HandleType.Fetch:
                    this.DoFetch<T>(handler.FetchHandle<T>);
                    break;
                default:
                    break;
            }
        }

        public void Launch<T>(IHandler handler, HandleType hType)
        {
            switch (hType)
            {
                case HandleType.Insert:
                    this.DoSingle<T>(handler.InsertHandle);
                    break;
                case HandleType.Update:
                    this.DoSingleKeys<T>(handler.UpdateHandle);
                    break;
                case HandleType.Delete:
                    this.DoSingleKeys<T>(handler.DeleteHandle);
                    break;
                default:
                    break;
            }
        }

        public void LaunchArray<T>(IHandler handler, HandleType hType)
        {
            switch (hType)
            {
                case HandleType.Insert:
                    this.DoArray<T>(handler.InsertHandle);
                    break;
                case HandleType.Update:
                    this.DoArrayKeys<T>(handler.UpdateHandle);
                    break;
                case HandleType.Delete:
                    this.DoArrayKeys<T>(handler.DeleteHandle);
                    break;
                default:
                    break;
            }
        }
        
        public void Do(OperHandle handler)
        {
            handler();
        }

        public void Do(OperCmdHandle handler)
        {
            handler(this.CmdStr, this.Params);
        }

        public void Do(OperTranHandle handler)
        {
            handler(this.Cmds);
        }

        public void Do(OperKeyHandle handler)
        {
            handler(this.CmdStr);
        }

        public void DoFetch<T>(OperFetchHandle<T> handler)
        {
            handler(this.CmdStr, this.Params);
        }

        public void DoSingle<T>(OperSingleHandle<T> handler)
        {
            handler(this.DestName, (T)this.Obj);
        }

        public void DoSingleKeys<T>(OperSingleKeysHandle<T> handler)
        {
            handler(this.DestName, (T)this.Obj, this.ObjKeys);
        }

        public void DoArray<T>(OperArrayHandle<T> handler)
        {
            handler(this.DestName, (this.ObjList as IList<T>).ToArray());
        }

        public void DoArrayKeys<T>(OperArrayKeysHandle<T> handler)
        {
            handler(this.DestName, (this.ObjList as IList<T>).ToArray(), this.ObjKeys);
        }

    }
}
