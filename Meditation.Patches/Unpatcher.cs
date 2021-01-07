//Copyright(c) 2021 Alexandra Kravtsova (aleksylum)

using HarmonyLib;
using Meditation.Data;
using System;

namespace Meditation.Patches
{
	internal class Unpatcher
	{
		public static EPatchStatus UnpatchSafe(CPatch patch)
		{
			try
			{
				DoUnpatch(patch);
				return EPatchStatus.Unpatched;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error during unpatching ({patch}\n)" + ex.Message);
				return EPatchStatus.HasErrorDuringUnpatching;
			}
		}

		private static void  DoUnpatch(CPatch patch)
		{
			var harmony = new Harmony(patch.Id.ToString());
			harmony.UnpatchAll();
		}
	}
}
