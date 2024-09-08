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
    /// Interface of Operator for relationship data client.
    /// Make integration with handler and connection, do core query or manipulate action.
    /// </summary>
    internal interface IRDOperator
    {
        /// <summary>
        /// Transfer sql result into Datatable
        /// </summary>
        /// <param name="sql">Sql text for search</param>
        /// <param name="parameters">Parameters which dynamic joint in sql text when searching</param>
        /// <returns>Result datatable</returns>
        DataTable GetDataTable(string sql, params Param[] parameters);

        /// <summary>
        /// Update data from datatable, must be used after GetDataTable method has been runned.
        /// The method is based on the sql in GetDataTable method which contained the effected destinate table. 
        /// </summary>
        /// <param name="dt">Datatable for update</param>
        void UpdateDataTable(DataTable dt);

        /// <summary>
        /// Get generic object by sql
        /// </summary>
        /// <typeparam name="T">Generic type of object</typeparam>
        /// <param name="sql">Sql text for search</param>
        /// <param name="parameters">Parameters which dynamic joint in sql text when searching</param>
        /// <returns>Result object, null if not found</returns>
        T GetScalar<T>(string sql, params Param[] parameters);

        /// <summary>
        /// Get generic objects list by sql
        /// </summary>
        /// <typeparam name="T">Generic type of objects list</typeparam>
        /// <param name="sql">Sql text for search</param>
        /// <param name="parameters">Parameters which dynamic joint in sql text when searching</param>
        /// <returns>Result objects list</returns>
        IList<T> GetList<T>(string sql, params Param[] parameters);

        /// <summary>
        /// DML sql execution
        /// </summary>
        /// <param name="sql">Sql test for DML</param>
        /// <param name="parameters">Parameters which dynamic joint in sql text when executing</param>
        /// <returns>Result of execution</returns>
        int ExecuteNoQuery(string sql, params Param[] parameters);

        /// <summary>
        /// DML sql transaction
        /// </summary>
        /// <param name="cmds">Sql command array for execution transaction</param>
        /// <returns>Result of execution</returns>
        int ExecuteTran(params DCommand[] cmds);
    }
}
