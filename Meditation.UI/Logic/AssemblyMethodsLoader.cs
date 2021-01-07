//Copyright(c) 2021 Alexandra Kravtsova (aleksylum)

using System;
using System.Collections.Generic;
using UI.Model;

namespace UI.Logic
{
	internal class AssemblyMethodsLoader : IDisposable
	{
		private readonly DotNetMethodsLoader _loader;
		private readonly DomainOwner _domainOwner;

		public AssemblyMethodsLoader(DomainOwner domainOwner)
		{
			_domainOwner = domainOwner;
			_loader = _domainOwner.CreateInstanceAndUnwrap<DotNetMethodsLoader>();
		}

		public List<Method> GetDotNetMethods(String assemblyPath, Boolean reflectionOnly)
		{
			return _loader.LoadByAssembly(assemblyPath, reflectionOnly);
		}

		public void Dispose()
		{
			_domainOwner?.Dispose();
		}
	}
}
