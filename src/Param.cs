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
    /// <summary>
    /// Parameters which dynamic joint in CmdStr of DComamand
    /// </summary>
    public class Param
    {
        
        /// <summary>
        /// Name of Parameter, which appears in CmdStr of DCommand
        /// </summary>
        internal string Name { get; set; }

        /// <summary>
        /// Value of Parameter, which dynamic replace Param Name in CmdStr of DCommand in execution progress
        /// </summary>
        internal object Value { get; set; }


        /// <summary>
        /// Constructor of Param
        /// </summary>
        /// <param name="name">Name of Parameter, which appears in CmdStr of DCommand</param>
        /// <param name="value">Value of Parameter, which dynamic replace Param Name in CmdStr of DCommand in execution progress</param>
        public Param(string name, object value)
        {
            this.Name = name;
            this.Value = value;
        }
    }
}
