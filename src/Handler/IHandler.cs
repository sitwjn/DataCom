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
    /// Handler interface for data client whether relationship or not
    /// </summary>
    internal interface IHandler
    {
        /// <summary>
        /// The Connection object of Handler
        /// </summary>
        /// <returns>Conncetion object</returns>
        IConn GetConn();

        /// <summary>
        /// Insert Single object of generic type
        /// </summary>
        /// <typeparam name="T">Generic Type of Object for insert</typeparam>
        /// <param name="destName">
        /// The destination table name for object insert when Relationship DML.
        /// The destination name for object insert, Key of Redis and Etcd, Table of Cassandra.
        /// </param>
        /// <param name="obj">Object for insert</param>
        void InsertHandle<T>(string destName, T obj);

        /// <summary>
        /// Update Single object of generic type
        /// </summary>
        /// <typeparam name="T">Generic Type of Object for insert</typeparam>
        /// <param name="destName">
        /// The destinate table name for object update when Relationship DML.
        /// The destination name for object update, Key of Redis and Etcd, Table of Cassandra.
        /// </param>
        /// <param name="obj">Object for update</param>
        /// <param name="keys">Key attributes for match the values of object in destination or table</param>
        void UpdateHandle<T>(string destName, T obj, string[] keys);

        /// <summary>
        /// Delete Single object of generic type
        /// </summary>
        /// <typeparam name="T">Generic Type of Object for delete</typeparam>
        /// <param name="destName">
        /// The destinate table name for object delete when Relationship DML.
        /// The destination name for object delete, Key of Redis and Etcd, Table of Cassandra.
        /// </param>
        /// <param name="obj">Object for delete</param>
        /// <param name="keys">Key attributes for match the values of object in destination or table</param>
        void DeleteHandle<T>(string destName, T obj, string[] keys);

        /// <summary>
        /// Insert Objects array of generic type
        /// </summary>
        /// <typeparam name="T">Generic Type of Objects for insert</typeparam>
        /// <param name="destName">
        /// The destinate table name for objects insert when Relationship DML.
        /// The destination name for objects insert, Key of Redis and Etcd, Table of Cassandra.
        /// </param>
        /// <param name="objs">Objects array for insert</param>
        void InsertHandle<T>(string destName, T[] objs);

        /// <summary>
        /// Update Objects array of generic type
        /// </summary>
        /// <typeparam name="T">Generic Type of Objects for update</typeparam>
        /// <param name="destName">
        /// The destinate table name for objects update when Relationship DML.
        /// The destination name for objects update, Key of Redis and Etcd, Table of Cassandra.
        /// </param>
        /// <param name="objs">Objects array for update</param>
        /// <param name="keys">Key attributes for match the values of objects in destination or table</param>
        void UpdateHandle<T>(string destName, T[] objs, string[] keys);

        /// <summary>
        /// Delete Objects list of generic type
        /// </summary>
        /// <typeparam name="T">Generic Type of Objects for delete</typeparam>
        /// <param name="destName">
        /// The destination table name for objects delete when Relationship DML.
        /// The destination name for objects delete, Key of Redis and Etcd, Table of Cassandra.
        /// </param>
        /// <param name="objs">Objects array for delete</param>
        /// <param name="keys">Key attributes for match the values of objects in destination or table</param>
        void DeleteHandle<T>(string destName, T[] objs, string[] keys);

        /// <summary>
        /// Datatable result for handle process in operator
        /// </summary>
        /// <returns>Datatable result</returns>
        DataTable ResultDt();

        /// <summary>
        /// Generic result for handle process in operator
        /// </summary>
        /// <typeparam name="T">Generic type of result</typeparam>
        /// <returns>Generic result</returns>
        T ResultScalar<T>();

        /// <summary>
        /// DML result for handle process in operator
        /// </summary>
        /// <returns>DML result</returns>
        int ResultNoQ();

        /// <summary>
        /// Generic list result for handle process in operator
        /// </summary>
        /// <typeparam name="T">Generic type of result</typeparam>
        /// <returns>Generic list result</returns>
        IList<T> ResultList<T>();
    }
}
