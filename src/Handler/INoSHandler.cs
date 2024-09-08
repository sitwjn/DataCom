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
    /// <summary>
    /// Handler interface for not relationship data client
    /// </summary>
    internal interface INoSHandler
    {
        /// <summary>
        /// Get data as Json format string, or a pure string from Redis and Etcd
        /// </summary>
        /// <param name="key">The data key(source) name</param>
        void GetHandle(string key);

        /// <summary>
        /// Save data as Json format string, or a pure string into Redis and Etcd
        /// </summary>
        /// <param name="key">The data key(destination) name</param>
        void SetHandle(string key);

        /// <summary>
        /// Save data as Json format string, or a pure string into Redis and Etcd
        /// </summary>
        /// <param name="val">Data string</param>
        void Set(string val);

        /// <summary>
        /// Transfer data in key into Datatable
        /// </summary>
        /// <param name="key">The key name of data for transfer, Key of Redis and Etcd, Table of Cassandra</param>
        void FillDtHandle(string key);

        /// <summary>
        /// Save data in Datatable
        /// </summary>
        /// <param name="dt">Datatable for save</param>
        void UpdateDt(DataTable dt);

        /// <summary>
        /// Save data in Datatable
        /// </summary>
        /// <param name="key">The key name(destination) of Datatable to save</param>
        void UpdateDtHandle(string key);

        /// <summary>
        /// Fetch generic data List by key(Redis and Etcd) or table name(Cassandra)
        /// </summary>
        /// <typeparam name="T">Generic Type of data list</typeparam>
        /// <param name="key">Source key name of list in Redis or Etcd, Source Table Name in Cassandra</param>
        /// <param name="parameters">
        /// Condition parameters, "index" of object in list(Redis or Etcd).
        /// "start"(Start index) of list, default is 0. "end"(End index) of list, default is -1 means all of elements in list
        /// </param>
        void FetchHandle<T>(string key, params Param[] parameters);
    }
}
