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
using System.Net;
using System.Text;

using dotnet_etcd;

namespace Sitwjn.DataCom
{
    internal class EtcdConn : AbstractConn, IConn
    {
        private EtcdClient _Instance;
        public EtcdClient Instance { get { return _Instance; } }

        private string Username { get; set; }

        private string Password { get; set; }

        public Grpc.Core.Metadata Token 
        {
            get 
            {
                if (this.Username != null)
                {
                    var authRes = this._Instance.Authenticate(new Etcdserverpb.AuthenticateRequest()
                    {
                        Name = this.Username,
                        Password = this.Password,
                    });
                    return new Grpc.Core.Metadata() { new Grpc.Core.Metadata.Entry("token", authRes.Token) };
                }
                return null;
            } 
        } 

        public EtcdConn(DCredential credential) : base(credential) { }

        public EtcdConn(string connStr) : base(connStr) { }

        protected override void SetCredential(DCredential credential)
        {
            string connStr = string.Format("{0}:{1}", credential.Host, credential.Port);
            this.SetConnStr(connStr);
            this.Username = credential.Username;
            this.Password = credential.Password;
        }

        protected override void SetConnStr(string connStr)
        {
            this._Instance = new EtcdClient(connStr);
        }

        public void Open()
        {
        }

        public void Close()
        {
        }
    }
}
