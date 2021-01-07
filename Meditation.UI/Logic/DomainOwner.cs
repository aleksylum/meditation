//Copyright(c) 2021 Alexandra Kravtsova (aleksylum)

using System;
using System.Security.Policy;

namespace UI.Logic
{
	internal class DomainOwner : IDisposable
	{
		private static readonly String NamePrefix = "DomainForTypeAndMethodInfoLoading__";
		private readonly AppDomain _appDomain;

		public DomainOwner()
		{
			String friendlyName = NamePrefix + Guid.NewGuid();
			Evidence evidence = new Evidence(AppDomain.CurrentDomain.Evidence);
			AppDomainSetup setup = new AppDomainSetup { ApplicationBase = AppDomain.CurrentDomain.SetupInformation.ApplicationBase };
			//TODO Mb we need special app domains with special settings for different specific assemblies?  
			_appDomain = AppDomain.CreateDomain(friendlyName, evidence, setup);
		}

		public T CreateInstanceAndUnwrap<T>() where T : MarshalByRefObject
		{
			return (T)_appDomain.CreateInstanceAndUnwrap(typeof(T).Assembly.FullName, typeof(T).FullName);
		}

		public void Dispose()
		{
			AppDomain.Unload(_appDomain);
		}
	}
}
