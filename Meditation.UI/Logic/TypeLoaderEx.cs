//Copyright(c) 2021 Alexandra Kravtsova (aleksylum)

using System;
using System.Linq;
using System.Reflection;

namespace UI.Logic
{
	internal static class TypeLoaderEx
	{
		public static Type[] GetAvailableTypes(this Assembly assembly)
		{
			try
			{
				return assembly.GetTypes();
			}
			catch (ReflectionTypeLoadException e)
			{
				return e.Types.Where(t => t != null).ToArray();
			}
		}
	}
}
