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
    /// Interface of Operator for not relationship data client.
    /// Make integration with handler and connection, do core query or manipulate action.
    /// </summary>
    internal interface INoSOperator
    {
        /// <summary>
        /// Transfer data in key into Datatable
        /// </summary>
        /// <param name="key">The key name of data for transfer, Key of Redis and Etcd, Table of Cassandra</param>
        /// <returns></returns>
        DataTable GetDataTable(string key);

        /// <summary>
        /// Save data in Datatable
        /// </summary>
        /// <param name="key">The key name(destination) of Datatable to save</param>
        /// <param name="dt">Datatable for save</param>
        void UpdateDataTable(string key, DataTable dt);

        /// <summary>
        /// Save data as Json format string, or a pure string into Redis and Etcd
        /// </summary>
        /// <param name="key">The data key(destination) name</param>
        /// <param name="value">Data string</param>
        void Set(string key, string value);

        /// <summary>
        /// Get data as Json format string, or a pure string from Redis and Etcd
        /// </summary>
        /// <param name="key">The data key(source) name</param>
        /// <returns>Data string</returns>
        string Get(string key);

        /// <summary>
        /// Fetch generic data List by key(Table Name) from Table in Cassandra
        /// </summary>
        /// <typeparam name="T">Generic Type of data list</typeparam>
        /// <param name="key">Source Table Name in Cassandra</param>
        /// <param name="parameters">Condition parameters</param>
        /// <returns></returns>
        IList<T> FetchList<T>(string key, params Param[] parameters);

        /// <summary>
        /// Fetch generic object by key from a list in Redis or Etcd
        /// </summary>
        /// <typeparam name="T">Generic type of object</typeparam>
        /// <param name="key">Source key name of list in Redis or Etcd</param>
        /// <param name="index">Index of object in list</param>
        /// <returns>Generic object</returns>
        T Fetch<T>(string key, long index);

        /// <summary>
        /// Fetch Generic list from Redis or Etcd
        /// </summary>
        /// <typeparam name="T">Generic type of list</typeparam>
        /// <param name="key">Source key name of list in Redis or Etcd</param>
        /// <param name="start">Start index of list, default is 0</param>
        /// <param name="end">End index of list, default is -1 means all of elements in list</param>
        /// <returns>Generic list</returns>
        IList<T> FetchList<T>(string key, long start = 0, long end = -1);
    }
}
