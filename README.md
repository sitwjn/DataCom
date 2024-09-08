
<h1 align="center">
  DataCom
  <br>
</h1>

<h4 align="center">
A database client library written in C# support not only relationship database include Sqlserver, Odbc, Oledb, Mysql, PostgreSql and Sqlite, but also no relationship database such as redis, cassandra or etcd.
</h4>

<p align="center">
  <a href="#key-features">Key Features</a> •
  <a href="#architecture">Architecture</a> •
  <a href="#install-library-from-Nuget">Install library from Nuget</a> •
  <a href="#credits">Credits</a> •
  <a href="#authors">Authors</a> •
  <a href="#license">License</a>
</p>

## Key Features

* Factory Pattern - Encapsulation, Flexibility, Code Reusability, Cacheable, Testability
  - It is one of the most used design patterns, which provides one of the best ways to create an object without exposing the creation logic to client.
* Based on Packages on Nuget - Official, Compatibility, Reliablity
  - Official packages of each kind database client library on Nuget plantform is more stable and reliable than other kind of third part library.
* Interface Behavior - Abstraction, Polymorphic, Encapsulation
  - Client as an Interface object represents the IS-A relationship can be used easily to call different database in the same way.

## Architecture

The library are consist of three core interfaces which are IConn, IHandler and IOperator. The IConn encapsulates each kind of database connection with connecting parameters, and it play important role in the begin and end data operation progress of DataCom. The implement entities of IHandler realize the behavior methods of specific operation for each data client. The IOperator charge of the data operation progress according to interface contract from open connection to close connection, such as select insert, update or delete and so on.

### The structure of core component relationship in the class diagram as below:

```mermaid
---
title: Main Components
---
classDiagram
    class IConn
	class IHandler
	class DCredential
	class DataClientFactory
	class DCommand
	class Param
	class DataComType
	class HandleAction
	class IOperator
	class IDataClient
	class OperHandle
	<<interface>> IConn
	<<interface>> IHandler
	<<interface>> IOperator
	<<interface>> IDataClient
	<<delegate>> OperHandle
	<<enumeration>> DataComType
	DCommand o-- Param
	DataClientFactory ..|> IDataClient
	DataClientFactory ..> DataComType
	IDataClient ..> DataComType
	IOperator o-- IHandler
	IHandler o-- IConn
	IConn --> DCredential
	IOperator --> HandleAction
	HandleAction ..|> OperHandle
	HandleAction ..> IHandler
	IHandler --> Param
	IHandler --> DCommand
	IDataClient o-- IOperator
```

### The progress about data client initialize and operation as below:

```mermaid
---
title: The Progress of Data Client Creation and Data Operation
---
sequenceDiagram
	participant DataClientFactory
	note right of DataClientFactory: Create Client Begin
	DataClientFactory->>IDataClient: create data client
	activate IDataClient
	IDataClient->>IOperator: make operator according to DataComType
	activate IOperator
	IOperator->>IConn: make connection by credential or connStr
	activate IConn
	IConn-->>IOperator: return connection instance
	deactivate IConn
	IOperator->>IHandler: make handler with connection
	activate IHandler
	IHandler-->>IOperator: return handler instance
	deactivate IHandler
	IOperator->>HandeAction: initial handle action
	activate HandeAction
	HandeAction-->>IOperator: return handle action instance
	deactivate HandeAction
	IOperator-->>IDataClient: return operator instance
	deactivate IOperator
	deactivate IDataClient
	note right of DataClientFactory: Create Client End
	
	note right of DataClientFactory: A Data Operation Begin
	critical do data operation
		IDataClient->>IOperator: call data operate method
		activate IOperator
		IOperator->>HandeAction: set parameter
		activate HandeAction
		HandeAction->>IOperator: run action
		deactivate HandeAction
		IOperator->>IConn: open connection
		activate IConn
		IConn-->>IOperator: connection open
		IOperator->>HandeAction: call action method
		activate HandeAction
		HandeAction->>IHandler: call method
		activate IHandler
		deactivate HandeAction
		IHandler->>IConn: do data operation
		IConn-->>IHandler: return operation result
		IHandler-->>IOperator: return result
		deactivate IHandler
		IOperator-->>IDataClient: return result to client
		IOperator->>IConn: close connection
		IConn-->>IOperator: connection closed
		deactivate IConn
		deactivate IOperator
	option connection or operation failed
		IConn-->>IOperator: throw connection exception
		activate IConn
		activate IOperator
		IOperator-->>IDataClient: throw exception to client
		IOperator->>IConn: close connection
		IConn-->>IOperator: connection closed
		deactivate IConn
		deactivate IOperator
	end
	note right of DataClientFactory: A Data Operation End
```

### The relationship between IDataClient and DataClientFactory can be described in the calss diagram as below:

``` mermaid
---
title: The Relationship between IDataClient and DataClientFactory
---
classDiagram
	class AbstractDataClient
	<<abstract>> AbstractDataClient
	class RDDataClient
	class NoSDataClient
	class IRDDataClient {
		int Insert<T>(T obj, string destName)
        int Update<T>(T obj, string destName, string[] keys)
        int Delete<T>(T obj, string destName, string[] keys)
        int Insert<T>(IList<T> list, string destName)
        int Update<T>(IList<T> list, string destName, string[] keys)
        int Delete<T>(IList<T> list, string destName, string[] keys)
        DataTable GetDataTable(string sql, params Param[] parameters)
        void UpdateDataTable(DataTable dt)
        T GetScalar<T>(string sql, params Param[] parameters)
        IList<T> GetList<T>(string sql, params Param[] parameters)
        int ExecuteNoQuery(string sql, params Param[] parameters)
        int ExecuteTran(params DCommand[] cmds)
	}
	<<interface>> IRDDataClient
	class INoSDataClient {
		int Insert<T>(T obj, string destName)
        int Update<T>(T obj, string destName, string[] keys)
        int Insert<T>(IList<T> list, string destName)
        int Update<T>(IList<T> list, string destName, string[] keys)
        int Delete<T>(IList<T> list, string destName, string[] keys)
        DataTable GetDataTable(string key)
        void UpdateDataTable(string key, DataTable dt)
        void Set(string key, string value)
        string Get(string key)
        IList<T> FetchList<T>(string key, params Param[] parameters)
        T Fetch<T>(string key, long index)
        IList<T> FetchList<T>(string key, long start = 0, long end = -1)
	}
	<<interface>> INoSDataClient
	class DataClientFactory {
		+IRDDataClient CreateClient(string connStr, DataComType oType)
        +IRDDataClient CreateSqlClient(string host, string dbName, string username, string password)
        +IRDDataClient CreateOdbcClient(string dns, string username, string password)
        +IRDDataClient CreateOleClient(string filePath)
        +IRDDataClient CreateOleClient(string host, string dbName, string provider, string username = null, string password = null)
        +IRDDataClient CreateSqlite3Client(string dataSource, string password=null)
        +IRDDataClient CreatePgSqlClient(string host, string dbName, string username, string password, int port)
        +IRDDataClient CreateMySqlClient(string host, string dbName, string username, string password, int port)
        +INoSDataClient CreateRedisClient(string host, string password, int port)
        +INoSDataClient CreateCassandraClient(string host, string dbName, string username, string password, int port)
		+INoSDataClient CreateEtcdClient(string host, string username, string password, int port)
	}
	class DataComType { 
		SqlServer
		Odbc
		OleDb
		Sqlite3
		PgSql
		MySql
		Redis
		Cassandra
		Etcd
	}
	<<enumeration>> DataComType
	IRDDataClient <|.. RDDataClient
	INoSDataClient <|.. NoSDataClient
	RDDataClient --|> AbstractDataClient
	NoSDataClient --|> AbstractDataClient
	DataClientFactory ..> IRDDataClient
	DataComType <.. DataClientFactory 
	DataClientFactory ..> INoSDataClient
```



## Install library from Nuget

The compiled library of DataCom has been uploaded on Nuget plantform for installing online. The commands of package manager for Visual Studio as below:

``` bash
Install-Package Sitwjn.DataCom.1.0.0.nupkg
```

## Credits

This project uses the following open source packages:

- [Mysql](https://www.nuget.org/packages/MySql.Data)
- [Npgsql](https://www.nuget.org/packages/Npgsql)
- [Sqlite](https://www.nuget.org/packages/Microsoft.Data.Sqlite.Core)
- [Redis](https://www.nuget.org/packages/StackExchange.Redis)
- [Cassandra](https://www.nuget.org/packages/CassandraCSharpDriver)
- [Etcd](https://www.nuget.org/packages/dotnet-etcd)

## Authors

* **Jianai Wang** - *Initial work* - [DataCom](https://github.com/sitwjn/DataCom)

## License

This project is licensed under the Apache-2.0 - see the [LICENSE](https://github.com/sitwjn/DataCom/blob/main/LICENSE) file for details

