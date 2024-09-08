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
    /// Before DCommand Execution
    /// </summary>
    /// <returns>Error message, return null for executing command normally, otherwise interrupt the command execution</returns>
    public delegate string BeforeDCmdHandle();

    /// <summary>
    /// Before DCommand Execution
    /// </summary>
    /// <param name="cmdResult">Result of This DCommand Execution</param>
    /// <returns>Error message, return null for executing next command normally, otherwise interrupt the next command execution</returns>
    public delegate string AfterDCmdHandle(int cmdResult);
    
    /// <summary>
    /// Data Command for Transaction DML method
    /// </summary>
    public class DCommand
    {
        /// <summary>
        /// The DML command executed by Transaction method
        /// </summary>
        public string CmdStr { get; set; }

        /// <summary>
        /// The parameters dynamic joint in CmdStr
        /// </summary>
        public Param[] Params { get; set; }
        

        /// <summary>
        /// The event handle before DML command execution for custom defined
        /// </summary>
        public BeforeDCmdHandle BeforeEvent;

        /// <summary>
        /// The event handle after DML command execution for custom defined
        /// </summary>
        public AfterDCmdHandle AfterEvent;

        /// <summary>
        /// Do BeforeEvent, Transaction method Called
        /// </summary>
        /// <param name="cmd">The cmd string transaction executed</param>
        internal void DoBeforeEvent(string cmd)
        {
            if (this.BeforeEvent != null)
            {
                string msg = this.BeforeEvent();
                if (!string.IsNullOrEmpty(msg))
                {
                    throw new ApplicationException(
                        string.Format("[Before DCommand Handle Err]: {0} [Command Text]: {1}", msg, cmd));
                }
            }
        }

        /// <summary>
        /// Do AfterEvent, Transaction method Called
        /// </summary>
        /// <param name="result">The cmd execution result in Transaction</param>
        /// <param name="cmd">The cmd string transaction executed</param>
        internal void DoAfterEvent(int result, string cmd)
        {
            if (this.AfterEvent != null)
            {
                string msg = this.AfterEvent(result);
                if (!string.IsNullOrEmpty(msg))
                {
                    throw new ApplicationException(
                        string.Format("[After DCommand Handle Err]: {0} [Command Text]: {1}", msg, cmd));
                }
            }
        }
    }
}
