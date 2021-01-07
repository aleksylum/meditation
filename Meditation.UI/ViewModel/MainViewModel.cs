//Copyright(c) 2021 Alexandra Kravtsova (aleksylum)

using Meditation.Db;
using Meditation.Data;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using UI.Model;
using System.Windows.Input;
using Prism.Commands;
using System;
using System.Threading.Tasks;
using UI.Logic;
using UI.Views;

namespace UI.ViewModel
{
	public class MainViewModel : ViewModelBase
	{
		private ObservableCollection<ProcessViewModel> _allRunningProcesses;
		public ObservableCollection<ProcessViewModel> AllRunningProcesses
		{
			get => _allRunningProcesses;
			set
			{
				_allRunningProcesses = value;
				OnPropertyChanged(nameof(AllRunningProcesses));
			}
		}

		private ProcessViewModel _selectedProcess;
		public ProcessViewModel SelectedProcess
		{
			get => _selectedProcess;
			set
			{
				if (_selectedProcess != value)
				{
					_selectedProcess = value;
					OnPropertyChanged(nameof(SelectedProcess));

					NewPatchCommand.RaiseCanExecuteChanged();
					ApplyPatchesCommand.RaiseCanExecuteChanged();
					UnpatchSelectedCommand.RaiseCanExecuteChanged();
					if (SelectedProcess != null)
					{
						Task.Run(() =>
						{
							IsBusy = true;

							SelectedProcess.LoadMethods();
							AllMethods = new ObservableCollection<MethodViewModel>(SelectedProcess.AllMethods);

							IsBusy = false;
						});
					}
					else
					{
						AllMethods = new ObservableCollection<MethodViewModel>();
					}

					SelectedMethod = null;
					SelectedPatch = null;
				}
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

		private MethodViewModel _selectedMethod;
		public MethodViewModel SelectedMethod
		{
			get => _selectedMethod;
			set
			{
				if (_selectedMethod != value)
				{
					_selectedMethod = value;
					OnPropertyChanged(nameof(SelectedMethod));

					NewPatchCommand.RaiseCanExecuteChanged();
					ApplyPatchesCommand.RaiseCanExecuteChanged();
					UnpatchSelectedCommand.RaiseCanExecuteChanged();

					if (_selectedMethod == null)
					{
						AllPatches = null;
						return;
					}

					ReloadPatches();
				}
			}
		}

		private void ReloadPatches()
		{
			SelectedMethod.LoadPatches(_selectedMethod.GetHash512(SelectedProcess.PID, SelectedProcess.Name));
			AllPatches = SelectedMethod.AllPatches;
			SelectedPatch = null;
		}

		private ObservableCollection<PatchViewModel> _allPatchesOfMethod;
		public ObservableCollection<PatchViewModel> AllPatches
		{
			get => _allPatchesOfMethod;
			set
			{
				_allPatchesOfMethod = value;
				OnPropertyChanged(nameof(AllPatches));
			}
		}

		private PatchViewModel _selectedPatch;
		public PatchViewModel SelectedPatch
		{
			get => _selectedPatch;
			set
			{
				if (_selectedPatch != value)
				{
					_selectedPatch = value;
					OnPropertyChanged(nameof(SelectedPatch));

					UnpatchSelectedCommand.RaiseCanExecuteChanged();
				}
			}
		}

		private bool _isBusy;
		public bool IsBusy
		{
			get => _isBusy;
			private set
			{
				if (_isBusy != value)
				{
					_isBusy = value;
					OnPropertyChanged(nameof(IsBusy));
				}
			}
		}

		#region Commands

		private ICommand _refreshCommand;
		public ICommand RefreshCommand =>
			_refreshCommand ?? (_refreshCommand = new DelegateCommand(Refresh));

		private DelegateCommand _newPatchCommand;
		public DelegateCommand NewPatchCommand
		{
			get
			{
				if (_newPatchCommand == null)
				{
					_newPatchCommand = new DelegateCommand(OpenNewPatchCreator, CanOpenNewPatchCreator);
				}
				return _newPatchCommand;
			}
		}

		private DelegateCommand _applyPatchesCommand;
		public DelegateCommand ApplyPatchesCommand
		{
			get
			{
				if (_applyPatchesCommand == null)
				{
					_applyPatchesCommand = new DelegateCommand(ApplyPatches, CanApplyPatches);
				}
				return _applyPatchesCommand;
			}
		}

		private DelegateCommand _unpatchSelectedCommand;
		public DelegateCommand UnpatchSelectedCommand
		{
			get
			{
				if (_unpatchSelectedCommand == null)
				{
					_unpatchSelectedCommand = new DelegateCommand(UnpatchSelected, CanUnpatchSelected);
				}
				return _unpatchSelectedCommand;
			}
		}

		private DelegateCommand _refreshPatchesCommand;
		public DelegateCommand RefreshPatchesCommand
		{
			get
			{
				if (_refreshPatchesCommand == null)
				{
					_refreshPatchesCommand = new DelegateCommand(ReloadPatches);
				}

				return _refreshPatchesCommand;
			}
		}

		#endregion

		private void InitializeDb()
		{
			using (PatchDbScope patchDbScope = new PatchDbScope())
			{
				//patchDbScope.ClearPatches(); //TODO really need it?
			}
		}

		public MainViewModel()
		{
			InitializeDb();
			AllRunningProcesses = new ObservableCollection<ProcessViewModel>();
			LoadProcesses();
		}

		private void LoadProcesses()
		{
			//TEMP FILTER FOR TEST!!!
			//IReadOnlyList<Process> processes = processes2.Where(p => (p.ProcessName.Contains("FrameworkSampleConsoleApp"))).ToList();

			IReadOnlyList<Process> newProcesses = Process.GetProcesses();//.ToHashSet(new ProcessEqualityComparer());

			//TODO Doesnt work because Assambly can be loaded dynamically in domains
			//HashSet<Process> curProcesses = AllRunningProcesses.Select(p => p.InnerProcess).ToHashSet(new ProcessEqualityComparer());

			//RemoveExpiredProcesses(newProcesses, curProcesses);
			//AddNewProcesses(newProcesses, curProcesses);

			IEnumerable<ProcessModel> processesModels = newProcesses.Select(p => new ProcessModel(p));
			AllRunningProcesses = new ObservableCollection<ProcessViewModel>(processesModels.Select(pm => new ProcessViewModel(pm)).OrderBy(p => p.Name));
		}

		private void AddNewProcesses(HashSet<Process> newProcesses, HashSet<Process> curProcesses)
		{
			var addedp = newProcesses.Except(curProcesses, new ProcessEqualityComparer());

			foreach (var proc in addedp)
			{
				AllRunningProcesses.Add(new ProcessViewModel(new ProcessModel(proc)));
			}
		}

		private void RemoveExpiredProcesses(HashSet<Process> newProcesses, HashSet<Process> curProcesses)
		{
			var comparer = new ProcessEqualityComparer();
			var expp = curProcesses.Except(newProcesses, comparer);

			foreach (var proc in expp)
			{
				var pvm = AllRunningProcesses.First(p => comparer.Equals(p.InnerProcess,proc));
				AllRunningProcesses.Remove(pvm);
			}
		}

		private void Refresh()
		{
			SelectedMethod = null;
			SelectedProcess = null;
			AllPatches = null;
			AllMethods = null;

			LoadProcesses();//TODO need to reload methods if user need it!!!!
		}

		private void OpenNewPatchCreator()
		{
			var newPatchCreatorViewModel = new NewPatchCreatorViewModel();
			new NewPatchCreatorWindow(newPatchCreatorViewModel).ShowDialog();

			if (newPatchCreatorViewModel.Result)
			{

				CPatch newPatch = new CPatch(
					Guid.NewGuid(),
					newPatchCreatorViewModel.SelectedPatchType.Key,
					SelectedMethod.Namespace,
					SelectedMethod.AssemblyName,
					SelectedMethod.Type,
					SelectedMethod.SimpleName,
					SelectedMethod.InArgs,
					newPatchCreatorViewModel.MessageText);

				using (PatchDbScope scope = new PatchDbScope())
				{
					scope.CreatePatch(newPatch,
						SelectedProcess.PID,
						SelectedProcess.Name,
						EPatchStatus.New,
						SelectedMethod.GetHash512(SelectedProcess.PID, SelectedProcess.Name));
				}

				ReloadPatches();
			}
		}

		private void ApplyPatches()
		{
			String filename = @"Injector.exe";
			Process proc = new Process();
			proc.StartInfo.Verb = "runas";
			proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
			proc.StartInfo.Arguments = SelectedProcess.PID.ToString();
			proc.StartInfo.WorkingDirectory = RegistryReader.GetMeditationInjectionPath();
			proc.StartInfo.FileName = filename;
			proc.Start();
			//ReloadPatches();
		}

		private void UnpatchSelected()
		{
			if (SelectedPatch == null)
				return;

			using (PatchDbScope scope = new PatchDbScope())
			{
				scope.UpdatePatchStatus(SelectedPatch.Patch.Id, EPatchStatus.WaitingForUnpatcing);
				ReloadPatches();
			}
		}

		private bool CanApplyPatches()
		{
			if (!CheckForNull())
			{
				return false;
			}

			return true;
		}

		private bool CanOpenNewPatchCreator()
		{
			if (!CheckForNull())
			{
				return false;
			}

			return true;
		}

		private bool CanUnpatchSelected()
		{

			if (SelectedPatch != null)//&& SelectedPatch.Status == EPatchStatus.Patched )// || p.Status == EPatchStatus.HasErrorDuringPatching)
				return true;

			return false;
		}

		private bool CheckForNull()
		{
			return SelectedProcess != null && SelectedMethod != null;
		}
	}
}
