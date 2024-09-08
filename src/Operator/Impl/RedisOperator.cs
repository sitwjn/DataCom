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
    internal class RedisOperator : AbstractOperator, IOperator, INoSOperator
    {
        public RedisOperator(DCredential credential) 
        {
            base.Handler = new RedisHandler(new RedisConn(credential));
        }

        public RedisOperator(string connStr)
        {
            base.Handler = new RedisHandler(new RedisConn(connStr));
        }

        public T Fetch<T>(string key, long index)
        {
            Action.CmdStr = key;
            Action.Params = new Param[] { new Param("index", index) };
            base.RunNoS<T>(HandleType.Fetch);
            if (Handler.ResultList<T>() != null && Handler.ResultList<T>().Count > 0)
                return Handler.ResultList<T>().FirstOrDefault();
            else
                return default(T);
        }

        public IList<T> FetchList<T>(string key, long start = 0, long end = -1)
        {
            Action.CmdStr = key;
            Action.Params = new Param[] { new Param("start", start), new Param("end", end) };
            this.RunNoS<T>(HandleType.Fetch);
            return Handler.ResultList<T>();
        }

        [Obsolete("This method is not support in Redis Operator. Use Overload Method instead.")]
        public IList<T> FetchList<T>(string key, params Param[] parameters)
        {
            throw new NotImplementedException("This method is not support in Redis Operator"); 
        }
    }
}
