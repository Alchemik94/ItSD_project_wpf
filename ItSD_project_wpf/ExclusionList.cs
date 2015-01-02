using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace ItSD_project_wpf
{
	public class ExclusionList: IDisposable
	{
		private HashSet<Ball> _balls;
		private HashSet<Line> _borders;
		private List<Timer> _waitingTimers;
		private Timer _cleaner;
		
		public ExclusionList()
		{
			_balls = new HashSet<Ball>();
			_borders = new HashSet<Line>();
			_waitingTimers = new List<Timer>();
			_cleaner = new Timer(50);
			_cleaner.AutoReset = true;
			_cleaner.Elapsed += ((object sender, ElapsedEventArgs e) =>
			{
				lock (_waitingTimers)
				{
					while (_waitingTimers.Count() > 0 && _waitingTimers.First().Enabled == false)
					{
						_waitingTimers.ElementAt(0).Dispose();
						_waitingTimers.RemoveAt(0);
					}
				}
			});
			_cleaner.Start();
		}

		public void Add(Ball ball, double expirationTime)
		{
			Timer expirationTimer = new Timer(expirationTime);
			expirationTimer.AutoReset = false;
			expirationTimer.Elapsed += ((object sender, ElapsedEventArgs e) =>
			{
				lock (_balls)
				{
					_balls.Remove(ball);
				}
			});
			lock (_balls)
			{
				_balls.Add(ball);
				expirationTimer.Start();
			}
			lock (_waitingTimers)
			{
				_waitingTimers.Add(expirationTimer);
			}
		}

		public void Add(Line wall, double expirationTime)
		{
			Timer expirationTimer = new Timer(expirationTime);
			expirationTimer.AutoReset = false;
			expirationTimer.Elapsed += ((object sender, ElapsedEventArgs e) =>
			{
				lock (_borders)
				{
					_borders.Remove(wall);
				}
			});
			lock (_borders)
			{
				_borders.Add(wall);
				expirationTimer.Start();
			}
			lock (_waitingTimers)
			{
				_waitingTimers.Add(expirationTimer);
			}
		}

		public bool Contains(Line wall)
		{
			lock(_borders)
			{
				return _borders.Contains(wall);
			}
		}

		public bool Contains(Ball ball)
		{
			lock(_balls)
			{
				return _balls.Contains(ball);
			}
		}

		public void Dispose()
		{
			_cleaner.Dispose();
			while (_waitingTimers.Count() > 0)
			{
				_waitingTimers.ElementAt(0).Dispose();
				_waitingTimers.RemoveAt(0);
			}
		}
	}
}
