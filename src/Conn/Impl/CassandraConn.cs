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

using Cassandra;

namespace Sitwjn.DataCom
{
    internal class CassandraConn : AbstractConn, IConn
    {
        private string ConnStr { get; set; } 

        private Cluster _Cluster { get; set; }

        private ISession _Session { get; set; }

        public ISession Instance { get { return _Session; } }

        public CassandraConn(DCredential credential) : base(credential) { }

        protected override void SetConnStr(string connStr)
        {
            this.ConnStr = connStr;
        }

        protected override void SetCredential(DCredential credential)
        {
            this._Cluster = Cluster.Builder().AddContactPoint(credential.Host)
                .WithPort((int)credential.Port).WithCredentials(credential.Username, credential.Password).Build();
            this.SetConnStr(credential.DBName);
        }

        public void Close()
        {
            this._Session.Dispose();
            this._Cluster.Dispose();
        }

        public void Open()
        {
            this._Session = this._Cluster.Connect(this.ConnStr);
        }

        
    }
}
