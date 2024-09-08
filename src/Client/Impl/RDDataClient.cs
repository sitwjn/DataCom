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
using System.Data.SqlClient;

namespace Sitwjn.DataCom
{
    internal class RDDataClient : AbstractDataClient, IRDDataClient
    {

        private IRDOperator RDOperator { get { return base.Operator as IRDOperator; } }

        internal RDDataClient(DCredential credential, string connStr, DataComType oType) 
            : base(credential, connStr, oType) { }

        public DataTable GetDataTable(string sql, params Param[] parameters)
        {
            return this.RDOperator.GetDataTable(sql, parameters);
        }

        public void UpdateDataTable(DataTable dt)
        {
            this.RDOperator.UpdateDataTable(dt);
        }

        public T GetScalar<T>(string sql, params Param[] parameters)
        {
            return this.RDOperator.GetScalar<T>(sql, parameters);
        }

        public IList<T> GetList<T>(string sql, params Param[] parameters)
        {
            return this.RDOperator.GetList<T>(sql, parameters);
        }

        public int ExecuteNoQuery(string sql, params Param[] parameters)
        {
            return this.RDOperator.ExecuteNoQuery(sql, parameters);
        }

        public int ExecuteTran(params DCommand[] cmds)
        {
            return this.RDOperator.ExecuteTran(cmds);
        }
    }
}
