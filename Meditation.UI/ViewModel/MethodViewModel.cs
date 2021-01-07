//Copyright(c) 2021 Alexandra Kravtsova (aleksylum)

using Meditation.Data;
using Meditation.Db;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UI.Model;

namespace UI.ViewModel
{
	public class MethodViewModel : ViewModelBase
	{
		private Method _method;

		private ObservableCollection<PatchViewModel> _allPatches;
		public ObservableCollection<PatchViewModel> AllPatches
		{
			get => _allPatches;
			set
			{
				if (_allPatches != value)
				{
					_allPatches = value;
					OnPropertyChanged(nameof(AllPatches));
				}
			}
		}

		public string Name => _method.Name;
		public string SimpleName => _method.SimpleName;
		public string Namespace => _method.Namespace;
		public string AssemblyName => _method.AssemblyName;
		public String Type => _method.Type;
		public ArgInfo[] InArgs => _method.Args;
		public String Ret => _method.RetValue;

		public string InArgsStr => string.Join(", ", InArgs.Select(a => $"{a.TypeName}"));

		public Byte[] GetHash512(Int32 pid, String processName)
		{
			return _method.GetHash512(pid, processName);
		}

		public MethodViewModel(Method method)
		{
			_method = method;
		}

		internal void LoadPatches(Byte[] methodHash)
		{
			using (PatchDbScope scope = new PatchDbScope())
			{
				List<KeyValuePair<CPatch, EPatchStatus>> patches = scope.GetAllPatchesByMethodHash(methodHash);
				AllPatches = new ObservableCollection<PatchViewModel>(patches.Select(p => new PatchViewModel(p.Key, p.Value)));
			}
		}
	}
}
