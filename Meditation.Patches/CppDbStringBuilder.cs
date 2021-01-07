//Copyright(c) 2021 Alexandra Kravtsova (aleksylum)

using Meditation.Data;
using System;
using System.Diagnostics;

namespace Meditation.Patches
{
	internal static class CppDbStringBuilder
	{
		private static String DatabaseName = "Meditation";
		private static String PatchTableName => $"[{DatabaseName}].[dbo].[Patches]";

		internal static String BuildConnectionString()
		{
			(string Server, string Db) args = RegistryReader.GetDbConnectionArgs();
			String connString = $"DRIVER={{SQL Server}};SERVER={args.Server};DATABASE={DatabaseName};Integrated Security=SSPI;";
			Console.WriteLine($"Connecion string created {connString}");
			return connString;
		}

		internal static String BuildPatchSelectQuery(EPatchStatus status)
		{
			Process process = Process.GetCurrentProcess();

			Console.WriteLine($"BuildPatchSelectQuery for {process.Id} {process.ProcessName}");

			String q = $@"SELECT [value] FROM {PatchTableName} WHERE [pid] = {process.Id} AND [process_name] = '{process.ProcessName}' AND [status] = {(Int32)status}";

			Console.WriteLine(q);

			return q;
		}

		internal static String BuildUpdateCommandForPatch(Guid id, EPatchStatus status)
		{
			return $"UPDATE {PatchTableName} SET status = {(Int32)status} WHERE id = '{id}'";
		}
	}
}
