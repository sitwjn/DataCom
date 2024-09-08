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
    internal interface IRDHandler
    {
        /// <summary>
        /// Transfer sql result into Datatable
        /// </summary>
        /// <param name="cmd">Sql text for search</param>
        /// <param name="parameters">Parameters which dynamic joint in sql text when searching</param>
        void FillDtHandle(string cmd, params Param[] parameters);

        /// <summary>
        /// Update data from datatable, must be used after GetDataTable method has been runned.
        /// The method is based on the sql in GetDataTable method which contained the effected destinate table. 
        /// </summary>
        void UpdateDtHandle();

        /// <summary>
        /// Update data from datatable, must be used after GetDataTable method has been runned.
        /// The method is based on the sql in GetDataTable method which contained the effected destinate table. 
        /// </summary>
        /// <param name="dt">Datatable for update</param>
        void UpdateDt(DataTable dt);

        /// <summary>
        /// Get generic object by sql
        /// </summary>
        /// <param name="cmd">Sql text for search</param>
        /// <param name="parameters">Parameters which dynamic joint in sql text when searching</param>
        void ScalarHandle(string cmd, params Param[] parameters);

        /// <summary>
        /// Search data by Sql
        /// </summary>
        /// <param name="cmd">Sql text for search</param>
        /// <param name="parameters">Parameters which dynamic joint in sql text when searching</param>
        void ReaderHandle(string cmd, params Param[] parameters);

        /// <summary>
        /// DML sql execution
        /// </summary>
        /// <param name="cmd">Sql test for DML</param>
        /// <param name="parameters">Parameters which dynamic joint in sql text when executing</param>
        void NoQueryHandle(string cmd, params Param[] parameters);

        /// <summary>
        /// DML sql transaction
        /// </summary>
        /// <param name="cmds">Sql command array for execution transaction</param>
        void NoQueryHandleTran(params DCommand[] cmds);

    }
}
