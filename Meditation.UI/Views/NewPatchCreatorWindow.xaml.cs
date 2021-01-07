using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using UI.ViewModel;

namespace UI.Views
{
	/// <summary>
	/// Interaction logic for NewPatchCreatorWindow.xaml
	/// </summary>
	public partial class NewPatchCreatorWindow : Window
	{
		public NewPatchCreatorViewModel ViewModel { get; }

		public NewPatchCreatorWindow(NewPatchCreatorViewModel viewModel)
		{
			ViewModel = viewModel;
			ViewModel.ClosingRequest += (sender, e) => Close();
			InitializeComponent();
		}
	}
}
