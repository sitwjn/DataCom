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
    internal class CassandraOperator : AbstractOperator, IOperator, INoSOperator
    {
        public CassandraOperator(DCredential credential) 
        {
            base.Handler = new CassandraHandler(new CassandraConn(credential));
        }


        public IList<T> Fetch<T>(string key, params Param[] parameters)
        {
            Action.CmdStr = key;
            Action.Params = parameters;
            base.RunNoS<T>(HandleType.Fetch);
            return Handler.ResultList<T>(); 
        }

        private IList<T> FetchListByParam<T>(string key, params Param[] parameters)
        {
            Action.CmdStr = key;
            Action.Params = parameters;
            this.RunNoS<T>(HandleType.Fetch);
            return Handler.ResultList<T>();
        }

        public IList<T> FetchList<T>(string key, params Param[] parameters)
        {
            return this.FetchListByParam<T>(key, parameters);
        }

        [Obsolete("This method is not support in Cassandra Operator. Use Overload Method instead.")]
        public T Fetch<T>(string key, long index)
        {
            throw new NotImplementedException("This method is not support in Cassandra Operator");
        }

        [Obsolete("This method is not support in Cassandra Operator. Use Overload Method instead.")]
        public IList<T> FetchList<T>(string key, long start = 0, long end = -1)
        {
            if (start == 0 && end == -1)
                return this.FetchListByParam<T>(key);
            else
                throw new NotImplementedException("This method is not support in Cassandra Operator");
        }
    }
}
