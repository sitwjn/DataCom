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
    internal class DCredential
    {
        public string Cfg { get; set; }
        public string Host { get; set; }
        public string DBName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public int? Port { get; set; }

        public DCredential()
        {

        }

        public DCredential(string cfg, string host, string dbName, string username, string password, int? port)
        {
            this.Cfg = cfg;
            this.Host = host;
            this.DBName = dbName;
            this.Username = username;
            this.Password = password;
            this.Port = port;
        }

    }
}
