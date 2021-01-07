//Copyright(c) 2021 Alexandra Kravtsova (aleksylum)

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Meditation.Db
{
	public static class SqlGenerator
	{
		public static String UpdateWhere(String table, String updateField, String conditionField)
		{
			return $"UPDATE {table} SET {updateField} = @{updateField} WHERE {conditionField} = @{conditionField}";
		}

		public static String SelectWhere(String table, String selectField1, String selectField2, String conditionField)
		{
			return $"SELECT {selectField1}, {selectField2} FROM {table} WHERE {conditionField} = @{conditionField}";
		}

		public static String Select(String table, String selectField)
		{
			return $"SELECT {selectField} FROM {table}";
		}

		public static String Insert(String table, params String[] parameters)
		{
			List<String> paramNames = parameters.Select(p => '@' + p).ToList();
			return $"INSERT INTO {table} VALUES({String.Join(", ", paramNames)})";
		}

		public static String ClearAll(String table)
		{
			return $"DELETE FROM {table}";
		}

		public static SqlParameter CreateParameter<T>(String name, SqlDbType type, T value)
		{
			SqlParameter param = new SqlParameter('@' + name, type);
			param.Value = value;
			return param;
		}
	}
}
