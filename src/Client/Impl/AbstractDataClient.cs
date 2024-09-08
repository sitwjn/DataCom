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
    internal abstract class AbstractDataClient
    {
        protected IOperator Operator { get; set; }

        internal AbstractDataClient() { }

        internal AbstractDataClient(DCredential credential, string connStr, DataComType oType)
        {
            switch (oType)
            {
                case DataComType.SqlServer:
                    this.Operator = credential != null ? new SqlOperator(credential) : new SqlOperator(connStr);
                    break;
                case DataComType.Odbc:
                    this.Operator = credential != null ? new OdbcOperator(credential) : new OdbcOperator(connStr);
                    break;
                case DataComType.OleDb:
                    this.Operator = credential != null ? new OleOperator(credential) : new OleOperator(connStr);
                    break;
                case DataComType.Sqlite3:
                    this.Operator = credential != null ? new Sqlite3Operator(credential) : new Sqlite3Operator(connStr);
                    break;
                case DataComType.PgSql:
                    this.Operator = credential != null ? new PgSqlOperator(credential) : new PgSqlOperator(connStr);
                    break;
                case DataComType.MySql:
                    this.Operator = credential != null ? new MySqlOperator(credential) : new MySqlOperator(connStr);
                    break;
                case DataComType.Redis:
                    this.Operator = credential != null ? new RedisOperator(credential) : new RedisOperator(connStr);
                    break;
                case DataComType.Etcd:
                    this.Operator = credential != null ? new EtcdOperator(credential) : new EtcdOperator(connStr);
                    break;
                case DataComType.Cassandra:
                    if (credential != null)
                        this.Operator = new CassandraOperator(credential);
                    else
                        throw new ArgumentException("Create cassandra data client by connection string is invalid: {0}", connStr);
                    break;
                default:
                    break;
            }
        }

        public int Insert<T>(T obj, string destName)
        {
            return this.Operator.Insert(obj, destName);
        }

        public int Update<T>(T obj, string destName, string[] keys)
        {
            return this.Operator.Update(obj, destName, keys);
        }

        public int Delete<T>(T obj, string destName, string[] keys)
        {
            return this.Operator.Delete(obj, destName, keys);
        }

        public int Insert<T>(IList<T> list, string destName)
        {
            return this.Operator.Insert(list, destName);
        }

        public int Update<T>(IList<T> list, string destName, string[] keys)
        {
            return this.Operator.Update(list, destName, keys);
        }

        public int Delete<T>(IList<T> list, string destName, string[] keys)
        {
            return this.Operator.Delete(list, destName, keys);
        }
    }
}
