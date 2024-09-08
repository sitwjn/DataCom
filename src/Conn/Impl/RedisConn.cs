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

using StackExchange.Redis;

namespace Sitwjn.DataCom
{
    internal class RedisConn : AbstractConn, IConn
    {    
        private ConfigurationOptions _Options { get; set; }
        private ConnectionMultiplexer _Instance;
        public ConnectionMultiplexer Instance { get { return _Instance; } }

        public RedisConn(DCredential credential) : base(credential) { }

        public RedisConn(string connStr) : base(connStr) { }

        protected override void SetCredential(DCredential credential)
        {
            string connStr = string.Format("{0}:{1},password={2},abortConnect=false"
                , credential.Host, credential.Port, credential.Password);
            this.SetConnStr(connStr);
        }

        protected override void SetConnStr(string connStr)
        {
            this._Options = ConfigurationOptions.Parse(connStr);
        }

        public void Open()
        {
            this._Instance = ConnectionMultiplexer.Connect(this._Options);
        }

        public void Close()
        {
            this.Instance.Close();
        }
    }
}
