//Copyright(c) 2021 Alexandra Kravtsova (aleksylum)

using Meditation.Data;
using Microsoft.Win32;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Meditation.Patches 
{
	public static class MainPatcher
	{
		static MainPatcher()
		{
			AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(MyResolveEventHandler);
		}

		[System.Runtime.InteropServices.DllExport]
		public static string _getConnString()
		{
			return CppDbStringBuilder.BuildConnectionString();
		}

		[DllExport]
		public static string _getPatchQuery()
		{
			return CppDbStringBuilder.BuildPatchSelectQuery(EPatchStatus.New);
		}

		[DllExport]
		public static string _getUnpatchQuery()
		{
			return CppDbStringBuilder.BuildPatchSelectQuery(EPatchStatus.WaitingForUnpatcing);
		}

		[DllExport]
		public static string _patch(string patchInfo)
		{
			try
			{
				Console.WriteLine($"Trying to patch {patchInfo}");
				CPatch patch = Serializer.Deserialize<CPatch>(patchInfo);
				EPatchStatus status = Patcher.PatchSafe(patch); // EPatchStatus.Patched;
				Console.WriteLine($"Patching finished with status {status}");
				return CppDbStringBuilder.BuildUpdateCommandForPatch(patch.Id, status);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Some error during patching process of {patchInfo}\n{ex.Message}");
				return "";//TODO return null; //check for null outside in injectee
			}
		}

		[DllExport]
		public static string _unpatch(string patchInfo)
		{
			try
			{
				Console.WriteLine($"Trying to unpatch {patchInfo}");
				CPatch patch = Serializer.Deserialize<CPatch>(patchInfo);
				EPatchStatus status = Unpatcher.UnpatchSafe(patch); // EPatchStatus.Unpatched;
				Console.WriteLine($"Unatching finished with status {status}");
				return CppDbStringBuilder.BuildUpdateCommandForPatch(patch.Id, status);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Some error during unpatching process of {patchInfo}\n{ex.Message}");
				return "";//TODO return null; //check for null outside in injectee
			}
		}

		private static Assembly MyResolveEventHandler(object sender, ResolveEventArgs args)
		{
			Console.WriteLine("Resolving..." + args.Name);
			try
			{
				RegistryView registryView = Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32;
	
				using (RegistryKey key = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, registryView))//Cannot move to Meditation.Data because of deadlock!!! 
				{
					RegistryKey subkey = key.OpenSubKey(@"SOFTWARE\Meditation");
					String rootPath = (String)subkey.GetValue("MeditationPatchesDll");
					String rootFolder = Path.GetDirectoryName(Path.GetDirectoryName(rootPath));
					String path = $"{rootFolder}\\{args.Name.Split(',')[0]}.dll";
					Console.WriteLine("Loading " + path);
					return Assembly.LoadFile(path);
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Cannot load unknown assembly {args.Name}");
				Console.WriteLine(ex);
				throw;
			}
		}
	}
}

