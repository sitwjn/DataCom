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

using MySql.Data.MySqlClient;

namespace Sitwjn.DataCom
{
    internal class MySqlHandler : AbstractHandler, IHandler, IRDHandler
    {
        private MySqlConnection MySqlConn
        {
            get
            {
                var conn = base.Conn as MySqlConn;
                return conn != null ? conn.Instance : null;
            }
        }
        
        private MySqlDataAdapter Adapter { get; set; }

        private MySqlCommand Cmd { get; set; }

        public MySqlHandler(IConn conn) : base(conn) { }

        private MySqlParameter[] ToParamsArray(Param[] parameters)
        {
            if (parameters != null)
            {
                MySqlParameter[] result = new MySqlParameter[parameters.Length];
                for(int i = 0; i < parameters.Length; i++)
                {
                    result[i] = new MySqlParameter(parameters[i].Name, parameters[i].Value);
                }
                return result;
            }
            return null;
        }

        private MySqlCommand CreateCommand(string sql, Param[] parameters)
        {
            MySqlCommand cmd = new MySqlCommand(sql, this.MySqlConn);
            if (parameters.Length > 0)
                cmd.Parameters.AddRange(this.ToParamsArray(parameters));
            return cmd;
        }

        #region Adapter

        public void FillDtHandle(string sql, params Param[] parameters)
        {
            MySqlCommand cmd = this.CreateCommand(sql, parameters);
            this.Adapter = new MySqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            this.Adapter.Fill(ds);
            this.Dt = ds.Tables[0];
        }

        public void UpdateDtHandle()
        {
            MySqlCommandBuilder builder = new MySqlCommandBuilder(this.Adapter);
            this.Adapter.Update(this.Dt);
            this.Dt.AcceptChanges();
        }

        #endregion

        #region Command

        public void ScalarHandle(string sql, params Param[] parameters)
        {
            this.Cmd = this.CreateCommand(sql, parameters);
            this.Scalar = this.Cmd.ExecuteScalar();
        }

        public void ReaderHandle(string sql, params Param[] parameters)
        {
            this.Cmd = this.CreateCommand(sql, parameters);
            MySqlDataReader reader = this.Cmd.ExecuteReader();
            this.Dics = new List<IDictionary<string, object>>();
            while (reader.Read())
            {
                IDictionary<string, object> dic = new Dictionary<string, object>();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    dic.Add(reader.GetName(i), reader.GetValue(i));
                }
                this.Dics.Add(dic);
            }
        }

        public void NoQueryHandle(string sql, params Param[] parameters)
        {
            this.Cmd = this.CreateCommand(sql, parameters);
            this.NoQResult = this.Cmd.ExecuteNonQuery();
        }

        public void NoQueryHandleTran(params DCommand[] cmds)
        {
            if (cmds.Length == 0) return;
            MySqlTransaction tran = this.MySqlConn.BeginTransaction();
            try
            {
                this.NoQResult = 0;
                foreach (var item in cmds)
                {
                    item.DoBeforeEvent(item.CmdStr);
                    this.Cmd = this.CreateCommand(item.CmdStr, item.Params);
                    this.Cmd.Transaction = tran;
                    int result = this.Cmd.ExecuteNonQuery();
                    item.DoAfterEvent(result, this.Cmd.CommandText);
                    this.NoQResult += result;
                }
                tran.Commit();
            }
            catch (Exception)
            {
                tran.Rollback();
                throw;
            }
        }

        #endregion

        public void InsertHandle<T>(string destName, T[] objs)
        {
            this.NoQueryHandleTran(this.GetInsertArrayCmd(objs, destName));
        }

        public void UpdateHandle<T>(string destName, T[] objs, string[] keys)
        {
            this.NoQueryHandleTran(this.GetUpdateArrayCmd(objs, destName, keys));
        }

        public void DeleteHandle<T>(string destName, T[] objs, string[] keys)
        {
            this.NoQueryHandleTran(this.GetDeleteArrayCmd(objs, destName, keys));
        }

        public void InsertHandle<T>(string destName, T obj)
        {
            this.NoQueryHandleTran(this.GetInsertCmd(obj, destName));
        }

        public void UpdateHandle<T>(string destName, T obj, string[] keys)
        {
            this.NoQueryHandleTran(this.GetUpdateCmd(obj, destName, keys));
        }

        public void DeleteHandle<T>(string destName, T obj, string[] keys)
        {
            this.NoQueryHandleTran(this.GetDeleteCmd(obj, destName, keys));
        }
    }
}
