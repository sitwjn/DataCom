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
    internal class NoSDataClient : AbstractDataClient, INoSDataClient
    {
        private INoSOperator NoSOperator { get { return this.Operator as INoSOperator; } }

        internal NoSDataClient(DCredential credential, string connStr, DataComType oType) 
            : base(credential, connStr, oType) { }


        public DataTable GetDataTable(string key)
        {
            return this.NoSOperator.GetDataTable(key);
        }

        public void UpdateDataTable(string key, DataTable dt)
        {
            this.NoSOperator.UpdateDataTable(key, dt);
        }

        public void Set(string key, string value)
        {
            this.NoSOperator.Set(key, value);
        }

        public string Get(string key)
        {
            return this.NoSOperator.Get(key);
        }

        public IList<T> FetchList<T>(string key, params Param[] parameters)
        {
            return this.NoSOperator.FetchList<T>(key, parameters);
        }

        public T Fetch<T>(string key, long index)
        {
            return this.NoSOperator.Fetch<T>(key, index);
        }

        public IList<T> FetchList<T>(string key, long start = 0, long end = -1)
        {
            return this.NoSOperator.FetchList<T>(key, start, end);
        }
    }
}
