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
    /// Factory for Create Data Client
    /// </summary>
    public static class DataClientFactory
    {
        /// <summary>
        /// Create Data Client by connection string with Data Com(Client) Type
        /// </summary>
        /// <param name="connStr">Connection string of different Data Client</param>
        /// <param name="oType">Data Com(Client) Type</param>
        /// <returns>Relationship Data Client</returns>
        public static IRDDataClient CreateClient(string connStr, DataComType oType)
        {
            return new RDDataClient(null, connStr, oType);
        }
        
        /// <summary>
        /// Create Data Client for Sqlserver
        /// </summary>
        /// <param name="host">IP address or DNS name of Database server</param>
        /// <param name="dbName">Database name</param>
        /// <param name="username">Username</param>
        /// <param name="password">password</param>
        /// <returns>Relationship Data Client</returns>
        public static IRDDataClient CreateSqlClient(string host, string dbName, string username, string password)
        {
            return new RDDataClient(new DCredential(null, host, dbName, username, password, null), null, DataComType.SqlServer);
        }

        /// <summary>
        /// Create Data Client for ODBC
        /// </summary>
        /// <param name="cfg">DSN name for ODBC </param>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        /// <returns>Relationship Data Client</returns>
        public static IRDDataClient CreateOdbcClient(string dns, string username, string password)
        {
            return new RDDataClient(new DCredential(dns, null, null, username, password, null), null, DataComType.Odbc);
        }

        /// <summary>
        /// Create Data Client for OleDb
        /// </summary>
        /// <param name="filePath">File path of OleDb, such as Excel file(*.xlsx) or Access file(*.accdb)</param>
        /// <returns>Relationship Data Client</returns>
        public static IRDDataClient CreateOleClient(string filePath)
        {
            return new RDDataClient(new DCredential(filePath, null, null, null, null, null), null, DataComType.OleDb);
        }

        /// <summary>
        /// Create Data Client for OleDb
        /// </summary>
        /// <param name="host">Remote Database host Dns or IP</param>
        /// <param name="dbName">Database name</param>
        /// <param name="provider">OLE Provider name</param>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        /// <returns>Relationship Data Client</returns>
        public static IRDDataClient CreateOleClient(string host, string dbName, string provider, string username = null, string password = null)
        {
            return new RDDataClient(new DCredential(provider, host, dbName, username, password, null), null, DataComType.OleDb);
        }

        /// <summary>
        /// Create Data Client for Sqlite3
        /// </summary>
        /// <param name="dataSource">Data source of Sqlite3, such as data file path</param>
        /// <param name="password">Password</param>
        /// <returns>Relationship Data Client</returns>
        public static IRDDataClient CreateSqlite3Client(string dataSource, string password=null)
        {
            return new RDDataClient(new DCredential(dataSource, null, null, null, password, null), null, DataComType.Sqlite3);
        }

        /// <summary>
        /// Create Data Client for PgSql
        /// </summary>
        /// <param name="host">IP address or DNS name of Database server</param>
        /// <param name="dbName">Database Name</param>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        /// <param name="port">The port number of Database server</param>
        /// <returns>Relationship Data Client</returns>
        public static IRDDataClient CreatePgSqlClient(string host, string dbName, string username, string password, int port)
        {
            return new RDDataClient(new DCredential(null, host, dbName, username, password, port), null, DataComType.PgSql);
        }

        /// <summary>
        /// Create Data Client for MySql
        /// </summary>
        /// <param name="host">IP address or DNS name of Database server</param>
        /// <param name="dbName">Database Name</param>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        /// <param name="port">The port number of Database server</param>
        /// <returns>Relationship Data Client</returns>
        public static IRDDataClient CreateMySqlClient(string host, string dbName, string username, string password, int port)
        {
            return new RDDataClient(new DCredential(null, host, dbName, username, password, port), null, DataComType.MySql);
        }

        /// <summary>
        /// Create Data Client for Redis
        /// </summary>
        /// <param name="host">IP address or DNS name of Database server</param>
        /// <param name="password">Password</param>
        /// <param name="port">The port number of Database server</param>
        /// <returns>Not Relationship Data Client</returns>
        public static INoSDataClient CreateRedisClient(string host, string password, int port)
        {
            return new NoSDataClient(new DCredential(null, host, null, null, password, port), null, DataComType.Redis);
        }

        /// <summary>
        /// Create Data Client for Cassandra
        /// </summary>
        /// <param name="host">IP address or DNS name of Database server</param>
        /// <param name="dbName">Database Name</param>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        /// <param name="port">The port number of Database server</param>
        /// <returns>Not Relationship Data Client</returns>
        public static INoSDataClient CreateCassandraClient(string host, string dbName, string username, string password, int port)
        {
            return new NoSDataClient(new DCredential(null, host, dbName, username, password, port), null, DataComType.Cassandra);
        }

        /// <summary>
        /// Create Data Client for Etcd
        /// </summary>
        /// <param name="host">URL of Database server</param>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        /// <param name="port">The port number of Database server</param>
        /// <returns>Not Relationship Data Client</returns>
        public static INoSDataClient CreateEtcdClient(string host, string username, string password, int port)
        {
            return new NoSDataClient(new DCredential(null, host, null, username, password, port), null, DataComType.Etcd);
        }
    }
}
