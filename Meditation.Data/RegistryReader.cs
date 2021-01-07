//Copyright(c) 2021 Alexandra Kravtsova (aleksylum)

using Microsoft.Win32;
using System;

namespace Meditation.Data
{
	public static class RegistryReader
	{
		public static String BaseKeyName = @"SOFTWARE\Meditation";
		private const String ServerKeyName = "Server";
		private const String DatabaseKeyName = "Database";
		private const String MeditationInjectionPathKeyName = "MeditationInjectionPath";

		public static String GetMeditationInjectionPath()
		{
			RegistryView registryView = Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32;

			using (RegistryKey key = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, registryView))
			{
				RegistryKey subkey = key.OpenSubKey(BaseKeyName);
				String path = (String)subkey.GetValue(MeditationInjectionPathKeyName);
				return path;
			}
		}

		public static (String Server, String Db) GetDbConnectionArgs()
		{
			RegistryView registryView = Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32;

			using (RegistryKey key = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, registryView))
			{
				RegistryKey subkey = key.OpenSubKey(BaseKeyName);
				Object server = subkey.GetValue(ServerKeyName);
				Object dbase = subkey.GetValue(DatabaseKeyName);
				return ((String)server, (String)dbase);
			}
		}
	}
}
