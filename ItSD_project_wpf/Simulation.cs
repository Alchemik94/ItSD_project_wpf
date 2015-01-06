using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Controls;
using System.Windows.Media;

namespace ItSD_project_wpf
{
	class Simulation: IDisposable
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
		#region Simulation constants
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
		public static double AirFrictionFactor
		{
			get { return 0.001; }
			private set { }
		}
		public static double WallCollisionLooseFactor
		{
			get { return 0.01; }
			private set { }
		}
		public static double BallsRadius
		{
			get { return 25; }
			private set { }
		}
		public static double BallsMass
		{
			get { return 1;}
			private set { }
		}
		public static double Width
		{
			get { return 500; }
			private set { }
		}
		public static double Height
		{
			get { return 500; }
			private set { }
		}
		#endregion

		#region Primary data
		public static int TicksPerSecond { get; set; }
		private volatile Timer _timer;
		private List<Line> _walls;
		private List<Ball> _balls;
		private List<BallDisplayer> _displayers;
		private Canvas _canvas;
		#endregion

		#region Construction of Simulation
		public Simulation(double degreesAngleOfSlipperySlope, Canvas canvas)
		{
			disposed = false;
			_canvas = canvas;
			InitializeBorders(degreesAngleOfSlipperySlope);
			_displayers = new List<BallDisplayer>();
			TicksPerSecond = 100;
			_balls = new List<Ball>();
			_timer = new Timer((double)1 / (double)TicksPerSecond);
			_timer.AutoReset = true;
			_timer.Elapsed += CollisionsCheck;
			_timer.Elapsed += (object sender, ElapsedEventArgs e) =>
			{
				lock (_balls)
					lock (handlerLockObj)
						if(IntervalTimeElapsed!=null)
							IntervalTimeElapsed(sender, e);
			};
		}

		private void InitializeBorders(double degreesAngleOfSlipperySlope)
		{
			if (disposed) throw new ObjectDisposedException(this.ToString());
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
			if (disposed) return;
			lock (_balls)
			{
				List<Ball> oldBalls = (from balls in _balls select new Ball(balls)).ToList();
				Parallel.ForEach(_balls,ball =>
				{
					lock (ball)
					{
						if (disposed) return;
						ball.RecalculateCollisions(oldBalls);
						ball.RecalculateCollisions(_walls);
					}
				});
				Parallel.ForEach(_balls, ball =>
				{
					lock (ball)
					{
						if (disposed) return;
						ball.AddExclusions(_balls);
					}
				});
				Parallel.ForEach(oldBalls, ball =>
				{
					ball.Dispose();
				});
			}
		}

		private ElapsedEventHandler IntervalTimeElapsed;
		private object handlerLockObj = new Object();
		public void AddBall(Ball ball)
		{
			if (disposed) throw new ObjectDisposedException(this.ToString());
			bool toBeEnabled = _timer.Enabled;
			if (toBeEnabled)
				_timer.Stop();
			lock (_balls)
			{
				//if (CollidesWithBalls(ball) || CollidesWithWalls(ball)) return;
				_displayers.Add(new BallDisplayer(_canvas, ball));
				_balls.Add(ball);

				lock (handlerLockObj)
					IntervalTimeElapsed += ((object sender, ElapsedEventArgs e) =>
					{
						if (!disposed)
							lock (ball)
								ball.IntervalTimeElapsed(sender, e);
					});
			}
			if (toBeEnabled)
				_timer.Start();
		}

		#region Any collisions check
		public bool CollidesWithBalls(Ball ball)
		{
			lock (_balls)
			{
				foreach (var singleBall in _balls)
					if (ball!=singleBall && ball.IsColliding(singleBall))
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

		#region Dispose
		private volatile bool disposed;
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		protected virtual void Dispose(bool disposing)
		{
			if (!disposed)
			{
				if (disposing)
				{
					_timer.Stop();
					_timer.Dispose();
					lock(_balls)
						Parallel.ForEach(_balls, ball =>
						{
							if (ball != null)
								lock(ball)
									ball.Dispose();
						});
				}
				Parallel.ForEach(_displayers, displayer =>
				{
					displayer.Clear();
				});
				_canvas.Dispatcher.Invoke(() =>
				{
					_canvas.Children.Clear();
				});
				disposed = true;
			}
		}
		~Simulation()
		{
			try
			{
				Dispose(false);
			}
			catch(TaskCanceledException ex)
			{
				//just ending, no problem
			}
		}
		#endregion
	}
}
