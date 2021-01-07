//Copyright(c) 2021 Alexandra Kravtsova (aleksylum)

using System;
using System.Collections.Generic;
using Meditation.Data;
using System.Data.SqlClient;
using System.Data;

namespace Meditation.Db
{
	public class PatchDbScope : IDisposable
	{
		public static readonly String TableName = "[Meditation].[dbo].[Patches]";

		private const String IdField = "id";
		private const String PidField = "pid";
		private const String ProcessNameField = "process_name";
		private const String MethodInfoField = "method_info";
		private const String StatusField = "status";
		private const String ValueField = "value";

		private readonly DbExecutor _dbExecutor;

		public PatchDbScope()
		{
			_dbExecutor = DbExecutor.Create();
		}

		public void CreatePatch(CPatch patch, Int32 pid, String processName, EPatchStatus status, Byte[] methodHash)
		{
			SqlParameter idParameter = SqlGenerator.CreateParameter(IdField, SqlDbType.UniqueIdentifier, patch.Id);
			SqlParameter pidParameter = SqlGenerator.CreateParameter(PidField, SqlDbType.Int, pid);
			SqlParameter processNameParameter = SqlGenerator.CreateParameter(ProcessNameField, SqlDbType.NVarChar, processName);
			SqlParameter methodInfoParameter = SqlGenerator.CreateParameter(MethodInfoField, SqlDbType.Binary, methodHash);
			SqlParameter statusParameter = SqlGenerator.CreateParameter(StatusField, SqlDbType.Int, status);
			SqlParameter valueParameter = SqlGenerator.CreateParameter(ValueField, SqlDbType.NVarChar, Serializer.Serialize<CPatch>(patch));

			String insert = SqlGenerator.Insert(TableName, IdField, PidField, ProcessNameField, MethodInfoField, StatusField, ValueField);
			_dbExecutor.ExecuteNonQuery(insert, idParameter, pidParameter, processNameParameter, methodInfoParameter, statusParameter, valueParameter);
		}

		public void ClearPatches()
		{
			String clearAll = SqlGenerator.ClearAll(TableName);
			_dbExecutor.ExecuteNonQuery(clearAll);
		}

		public void UpdatePatchStatus(Guid id, EPatchStatus status)
		{
			String updateWhere = SqlGenerator.UpdateWhere(TableName, StatusField, IdField);

			SqlParameter idParam = SqlGenerator.CreateParameter(IdField, SqlDbType.UniqueIdentifier, id);

			SqlParameter statusParam = SqlGenerator.CreateParameter(StatusField, SqlDbType.Int, status);

			_dbExecutor.ExecuteNonQuery(updateWhere, statusParam, idParam);
		}

		public List<KeyValuePair<CPatch, EPatchStatus>> GetAllPatchesByMethodHash(Byte[] methodHash)
		{
			SqlParameter statusParam = SqlGenerator.CreateParameter(MethodInfoField, SqlDbType.Binary, methodHash);

			String s = SqlGenerator.SelectWhere(TableName, ValueField, StatusField, MethodInfoField);
			var reader = _dbExecutor.Read(s, statusParam);

			List<KeyValuePair<CPatch, EPatchStatus>> patches = new List<KeyValuePair<CPatch, EPatchStatus>>();

			while (reader.Read())
				patches.Add(new KeyValuePair<CPatch, EPatchStatus>(Serializer.Deserialize<CPatch>((String)reader[0]), (EPatchStatus)reader[1]));

			return patches;
		}

		public void Dispose()
		{
			_dbExecutor?.Dispose();
		}
	}
}
