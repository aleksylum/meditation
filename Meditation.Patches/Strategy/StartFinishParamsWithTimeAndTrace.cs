//Copyright(c) 2021 Alexandra Kravtsova (aleksylum)

using HarmonyLib;
using Meditation.Data;
using System;
using System.Reflection;
using System.Text;

namespace Meditation.Patches
{
    internal class StartFinishParamsWithTimeAndTrace : IPatchingStrategy
    {
		private readonly CPatch _patch;
		private readonly MethodInfo _method;

		public StartFinishParamsWithTimeAndTrace(CPatch patch, Type type, MethodInfo method)
		{
			_patch = patch;
			_method = method;
		}

		public void Patch()
		{
			Console.WriteLine("Creating a new Patch...");

			var harmony = new Harmony(_patch.Id.ToString());
			MethodInfo m = AccessTools.Method(GetType(), "MyPrefix");
			MethodInfo m2 = AccessTools.Method(GetType(), "MyPostfix");

			harmony.Patch(_method, new HarmonyMethod(m), new HarmonyMethod(m2));
			Console.WriteLine("Patch created.");
		}

		public static void MyPrefix(params (String, Object)[] __params)
		{
			Console.WriteLine($"Trace: {Environment.StackTrace}");
			StringBuilder sb = new StringBuilder();
			sb.Append("Arguments: \n");

			foreach ((String argName, Object argValue) in __params)
			{
				Type type = argValue?.GetType();
				var v = (dynamic)(argValue);
				String js = Newtonsoft.Json.JsonConvert.SerializeObject(v) ?? "null";
				sb.Append($"\t{type.FullName} {argName} = {js}\n");
			}
			Console.WriteLine(sb.ToString());
			DateTime time = DateTime.UtcNow;
			Console.WriteLine($"Method started in {time} {time.Millisecond}");
		}

		public static void MyPostfix(ref object __result)
		{
			var t = DateTime.UtcNow;
			Console.WriteLine($"Method finish in {t} {t.Millisecond}");
			var v = (dynamic)(__result);
			String js = Newtonsoft.Json.JsonConvert.SerializeObject(v);
			Console.WriteLine($"Result:\n\t{js}");
			Console.WriteLine($"Trace: {Environment.StackTrace}");
		}
	}
}
