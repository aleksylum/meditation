//Copyright(c) 2021 Alexandra Kravtsova (aleksylum)

using Meditation.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UI.Model;

namespace UI.Logic
{
	internal class DotNetMethodsLoader : MarshalByRefObject
	{
		private readonly List<Method> _methods = new List<Method>();
		public List<Method> LoadByAssembly(string assemblyPath, Boolean reflectionOnly)
		{
			try
			{
				var assembly = reflectionOnly
						  ? Assembly.ReflectionOnlyLoadFrom(assemblyPath)
						  : Assembly.LoadFile(assemblyPath);

				if (assembly != null)
					LoadTypes(assembly);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}
			return _methods;
		}

		private void LoadTypes(Assembly assembly)
		{
			Type[] types = assembly.GetAvailableTypes();
			String assemblyName = assembly.GetName().Name;
			foreach (Type type in types)
			{
				try
				{
					LoadMethods(type, assemblyName);
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex);
				}
			}
		}

		private void LoadMethods(Type type, String assemblyName)
		{
			var flags = BindingFlags.DeclaredOnly
			            |BindingFlags.Public
			            | BindingFlags.NonPublic
			            | BindingFlags.Instance
			            | BindingFlags.Static
			            | BindingFlags.GetField
			            | BindingFlags.SetField
			            | BindingFlags.GetProperty
			            | BindingFlags.SetProperty;

		MethodInfo[] methodInfos = type.GetMethods(flags);

			//TODO load .ctor etc.
			foreach (MethodInfo methodInfo in methodInfos)
			{
				try
				{
					ParameterInfo[] parameters = methodInfo.GetParameters();
					ArgInfo[] args = parameters.Select(p => new ArgInfo(p.ParameterType.Assembly.GetName().Name, p.ParameterType.Namespace, p.ParameterType.Name)).ToArray();
					String retValue = methodInfo.ReturnParameter.ParameterType.Name;
					//var gen = item.GetGenericArguments(); //TODO support generic
					_methods.Add(new Method(assemblyName, methodInfo, args, retValue));
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex);
				}
			}
		}
	}
}
