using System;
using System.Collections.Concurrent;
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
		private ConcurrentHashSet<Ball> _balls;
		private ConcurrentHashSet<Line> _borders;
		
		public ExclusionList()
		{
			disposed = false;
			_balls = new ConcurrentHashSet<Ball>();
			_borders = new ConcurrentHashSet<Line>();
		}

		public void Add(Ball ball, double expirationTime)
		{
			if (disposed) throw new ObjectDisposedException(this.ToString());
			_balls.Add(ball);

			new System.Threading.Thread(() =>
			{
				System.Threading.Thread.Sleep((int)expirationTime);
				_balls.Remove(ball);
			}).Start();
		}

		public void Add(Line wall, double expirationTime)
		{
			if (disposed) throw new ObjectDisposedException(this.ToString());
			_borders.Add(wall);

			new System.Threading.Thread(() =>
			{
				System.Threading.Thread.Sleep((int)expirationTime);
				_borders.Remove(wall);
			}).Start();
		}

		public bool Contains(Line wall)
		{
			if (disposed) throw new ObjectDisposedException(this.ToString());
			return _borders.Contains(wall);
		}

		public bool Contains(Ball ball)
		{
			if (disposed) throw new ObjectDisposedException(this.ToString());
			return _balls.Contains(ball);
		}

		public void Clear()
		{
			if (disposed) throw new ObjectDisposedException(this.ToString());
			_balls.Clear();
			_borders.Clear();
		}

		#region IDisposable
		private bool disposed;
		~ExclusionList()
		{
			Dispose(false);
		}
		protected void Dispose(bool disposing)
		{
			if(!disposed)
			{
				if(disposing)
				{
					
				}
				_balls.Clear();
				_balls = null;
				_borders.Clear();
				_borders = null;
				disposed = true;
			}
		}
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		#endregion
	}
}
