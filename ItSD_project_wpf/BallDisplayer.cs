﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ItSD_project_wpf
{
	class BallDisplayer: IDisposable
	{
		#region Graphics initialization
		private Canvas _displayCanvas;
		public Canvas DisplayCanvas
		{
			get
			{
				return _displayCanvas;
			}
			private set
			{
				if(_displayCanvas!=null)
					_displayCanvas.Children.Remove(_ellipse);
				_displayCanvas = value;
				Canvas.SetBottom(_ellipse, DisplayedBall.Position.Y - DisplayedBall.Radius);
				Canvas.SetLeft(_ellipse, DisplayedBall.Position.X - DisplayedBall.Radius);
				_displayCanvas.Children.Add(_ellipse);
			}
		}
		private Ellipse _ellipse;
		#endregion
		
		public Ball DisplayedBall
		{
			get;
			private set;
		}

		public BallDisplayer(Canvas canvas, Ball ball)
		{
			_ellipse = new Ellipse();
			_ellipse.Fill = Brushes.Red;
			_ellipse.Stroke = Brushes.Red;
			_ellipse.StrokeThickness = 0;
			_ellipse.Width = _ellipse.Height = 2 * ball.Radius;
			DisplayedBall = ball;
			DisplayCanvas = canvas;
			DisplayedBall.IntervalTimeElapsed += Refresh;
		}

		private void Refresh(object sender, EventArgs e)
		{
			_displayCanvas.Dispatcher.Invoke((Action)(() =>
			{
				Canvas.SetBottom(_ellipse, DisplayedBall.Position.Y - DisplayedBall.Radius);
				Canvas.SetLeft(_ellipse, DisplayedBall.Position.X - DisplayedBall.Radius);
			}));
		}

		//Doesn't dispose the ball!
		public void Dispose()
		{
			if(DisplayCanvas != null)
				DisplayCanvas.Children.Remove(_ellipse);
		}
	}
}
