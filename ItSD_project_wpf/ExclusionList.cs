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
		private volatile ConcurrentHashSet<Ball> _balls;
		
		public ExclusionList()
		{
			disposed = false;
			_balls = new ConcurrentHashSet<Ball>();
		}

		public void Add(Ball ball, double expirationTime)
		{
			if (disposed) throw new ObjectDisposedException(this.ToString());
			_balls.Add(ball);

			new System.Threading.Thread(() =>
			{
				System.Threading.Thread.Sleep((int)expirationTime);
				if(!disposed)
					_balls.Remove(ball);
			}).Start();
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
		}

		#region Dispose
		private bool disposed;
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
					if (_balls != null)
						_balls.Dispose();
				disposed = true;
			}
		}
		~ExclusionList()
		{
			Dispose(false);
		}
		#endregion
	}
}
