//Copyright(c) 2021 Alexandra Kravtsova (aleksylum)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using Meditation.Data;

namespace Meditation.Patches
{
	public class Patcher
	{
		public static EPatchStatus PatchSafe(CPatch patch)
		{
			try
			{
				Console.WriteLine($"{patch}");
				return DoPatch(patch);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error during patching ({patch}\n)" + ex.Message);
				return EPatchStatus.HasErrorDuringPatching;
			}
		}

		private static EPatchStatus DoPatch(CPatch patch)
		{
			Console.WriteLine("And here in DO PATHCH");

			Console.WriteLine(String.Join(",", patch.Args.ToString()));

			Assembly a = GetAssemblyByName(patch.AssemblyName);

			if (a == null)
			{
				Console.WriteLine($"Cannot find assambly {patch.AssemblyName}");
				return EPatchStatus.HasErrorDuringPatching;
			}

			Console.WriteLine($"Assembly {a.FullName}\n");
			Type t = a.GetType($"{patch.Namespace}.{patch.TypeName}");
			if (t != null)
				Console.WriteLine(t.ToString());
			else
			{
				Console.WriteLine($"Cannot find type {patch.TypeName}");
				return EPatchStatus.HasErrorDuringPatching;
			}

			//Console.WriteLine($"ARGS {String.Join(", ",patch.Args.Select(ar=>ar.GetElementType().Name))}");
			List<Type> types = new List<Type>();
			foreach (var arg in patch.Args)
			{
				Console.WriteLine($"FOR {arg}");
				Assembly assembly = GetAssemblyByName(arg.AssemblyName);
				if (assembly == null)
				{
					Console.WriteLine($"Cannot find {arg.AssemblyName}");
					continue;
				}
				Console.WriteLine(assembly.FullName);
				Type type = assembly.GetType($"{arg.Namespace}.{arg.TypeName}");
				Console.WriteLine(type);
				types.Add(type);
			}

			MethodInfo originalMethod = AccessTools.Method(t, patch.MethodName, types.ToArray());

			if (originalMethod != null)
				Console.WriteLine(originalMethod.ToString());
			else
			{
				Console.WriteLine($"Cannot find method {patch.MethodName}");
				return EPatchStatus.HasErrorDuringPatching;
			}

			IPatchingStrategy strategy = PatchingStrategyFactory.Create(patch, t, originalMethod);

			strategy.Patch();
			Console.WriteLine("Patch succeded");
			return EPatchStatus.Patched;
		}

		private static Assembly GetAssemblyByName(string name)
		{
			return AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(assembly => assembly.GetName().Name == name);//TODO SingleOrDefault?
		}
	}
}
