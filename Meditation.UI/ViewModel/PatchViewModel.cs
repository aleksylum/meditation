//Copyright(c) 2021 Alexandra Kravtsova (aleksylum)

using Meditation.Data;
using System;

namespace UI.ViewModel
{
	public class PatchViewModel : ViewModelBase
	{
		private CPatch _patch;
		private EPatchStatus _status;
		public CPatch Patch => _patch;
		public EPatchStatus Status => _status;
		public EPatchType Type => _patch.Type;
		public String CustomMessage => _patch.CustomMessage;

		public PatchViewModel(CPatch patch, EPatchStatus status)
		{
			_patch = patch;
			_status = status;
		}
	}
}
