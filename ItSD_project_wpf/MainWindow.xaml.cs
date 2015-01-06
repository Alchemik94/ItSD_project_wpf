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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ItSD_project_wpf
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private Simulation _simulation;
		public MainWindow()
		{
			InitializeComponent();
		}

		private void MenuItem_Click_Exit(object sender, RoutedEventArgs e)
		{
			Close();
		}

		#region Simulation steering
		private void MenuItem_Click_NewSimulation(object sender, RoutedEventArgs e)
		{
			var dialog = new AngleDialogBox();
			dialog.Owner = this;
			dialog.ShowDialog();
			if (dialog.CollectedData)
			{
				if (_simulation != null)
				{
					_simulation.Stop();
					_simulation.Dispose();
					_simulation = null;
				}
				_simulation = new Simulation(dialog.Angle, SimulationCanvas);
				SimulationCategory.IsEnabled = true;
				StartButton.IsEnabled = true;
				AddBallButton.IsEnabled = true;
				StopButton.IsEnabled = false;
			}
		}

		private void MenuItem_Click_Start(object sender, RoutedEventArgs e)
		{
			NewSimulationButton.IsEnabled = false;
			StartButton.IsEnabled = false;
			AddBallButton.IsEnabled = false;
			StopButton.IsEnabled = true;
			_simulation.Start();
		}

		private void MenuItem_Click_Stop(object sender, RoutedEventArgs e)
		{
			_simulation.Stop();
			NewSimulationButton.IsEnabled = true;
			StartButton.IsEnabled = true;
			AddBallButton.IsEnabled = true;
			StopButton.IsEnabled = false;
		}
		#endregion

		private volatile bool CatchingNewBall;
		private Ball _catchedBall;
		private Ellipse _catchedBallVisualization;
		private void MenuItem_Click_AddBall(object sender, RoutedEventArgs e)
		{
			CatchingNewBall = true;
			_catchedBall = new Ball(new Point(250, 450), Vector.ZeroVector, Simulation.BallsRadius, Simulation.BallsMass);
			_catchedBallVisualization = new Ellipse();
			_catchedBallVisualization.Fill = Brushes.Red;
			_catchedBallVisualization.Stroke = Brushes.Red;
			_catchedBallVisualization.StrokeThickness = 0;
			_catchedBallVisualization.Width = _catchedBallVisualization.Height = 2 * Simulation.BallsRadius;
			SimulationCanvas.Children.Add(_catchedBallVisualization);
			Canvas.SetBottom(_catchedBallVisualization, _catchedBall.Position.Y - _catchedBall.Radius);
			Canvas.SetLeft(_catchedBallVisualization, _catchedBall.Position.X - _catchedBall.Radius);

			SimulationCanvas.CaptureMouse();
			//while (CatchingNewBall) System.Threading.Thread.Yield();
			//_simulation.AddBall(new Ball(new Point(250, 450), Vector.ZeroVector, Simulation.BallsRadius, Simulation.BallsMass));
		}

		private void SimulationCanvas_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (CatchingNewBall)
			{
				SimulationCanvas.Children.Remove(_catchedBallVisualization);
				_catchedBallVisualization = null;
				_simulation.AddBall(_catchedBall);
				CatchingNewBall = false;
				SimulationCanvas.ReleaseMouseCapture();
			}
		}

		private void SimulationCanvas_MouseMove(object sender, MouseEventArgs e)
		{
			if(CatchingNewBall)
			{
				lock (_catchedBall)
				{
					Point old = _catchedBall.Position;
					_catchedBall.Position.X = e.GetPosition(SimulationCanvas).X;
					_catchedBall.Position.Y = SimulationCanvas.Height - e.GetPosition(SimulationCanvas).Y;
					if (_simulation.CollidesWithBalls(_catchedBall) || _simulation.CollidesWithWalls(_catchedBall))
						_catchedBall.Position = old;
					else
					{
						Canvas.SetBottom(_catchedBallVisualization, _catchedBall.Position.Y - _catchedBall.Radius);
						Canvas.SetLeft(_catchedBallVisualization, _catchedBall.Position.X - _catchedBall.Radius);
					}
				}
			}
		}

		private void SimulationCanvas_LostMouseCapture(object sender, MouseEventArgs e)
		{
			CatchingNewBall = false;
			SimulationCanvas.Children.Remove(_catchedBallVisualization);
			_catchedBallVisualization = null;
			_catchedBall = null;
			ReleaseMouseCapture();
		}
	}
}
