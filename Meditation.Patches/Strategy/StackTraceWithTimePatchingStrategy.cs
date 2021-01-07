//Copyright(c) 2021 Alexandra Kravtsova (aleksylum)

using HarmonyLib;
using Meditation.Data;
using System;
using System.Reflection;

namespace Meditation.Patches
{
    public class StackTraceWithTimePatchingStrategy : IPatchingStrategy
    {
        private readonly CPatch _patch;
        private readonly MethodInfo _method;

        public StackTraceWithTimePatchingStrategy(CPatch patch, MethodInfo method)
        {
            _patch = patch;
            _method = method;
        }

        public void Patch()
        {
            var harmony = new Harmony(_patch.Id.ToString());
            MethodInfo mPrefix = SymbolExtensions.GetMethodInfo(() => MyPrefix());
            MethodInfo mPostfix = SymbolExtensions.GetMethodInfo(() => MyPostfix());
				harmony.Patch(_method, new HarmonyMethod(mPrefix), new HarmonyMethod(mPostfix));
        }

        public static void MyPrefix()
        {
	        var t = DateTime.UtcNow;
	        Console.WriteLine($"Method started in {t} {t.Millisecond}");
	        Console.WriteLine(Environment.StackTrace);
        }

        public static void MyPostfix()
        {
	        var t = DateTime.UtcNow;
	        Console.WriteLine($"Method finish in {t} {t.Millisecond}");
	        Console.WriteLine(Environment.StackTrace);
		}
	}
}
