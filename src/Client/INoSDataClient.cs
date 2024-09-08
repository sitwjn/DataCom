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
    /// Not Relationship Data Client
    /// </summary>
    public interface INoSDataClient
    {
        /// <summary>
        /// Insert Single object of generic type
        /// </summary>
        /// <typeparam name="T">Generic Type of Object for insert</typeparam>
        /// <param name="obj">Object for insert</param>
        /// <param name="destName">The destination name for object insert, Key of Redis and Etcd, Table of Cassandra</param>
        /// <returns>The count of object inserted</returns>
        int Insert<T>(T obj, string destName);

        /// <summary>
        /// Update Single object of generic type
        /// </summary>
        /// <typeparam name="T">Generic Type of Object for update</typeparam>
        /// <param name="obj">Object for update</param>
        /// <param name="destName">The destination name for object update, Key of Redis and Etcd, Table of Cassandra</param>
        /// <param name="keys">Key attributes for match the values of object in destination</param>
        /// <returns>The count of object updated</returns>
        int Update<T>(T obj, string destName, string[] keys);

        /// <summary>
        /// Delete Single object of generic type
        /// </summary>
        /// <typeparam name="T">Generic Type of Object for delete</typeparam>
        /// <param name="obj">Object for delete</param>
        /// <param name="destName">The destination name for object delete, Key of Redis and Etcd, Table of Cassandra</param>
        /// <param name="keys">Key attributes for match the values of object in destination</param>
        /// <returns>The count of object deleted</returns>
        int Delete<T>(T obj, string destName, string[] keys);

        /// <summary>
        /// Insert Objects list of generic type
        /// </summary>
        /// <typeparam name="T">Generic Type of Objects for insert</typeparam>
        /// <param name="list">Objects list for insert</param>
        /// <param name="destName">The destination name for objects insert, Key of Redis and Etcd, Table of Cassandra</param>
        /// <returns>The counts of object inserted</returns>
        int Insert<T>(IList<T> list, string destName);

        /// <summary>
        /// Update Objects list of generic type
        /// </summary>
        /// <typeparam name="T">Generic Type of Objects for update</typeparam>
        /// <param name="list">Objects list for update</param>
        /// <param name="destName">The destination name for objects update, Key of Redis and Etcd, Table of Cassandra</param>
        /// <param name="keys">Key attributes for match the values of objects in destination</param>
        /// <returns>The counts of object updated</returns>
        int Update<T>(IList<T> list, string destName, string[] keys);

        /// <summary>
        /// Delete Objects list of generic type
        /// </summary>
        /// <typeparam name="T">Generic Type of Objects for delete</typeparam>
        /// <param name="list">Objects list for delete</param>
        /// <param name="destName">The destination name for objects delete, Key of Redis and Etcd, Table of Cassandra</param>
        /// <param name="keys">Key attributes for match the values of objects in destination</param>
        /// <returns>The counts of object deleted</returns>
        int Delete<T>(IList<T> list, string destName, string[] keys);

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
        /// <param name="parameters">Condition parameters </param>
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
