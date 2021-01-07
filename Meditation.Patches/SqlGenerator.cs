//Copyright(c) 2021 Alexandra Kravtsova (aleksylum)

using System;
using System.Data;
using System.Data.SqlClient;

namespace Meditation.Patches
{
	public static class SqlGenerator
	{
		public static String UpdateWhere(String table, String updateField, String conditionField)
		{
			return $"UPDATE {table} SET {updateField} = @{updateField} WHERE {conditionField} = @{conditionField}";
		}

		public static String SelectWhere(String table, String selectField, String conditionField)
		{
			return $"SELECT {selectField} FROM {table} WHERE {conditionField} = @{conditionField}";
		}

		public static String Insert(String table, params String[] values)
		{
			return $"INSERT INTO {table} VALUES('{String.Join("', '", values)}')";
		}

		public static SqlParameter CreateParameter<T>(String name, SqlDbType type, T value)
		{
			SqlParameter param = new SqlParameter('@' + name, type);
			param.Value = value;
			return param;
		}
	}
}
