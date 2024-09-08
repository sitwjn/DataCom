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
    /// Interface of Operator for data client whether relationship or not.
    /// Make integration with handler and connection, do core query or manipulate action.
    /// </summary>
    internal interface IOperator
    {



        /// <summary>
        /// Insert Single object of generic type
        /// </summary>
        /// <typeparam name="T">Generic Type of Object for insert</typeparam>
        /// <param name="obj">Object for insert</param>
        /// <param name="destName">
        /// The destination table name for object insert when Relationship DML.
        /// The destination name for object insert, Key of Redis and Etcd, Table of Cassandra.
        /// </param>
        /// <returns>The count of object inserted</returns>
        int Insert<T>(T obj, string destName);

        /// <summary>
        /// Update Single object of generic type
        /// </summary>
        /// <typeparam name="T">Generic Type of Object for update</typeparam>
        /// <param name="obj">Object for update</param>
        /// <param name="destName">
        /// The destinate table name for object update when Relationship DML.
        /// The destination name for object update, Key of Redis and Etcd, Table of Cassandra.
        /// </param>
        /// <param name="keys">Key attributes for match the values of object in destination or table</param>
        /// <returns>The count of object updated</returns>
        int Update<T>(T obj, string destName, string[] keys);

        /// <summary>
        /// Delete Single object of generic type
        /// </summary>
        /// <typeparam name="T">Generic Type of Object for delete</typeparam>
        /// <param name="obj">Object for delete</param>
        /// <param name="destName">
        /// The destinate table name for object delete when Relationship DML.
        /// The destination name for object delete, Key of Redis and Etcd, Table of Cassandra.
        /// </param>
        /// <param name="keys">Key attributes for match the values of object in destination or table</param>
        /// <returns>The count of object deleted</returns>
        int Delete<T>(T obj, string destName, string[] keys);

        /// <summary>
        /// Insert Objects list of generic type
        /// </summary>
        /// <typeparam name="T">Generic Type of Objects for insert</typeparam>
        /// <param name="list">Objects list for insert</param>
        /// <param name="destName">
        /// The destinate table name for objects insert when Relationship DML.
        /// The destination name for objects insert, Key of Redis and Etcd, Table of Cassandra.
        /// </param>
        /// <returns>The counts of object inserted</returns>
        int Insert<T>(IList<T> list, string destName);

        /// <summary>
        /// Update Objects list of generic type
        /// </summary>
        /// <typeparam name="T">Generic Type of Objects for update</typeparam>
        /// <param name="list">Objects list for update</param>
        /// <param name="destName">
        /// The destinate table name for objects update when Relationship DML.
        /// The destination name for objects update, Key of Redis and Etcd, Table of Cassandra.
        /// </param>
        /// <param name="keys">Key attributes for match the values of objects in destination or table</param>
        /// <returns>The counts of object updated</returns>
        int Update<T>(IList<T> list, string destName, string[] keys);

        /// <summary>
        /// Delete Objects list of generic type
        /// </summary>
        /// <typeparam name="T">Generic Type of Objects for delete</typeparam>
        /// <param name="list">Objects list for delete</param>
        /// <param name="destName">
        /// The destination table name for objects delete when Relationship DML.
        /// The destination name for objects delete, Key of Redis and Etcd, Table of Cassandra.
        /// </param>
        /// <param name="keys">Key attributes for match the values of objects in destination or table</param>
        /// <returns>The counts of object deleted</returns>
        int Delete<T>(IList<T> list, string destName, string[] keys);
    }
}
