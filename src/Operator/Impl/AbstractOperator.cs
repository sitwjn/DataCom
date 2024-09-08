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

namespace Sitwjn.DataCom
{
    internal abstract class AbstractOperator
    {
        private enum OperType { RD, NoS, GenNoS, Generic, GenArray }
        
        private IConn Conn { get { return this.Handler.GetConn(); } }
        

        protected IHandler Handler { get; set; }

        protected IRDHandler RDHandler { get { return this.Handler as IRDHandler; } }

        protected INoSHandler NoSHandler { get { return this.Handler as INoSHandler; } }

        protected HandleAction Action = new HandleAction();

        public AbstractOperator() { }

        private void Run(OperType oType, HandleType hType)
        {
            try
            {
                this.Conn.Open();
                switch (oType)
                {
                    case OperType.RD:
                        Action.LaunchRD(this.RDHandler, hType);
                        break;
                    case OperType.NoS:
                        Action.LaunchNoS(this.NoSHandler, hType);
                        break;
                    default:
                        break;
                }

            }
            catch
            {
                throw;
            }
            finally
            {
                this.Conn.Close();
            }
        }

        private void Run<T>(OperType oType, HandleType hType)
        {
            try
            {
                this.Conn.Open();
                switch(oType)
                {
                    case OperType.GenNoS:
                        Action.LaunchNoS<T>(this.NoSHandler, hType);
                        break;
                    case OperType.Generic:
                        Action.Launch<T>(this.Handler, hType);
                        break;
                    case OperType.GenArray:
                        Action.LaunchArray<T>(this.Handler, hType);
                        break;
                    default:
                        break;
                }
                
            }
            catch
            {
                throw;
            }
            finally
            {
                this.Conn.Close();
            }
        }

        protected void RunRD(HandleType hType)
        {
            this.Run(OperType.RD, hType);
        }

        protected void RunNoS(HandleType hType)
        {
            this.Run(OperType.NoS, hType);
        }

        protected void RunNoS<T>(HandleType hType)
        {
            this.Run<T>(OperType.GenNoS, hType);
        }

        protected void RunGen<T>(HandleType hType)
        {
            this.Run<T>(OperType.Generic, hType);
        }

        protected void RunArray<T>(HandleType hType)
        {
            this.Run<T>(OperType.GenArray, hType);
        }



        public DataTable GetDataTable(string sql, params Param[] parameters)
        {
            Action.CmdStr = sql;
            Action.Params = parameters;
            this.RunRD(HandleType.GetDt);
            return Handler.ResultDt();
        }

        public void UpdateDataTable(DataTable dt)
        {
            RDHandler.UpdateDt(dt.GetChanges());
            this.RunRD(HandleType.UpdateDt);
        }

        public T GetScalar<T>(string sql, params Param[] parameters)
        {
            Action.CmdStr = sql;
            Action.Params = parameters;
            this.RunRD(HandleType.GetScalar);
            return Handler.ResultScalar<T>();
        }

        public IList<T> GetList<T>(string sql, params Param[] parameters)
        {
            Action.CmdStr = sql;
            Action.Params = parameters;
            this.RunRD(HandleType.GetList);
            return Handler.ResultList<T>();
        }

        public int ExecuteNoQuery(string sql, params Param[] parameters)
        {
            Action.CmdStr = sql;
            Action.Params = parameters;
            this.RunRD(HandleType.NoQuery);
            return Handler.ResultNoQ();
        }

        public int ExecuteTran(params DCommand[] cmds)
        {
            Action.Cmds = cmds;
            this.RunRD(HandleType.NoQueryTran);
            return Handler.ResultNoQ();
        }

        public int Insert<T>(T obj, string destName)
        {
            Action.Obj = obj;
            Action.DestName = destName;
            this.RunGen<T>(HandleType.Insert);
            return Handler.ResultNoQ();
        }

        public int Update<T>(T obj, string destName, string[] keys)
        {
            Action.Obj = obj;
            Action.DestName = destName;
            Action.ObjKeys = keys;
            this.RunGen<T>(HandleType.Update);
            return Handler.ResultNoQ();
        }

        public int Delete<T>(T obj, string destName, string[] keys)
        {
            Action.Obj = obj;
            Action.DestName = destName;
            Action.ObjKeys = keys;
            this.RunGen<T>(HandleType.Delete);
            return Handler.ResultNoQ();
        }

        public int Insert<T>(IList<T> list, string destName)
        {
            Action.ObjList = list;
            Action.DestName = destName;
            this.RunArray<T>(HandleType.Insert);
            return Handler.ResultNoQ();
        }

        public int Update<T>(IList<T> list, string destName, string[] keys)
        {
            Action.ObjList = list;
            Action.DestName = destName;
            Action.ObjKeys = keys;
            this.RunArray<T>(HandleType.Update);
            return Handler.ResultNoQ();
        }

        public int Delete<T>(IList<T> list, string destName, string[] keys)
        {
            Action.ObjList = list;
            Action.DestName = destName;
            Action.ObjKeys = keys;
            this.RunArray<T>(HandleType.Delete);
            return Handler.ResultNoQ();
        }

        public DataTable GetDataTable(string key)
        {
            Action.CmdStr = key;
            this.RunNoS(HandleType.GetDt);
            return Handler.ResultDt();
        }

        public void UpdateDataTable(string key, DataTable dt)
        {
            Action.CmdStr = key;
            NoSHandler.UpdateDt(dt);
            this.RunNoS(HandleType.UpdateDt);
        }

        public void Set(string key, string value)
        {
            Action.CmdStr = key;
            NoSHandler.Set(value);
            this.RunNoS(HandleType.Set);
        }

        public string Get(string key)
        {
            Action.CmdStr = key;
            this.RunNoS(HandleType.Get);
            return Handler.ResultScalar<string>();
        }

        
    }
}
