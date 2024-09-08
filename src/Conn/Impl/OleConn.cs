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

using System.Data.OleDb;

namespace Sitwjn.DataCom
{
    internal class OleConn : AbstractConn, IConn
    {
        private OleDbConnection _Instance;
        public OleDbConnection Instance { get { return _Instance; } }

        public OleConn(DCredential credential) : base(credential) { }

        public OleConn(string connStr) : base(connStr) { }

        protected override void SetCredential(DCredential credential)
        {
            string connStr = null;

            if (credential.Cfg.EndsWith(".accdb"))
            {
                connStr = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Persist Security Info=False;", credential.Cfg);
            }
            else if (credential.Cfg.EndsWith(".xlsx"))
            {
                connStr = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=\"Excel 12.0 Xml;HDR=YES\";", credential.Cfg);
            }
            else if (credential.Username == null)
            {
                connStr = string.Format("Server={0};Database={1};Integrated Security=SSPI;", credential.Host, credential.DBName);
            }
            else
            {
                connStr = string.Format("Server={0};Database={1};User Id={2};Password={3};Provider={4}",
                    credential.Host, credential.DBName, credential.Username, credential.Password, credential.Cfg);
            }
            this.SetConnStr(connStr);
        }

        protected override void SetConnStr(string connStr)
        {
            this._Instance = new OleDbConnection(connStr);
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
