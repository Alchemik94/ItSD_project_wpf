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

namespace ItSD_project_wpf
{
	/// <summary>
	/// Interaction logic for AngleDialogBox.xaml
	/// </summary>
	public partial class AngleDialogBox : Window
	{
		public double Angle
		{
			get { return AngleSlider.Value; }
			private set { AngleSlider.Value = value; }
		}
		public bool CollectedData
		{
			get;
			private set;
		}
		public AngleDialogBox()
		{
			InitializeComponent();
			CollectedData = false;
		}

		private void OK_Button_Click(object sender, RoutedEventArgs e)
		{
			CollectedData = true;
			Close();
		}
	}
}
