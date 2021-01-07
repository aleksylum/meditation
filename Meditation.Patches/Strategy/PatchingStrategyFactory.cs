//Copyright(c) 2021 Alexandra Kravtsova (aleksylum)

using Meditation.Data;
using System;
using System.Reflection;

namespace Meditation.Patches
{
    internal static class PatchingStrategyFactory
    {
		public static IPatchingStrategy Create(CPatch patch, Type type,  MethodInfo info)
		{
			switch (patch.Type)
			{
				case EPatchType.StartFinishParamsWithTimeAndTrace:
					return new StartFinishParamsWithTimeAndTrace(patch, type, info);
				case EPatchType.StackTraceWithTime:
					return new StackTraceWithTimePatchingStrategy(patch, info);
			}

			throw new NotImplementedException();
		}
	}

	internal interface IPatchingStrategy
    {
        void Patch();
    }
}
