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

using MySql.Data.MySqlClient;

namespace Sitwjn.DataCom
{
    internal class MySqlConn : AbstractConn, IConn
    {

        private MySqlConnection _Instance;
        public MySqlConnection Instance { get { return _Instance; } }

        public MySqlConn(DCredential credential) : base(credential) { }

        public MySqlConn(string connStr) : base(connStr) { }

        protected override void SetCredential(DCredential credential)
        {
            string connStr = string.Format("server={0};port={1};database={2};user={3};password={4}"
                , credential.Host, credential.Port, credential.DBName, credential.Username, credential.Password);
            this.SetConnStr(connStr);
        }

        protected override void SetConnStr(string connStr)
        {
            this._Instance = new MySqlConnection(connStr);
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
