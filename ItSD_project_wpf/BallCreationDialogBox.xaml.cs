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
	/// Interaction logic for BallCreationDialog.xaml
	/// </summary>
	public partial class BallCreationDialogBox : Window
	{
		public bool CollectedData
		{
			get;
			private set;
		}
		public double Mass
		{
			get { return MassSlider.Value; }
			private set { }
		}
		public double VelocityLength
		{
			get { return VelocitySlider.Value; }
			private set { }
		}
		public double VelocityAngle
		{
			get { return AngleVelocitySlider.Value; }
			private set { }
		}

		public BallCreationDialogBox()
		{
			InitializeComponent();
			CollectedData = false;
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			CollectedData = true;
			Close();
		}
	}
}
