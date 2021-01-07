//Copyright(c) 2021 Alexandra Kravtsova (aleksylum)

using System;

namespace UI.ViewModel
{
	public class CloseableViewModel : ViewModelBase
	{
		public event EventHandler ClosingRequest;

		protected void OnClosingRequest()
		{
			ClosingRequest?.Invoke(this, EventArgs.Empty);
		}
	}
}
