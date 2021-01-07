//Copyright(c) 2021 Alexandra Kravtsova (aleksylum)

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Diagnostics.Runtime;
using UI.Model;

namespace UI.Logic
{
	internal class ProcessMethodsLoader : IDisposable
	{
		private readonly AssemblyMethodsLoader _assemblyMethodsLoader;
		private readonly Int32 _pid;
		private readonly List<Method> _methods;

		public ProcessMethodsLoader(Int32 pid)
		{
			_assemblyMethodsLoader = new AssemblyMethodsLoader(new DomainOwner());
			_methods = new List<Method>();
			_pid = pid;
		}

		public IReadOnlyList<Method> Load()
		{
			try
			{
				Process process = Process.GetProcessById(_pid); //Check process alive
				LoadClrVersions();
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
			}

			return _methods;
		}

		private void LoadClrVersions()
		{
			using (DataTarget dataTarget = DataTarget.AttachToProcess(_pid, 5000, AttachFlag.Passive))
			{
				foreach (ClrInfo clr in dataTarget.ClrVersions)
				{
					try
					{
						if (!clr.ModuleInfo.GetPEImage().IsPE64)
							continue;

						var runtime = clr.CreateRuntime();
						LoadAppDomains(runtime.AppDomains);
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.Message);
					}
				}
			}
		}

		private void LoadAppDomains(IList<ClrAppDomain> domains)
		{
			foreach (ClrAppDomain domain in domains)
			{
				try
				{
					LoadModules(domain.Modules);
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex.Message);
				}
			}
		}

		private void LoadModules(IList<ClrModule> modules)
		{
			foreach (ClrModule module in modules)
			{
				try
				{
					if (module.AssemblyName != null && !module.AssemblyName.StartsWith("C:\\WINDOWS", StringComparison.InvariantCultureIgnoreCase))
						_methods.AddRange(_assemblyMethodsLoader.GetDotNetMethods(module.AssemblyName, false));
				}
				catch (Exception e)
				{
					Console.WriteLine(e);
				}
			}
		}

		public void Dispose()
		{
			_assemblyMethodsLoader?.Dispose();
		}
	}
}
