//Copyright(c) 2021 Alexandra Kravtsova (aleksylum)

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using UI.Logic;
using UI.Model;

namespace UI.ViewModel
{
	public class ProcessViewModel : ViewModelBase
	{
		private ProcessModel _process;

		public Process InnerProcess => _process.Process;

		private bool _isSelected;
		public bool IsSelected
		{
			get => _isSelected;
			set
			{
				_isSelected = value;
				OnPropertyChanged(nameof(IsSelected));
			}
		}

		private ObservableCollection<MethodViewModel> _allMethodsOfAssembly;
		public ObservableCollection<MethodViewModel> AllMethods
		{
			get => _allMethodsOfAssembly;
			set
			{
				_allMethodsOfAssembly = value;
				OnPropertyChanged(nameof(AllMethods));
			}
		}

		public void LoadMethods()
		{
			if (AllMethods != null)
				return;

			using (ProcessMethodsLoader processMethodsLoader = new ProcessMethodsLoader(_process.PID))
			{
				IReadOnlyList<Method> methods = processMethodsLoader.Load();
				AllMethods = new ObservableCollection<MethodViewModel>(methods.Distinct().Select(m => new MethodViewModel(m)));
				//TODO check: smtms equals does not work with object methods!!! 
			}
		}

		public string Name => _process.Name;
		public Int32 PID => _process.PID;

		public ProcessViewModel(ProcessModel process)
		{
			_process = process;
		}
	}
}
