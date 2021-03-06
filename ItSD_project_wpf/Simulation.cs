﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Controls;
using System.Windows.Media;

namespace ItSD_project_wpf
{
	class Simulation
	{
		#region Approximate calculations
		private const double epsilon = 0.001;
		public static bool IsZero(double num)
		{
			if (Math.Abs(num) <= epsilon)
				return true;
			return false;
		}
		#endregion
		#region Gravity acceleration
		private static object _lockObj = new object();
		private static Vector _gravity;
		public static Vector Gravity 
		{
			get
			{
				if(_gravity==null)
					lock(_lockObj)
					{
						if (_gravity == null)
							_gravity = new Vector(Point.Origin, new Point(0, -100));
					}
				return _gravity;
			}
			private set
			{
				_gravity = value;
			}
		}
		#endregion

		#region Primary data
		public static int TicksPerSecond { get; set; }
		private Timer _timer;
		private List<Line> _walls;
		private List<Ball> _balls;
		private List<BallDisplayer> _displayers;
		private Canvas _canvas;
		#endregion

		#region Construction of Simulation
		public Simulation(double degreesAngleOfSlipperySlope, Canvas canvas)
		{
			_canvas = canvas;
			InitializeBorders(degreesAngleOfSlipperySlope);
			_displayers = new List<BallDisplayer>();
			TicksPerSecond = 100;
			_balls = new List<Ball>();
			_timer = new Timer((double)1 / (double)TicksPerSecond);
			_timer.AutoReset = true;
			_timer.Elapsed += CollisionsCheck;
		}

		private void InitializeBorders(double degreesAngleOfSlipperySlope)
		{
			_walls = new List<Line>();
			_walls.Add(new Line(new Point(500, 0), new Point(500, 500)));
			_walls.Add(new Line(new Point(500, 500), new Point(0, 500)));
			if (degreesAngleOfSlipperySlope >= 45 && degreesAngleOfSlipperySlope < 90)
			{
				_walls.Add(new Line(new Point(0, 500), new Point(0, 250)));
				_walls.Add(new Line(new Point(0, 250), new Point(250/Math.Tan(degreesAngleOfSlipperySlope*Math.PI/180),0)));
				_walls.Add(new Line(new Point(250 / Math.Tan(degreesAngleOfSlipperySlope * Math.PI / 180), 0),new Point(500, 0)));
			}
			else if (degreesAngleOfSlipperySlope <= 45 && degreesAngleOfSlipperySlope >= 0)
			{
				_walls.Add(new Line(new Point(0, 500), new Point(0, 250*Math.Tan(degreesAngleOfSlipperySlope*Math.PI/180))));
				_walls.Add(new Line(new Point(0, 250 * Math.Tan(degreesAngleOfSlipperySlope * Math.PI / 180)), new Point(250, 0)));
				_walls.Add(new Line(new Point(250, 0), new Point(500, 0)));
			}
			else
				throw new ArgumentOutOfRangeException("Angle is out of the allowed interval.");
			foreach(Line border in _walls)
			{
				System.Windows.Shapes.Line line = new System.Windows.Shapes.Line();
				line.Stroke = Brushes.Black;
				line.StrokeThickness = 1;
				line.X1 = border.First.X;
				line.Y1 = 500 - border.First.Y;
				line.X2 = border.Second.X;
				line.Y2 = 500 - border.Second.Y;
				_canvas.Children.Add(line);
			}
		}
		#endregion

		private void CollisionsCheck(object sender, ElapsedEventArgs e)
		{
			lock (_balls)
			{
				var oldBalls = from balls in _balls select new Ball(balls);
				Parallel.ForEach(_balls,ball =>
					{
						lock (ball)
						{
							ball.RecalculateCollisions(_walls);
							ball.RecalculateCollisions(oldBalls);
						}
					}
					);
				//foreach (var ball in _balls)
				//{
				//	ball.RecalculateCollisions(oldBalls);
				//	ball.RecalculateCollisions(_walls);
				//}
			}
		}

		public void AddBall(Ball ball)
		{
			if (CollidesWithBalls(ball) || CollidesWithWalls(ball)) return;
			bool toBeEnabled = false;
			if (_timer.Enabled)
			{
				_timer.Stop();
				toBeEnabled = true;
			}
			_displayers.Add(new BallDisplayer(_canvas, ball));
			_balls.Add(ball);
			_timer.Elapsed += ((object sender, ElapsedEventArgs e) =>
			{
				lock (ball)
				{
					ball.IntervalTimeElapsed(sender, e);
				}
			});
			if(toBeEnabled)
				_timer.Start();
		}

		#region Any collisions check
		public bool CollidesWithBalls(Ball ball)
		{
			lock (_balls)
			{
				foreach (var singleBall in _balls)
					if (ball.IsColliding(singleBall))
						return true;
			}
			return false;
		}
		public bool CollidesWithWalls(Ball ball)
		{
			lock (_walls)
			{
				foreach (var wall in _walls)
					if (ball.IsColliding(wall))
						return true;
			}
			return false;
		}
		#endregion

		#region Steering
		public void Start()
		{
			_timer.Start();
		}
		public void Stop()
		{
			_timer.Stop();
		}
		#endregion
	}
}
