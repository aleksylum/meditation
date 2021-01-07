//Copyright(c) 2021 Alexandra Kravtsova (aleksylum)

using Meditation.Data;
using System;
using System.Data.Common;
using System.Data.SqlClient;

namespace Meditation.Db
{
	internal class DbExecutor : IDisposable
	{
		private readonly SqlConnection _connection;

		private DbExecutor(SqlConnection connection)
		{
			_connection = connection;
			_connection.Open();
		}

		public static DbExecutor Create()
		{
			(string Server, string Db) args = RegistryReader.GetDbConnectionArgs();
			DbConnectionStringBuilder dbConnectionStringBuilder = new DbConnectionStringBuilder
			{
				["Server"] = args.Server,
				["Initial Catalog"] = args.Db,
				["Integrated Security"] = true
			};
			return new DbExecutor(new SqlConnection(dbConnectionStringBuilder.ConnectionString));
		}

		public SqlDataReader Read(String command, params SqlParameter[] sqlParameters)
		{
			SqlCommand sqlCommand = new SqlCommand(command, _connection);
			sqlCommand.Parameters.AddRange(sqlParameters);
			return sqlCommand.ExecuteReader();
		}

		public void ExecuteNonQuery(String command, params SqlParameter[] sqlParameters)
		{
			SqlCommand sqlCommand = new SqlCommand(command, _connection);
			sqlCommand.Parameters.AddRange(sqlParameters);
			sqlCommand.ExecuteNonQuery();
		}

		public void Dispose()
		{
			_connection?.Dispose();
		}
	}
}
