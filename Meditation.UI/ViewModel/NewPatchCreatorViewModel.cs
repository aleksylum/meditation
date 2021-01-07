//Copyright(c) 2021 Alexandra Kravtsova (aleksylum)

using Meditation.Data;
using Prism.Commands;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace UI.ViewModel
{
	public class NewPatchCreatorViewModel : CloseableViewModel
	{
		public bool Result { get; private set; }

		//TODO add normal proper name to be dispalyed in UI
		public IReadOnlyDictionary<EPatchType, string> PatchNameByType { get; }
		= new Dictionary<EPatchType, string>
			{
				{EPatchType.StartFinishParamsWithTimeAndTrace ,"StartFinishParamsWithTimeAndTrace"},
				{EPatchType.StackTraceWithTime ,"StackTrace"}
			};

		private KeyValuePair<EPatchType, string> _selectedPatchType;
		public KeyValuePair<EPatchType, string> SelectedPatchType
		{
			get => _selectedPatchType;
			set
			{
				_selectedPatchType = value;
				OnPropertyChanged(nameof(SelectedPatchType));
			}
		}

		private string _messageText;
		public string MessageText
		{
			get => _messageText;
			set
			{
				if (_messageText != value)
				{
					_messageText = value;
					OnPropertyChanged(nameof(MessageText));
				}
			}
		}

		#region Commands

		private ICommand _okCommand;
		public ICommand OkCommand =>
			_okCommand ?? (_okCommand = new DelegateCommand(SaveAndExit));

		private ICommand _cancelCommand;
		public ICommand CancelCommand =>
			_cancelCommand ?? (_cancelCommand = new DelegateCommand(ExitWithoutSaving));

		#endregion


		public NewPatchCreatorViewModel()
		{
			SelectedPatchType = PatchNameByType.SingleOrDefault(p => p.Key == (EPatchType)1);
		}

		private void SaveAndExit()
		{
			Result = true;
			Close();
		}

		private void ExitWithoutSaving()
		{
			Result = false;
			Close();
		}

		private void Close()
		{
			OnClosingRequest();
		}
	}
}
