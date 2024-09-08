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

using Npgsql;

namespace Sitwjn.DataCom
{
    internal class PgSqlConn : AbstractConn, IConn
    {
        private NpgsqlConnection _Instance;
        public NpgsqlConnection Instance { get { return _Instance; } }

        public PgSqlConn(DCredential credential) : base(credential) { }

        public PgSqlConn(string connStr) : base(connStr) { }

        protected override void SetCredential(DCredential credential)
        {
            string connStr = string.Format("Host={0};Username={1};Password={2};Database={3};Port={4};"
                , credential.Host, credential.Username, credential.Password, credential.DBName, credential.Port);
            this.SetConnStr(connStr);
        }

        protected override void SetConnStr(string connStr)
        {
            this._Instance = new NpgsqlConnection(connStr);
        }

        public void Open()
        {
            this.Instance.Open();
        }

        public void Close()
        {
            this.Instance.Close();
        }
    }
}
