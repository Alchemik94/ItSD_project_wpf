using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace ItSD_project_wpf
{
	public class Ball: IDisposable
	{
		#region Primary parameters
		public Point Position { get; set; }
		public Vector Velocity { get; set; }
		public double Radius { get; set; }
		public double Mass { get; set; }
		#endregion

		#region Equality operators
		public static bool operator!=(Ball first, Ball second)
		{
			if (ReferenceEquals(first, second)) return false;
			if (ReferenceEquals(first, null) || ReferenceEquals(second, null)) return true;
			if (Simulation.IsZero(first.Mass - second.Mass)==false)
				return true;
			if (first.Position != second.Position)
				return true;
			if (Simulation.IsZero(first.Radius - second.Radius)==false)
				return true;
			if (first.Velocity != second.Velocity)
				return true;
			return false;
		}
		public override bool Equals(object obj)
		{
			if (obj as Ball == null) return false;
			return this==(obj as Ball);
		}
		public override int GetHashCode()
		{
			int hash = 17;
			int anotherPrime = 486187739;
			//23
			hash = hash * anotherPrime + Position.GetHashCode();
			hash = hash * anotherPrime + Velocity.GetHashCode();
			hash = hash * anotherPrime + Radius.GetHashCode();
			hash = hash * anotherPrime + Mass.GetHashCode();
			return hash;
		}
		public static bool operator==(Ball first, Ball second)
		{
			return (first != second) == false;
		}
		#endregion

		#region Constructors
		private Ball()
		{
			_collisionExclusionList = new ExclusionList();
			BallCollisionEvent += BallCollisionEventHandler;
			WallCollisionEvent += WallCollisionEventHandler;
			IntervalTimeElapsed += Displacement;
			IntervalTimeElapsed += Acceleration;
		}
		public Ball(Point center, Vector velocity, double radius, double mass):this()
		{
			Position = center;
			Velocity = velocity;
			Radius = radius;
			Mass = mass;
		}
		public Ball(Ball ball): this(new Point(ball.Position),new Vector(ball.Velocity),ball.Radius,ball.Mass)
		{
			
		}
		#endregion

		#region Collisions
		private volatile ExclusionList _collisionExclusionList;
		private const double MaxLeavingTime = 1000000;
		
		#region Ball collisions
		private EventHandler<Ball> BallCollisionEvent;
		#region Total velocity calculation variables
		private Vector _partialVelocity;
		private int _numberOfCollisions;
		private object _collisionsLockobj = new object();
		#endregion
		public void RecalculateCollisions(IEnumerable<Ball> otherBalls)
		{
			if (disposed) throw new ObjectDisposedException(this.ToString());
			lock (this)
			{
				#region Sum of partial velocities calculation
				_partialVelocity = Vector.ZeroVector;
				_numberOfCollisions = 0;
				Parallel.ForEach(otherBalls, ball =>
				{
					if (this != ball && this.IsColliding(ball))
					{
						_collisionExclusionList.Clear();
					}
				});
				Parallel.ForEach(otherBalls,ball =>
				{
					if (this != ball && this.IsColliding(ball))
					{
						lock (_collisionsLockobj)
							++_numberOfCollisions;
						BallCollisionEvent(this, ball);
					}	
				});
				#endregion
				#region Change of velocity
				if (_numberOfCollisions > 0)
					Velocity = Velocity + _partialVelocity / (double)_numberOfCollisions;
				#endregion
			}
		}
		public bool IsColliding(Ball other)
		{
			if (disposed) throw new ObjectDisposedException(this.ToString());
			if (_collisionExclusionList.Contains(other))
				return false;
			if (new Vector(other.Position, this.Position).Length() <= other.Radius + this.Radius)
				return true;
			return false;
		}
		private void BallCollisionEventHandler(object sender, Ball colliding)
		{
			Vector distanceVector = new Vector(colliding.Position, this.Position);
			double constant = 2*colliding.Mass/(this.Mass+colliding.Mass);
			constant *= (this.Velocity-colliding.Velocity).DotProduct(distanceVector);
			constant /= distanceVector.DotProduct(distanceVector);
			lock(_partialVelocity)
				_partialVelocity = _partialVelocity - distanceVector * constant ;
		}
		public void AddExclusions(IEnumerable<Ball> ballsWithRecalculatedVelocities)
		{
			if (disposed) throw new ObjectDisposedException(this.ToString());
			Parallel.ForEach(ballsWithRecalculatedVelocities, ball =>
			{
				if (this != ball && this.IsColliding(ball))
				{
					#region Time of leaving the collision area calculation
					Vector distanceVector = new Vector(this.Position, ball.Position);
					Vector relativeVelocity = (this.Velocity - ball.Velocity).Projection(new Line(this.Position, ball.Position));
					double leavingTime = -((distanceVector.Length() - this.Radius - ball.Radius) / relativeVelocity.Length());
					if (Simulation.IsZero(leavingTime) == false && leavingTime < 1000)
						_collisionExclusionList.Add(ball, (leavingTime * (double)1000) / (double)Simulation.TicksPerSecond + 1);
					else if (leavingTime < 1000)
						_collisionExclusionList.Add(ball, 1000 / Simulation.TicksPerSecond);
					else
						_collisionExclusionList.Add(ball,MaxLeavingTime);
					#endregion
				}
			});
		}
		#endregion
		
		#region Wall collisions
		private EventHandler<Line> WallCollisionEvent;
		public void RecalculateCollisions(IEnumerable<Line> walls)
		{
			if (disposed) throw new ObjectDisposedException(this.ToString());
			lock (this)
			{
				foreach (var wall in walls)
				{
					if (this.IsColliding(wall))
					{
						WallCollisionEvent(this, wall);
						_collisionExclusionList.Clear();
					}
				}
			}
		}
		public bool IsColliding(Line wall)
		{
			if (disposed) throw new ObjectDisposedException(this.ToString());
			double distance = wall.Distance(this.Position);
			if(distance <= this.Radius || Simulation.IsZero(distance-this.Radius))
				return true;
			return false;
		}
		private void WallCollisionEventHandler(object sender, Line wall)
		{
			Vector incomingPartial = Velocity - Velocity.Projection(wall);
			Velocity = Velocity + incomingPartial * (-2);
		}
		#endregion
		
		#endregion

		#region Moving
		public ElapsedEventHandler IntervalTimeElapsed;
		private void Acceleration(object sender, EventArgs e)
		{
			if (disposed) throw new ObjectDisposedException(this.ToString());
			lock (this)
			{
				Velocity = Velocity + Simulation.Gravity / Simulation.TicksPerSecond;
			}
		}
		private void Displacement(object sender, EventArgs e)
		{
			if (disposed) throw new ObjectDisposedException(this.ToString());
			lock (this)
			{
				Position = (Velocity.OriginatedAt(Position) / Simulation.TicksPerSecond).Ending;
			}
		}
		#endregion

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
					if (_collisionExclusionList != null)
						_collisionExclusionList.Dispose();
				disposed = true;
			}
		}
		~Ball()
		{
			Dispose(false);
		}
		#endregion
	}
}
