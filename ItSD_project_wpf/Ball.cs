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
		private Point _position;
		public Point Position
		{
			get
			{
				return _position;
			}
			set
			{
				if(double.IsNaN(value.X) || double.IsNaN(value.Y))
					throw new ArgumentException();
				_position = value;
			}
		}
		private Vector _velocity;
		public Vector Velocity
		{
			get
			{
				return _velocity;
			}
			set
			{
				if (double.IsNaN(value.Beginning.X) || double.IsNaN(value.Beginning.Y) || double.IsNaN(value.Ending.X) || double.IsNaN(value.Ending.Y))
					throw new ArgumentException();
				_velocity = value;
			}
		}
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
			//if (first.Velocity != second.Velocity)
			//	return true;
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
			PartialVelocity = Vector.ZeroVector;
			CorrectiveVelocity = Vector.ZeroVector;
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
		private const double MaxLeavingTime = 100;
		
		#region Ball collisions
		private EventHandler<Ball> BallCollisionEvent;
		#region Total velocity calculation variables
		#region Partial velocity
		private Vector _partialVelocity;
		private Vector PartialVelocity
		{
			get
			{
				return _partialVelocity;
			}
			set
			{
				if (double.IsNaN(value.Beginning.X) || double.IsNaN(value.Beginning.Y) || double.IsNaN(value.Ending.X) || double.IsNaN(value.Ending.Y))
					throw new ArgumentException();
				_partialVelocity = value;
			}
		}
		#endregion
		private int _numberOfCollisions;
		private object _collisionsLockobj = new object();
		#endregion
		public void RecalculateCollisions(IEnumerable<Ball> otherBalls)
		{
			if (disposed) throw new ObjectDisposedException(this.ToString());
			lock (this)
			{
				#region Sum of partial velocities calculation
				lock(PartialVelocity)
					PartialVelocity = Vector.ZeroVector;
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
					lock(PartialVelocity)
						lock(Velocity)
							Velocity = Velocity + PartialVelocity / (double)_numberOfCollisions;
				#endregion
			}
		}
		public bool IsColliding(Ball other)
		{
			if (disposed) throw new ObjectDisposedException(this.ToString());
			if (_collisionExclusionList.Contains(other))
				return false;
			if (this.Velocity == Vector.ZeroVector && other.Velocity == Vector.ZeroVector)
				return false;
			if (new Vector(other.Position, this.Position).Length <= other.Radius + this.Radius)
				return true;
			return false;
		}
		private void BallCollisionEventHandler(object sender, Ball colliding)
		{
			lock(Velocity)
				lock (Position)
				{
					Vector distanceVector = new Vector(colliding.Position, this.Position);
					double c1 = 2 * colliding.Mass / (this.Mass + colliding.Mass);
					double c2 = (this.Velocity - colliding.Velocity).DotProduct(distanceVector);
					double c3 = distanceVector.DotProduct(distanceVector);
					lock (PartialVelocity)
						PartialVelocity = PartialVelocity - distanceVector * (c1 * c2 / c3);
					//if(Simulation.IsZero(distanceVector.Length-this.Radius-colliding.Radius)==false)
					//{
					//	Position = (distanceVector.Normalized()*this.Radius).OriginatedAt((distanceVector/2).Ending).Ending;
					//}
				}
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
					double leavingTime = -((distanceVector.Length - this.Radius - ball.Radius) / relativeVelocity.Length);
					if (Simulation.IsZero(leavingTime) == false && leavingTime <= Simulation.TicksPerSecond)
						_collisionExclusionList.Add(ball, (leavingTime * (double)1000) / (double)Simulation.TicksPerSecond + 1);
					else if (leavingTime <= Simulation.TicksPerSecond)
						_collisionExclusionList.Add(ball, 1000 / Simulation.TicksPerSecond);
					else
						RepairVelocity(ball);
					#endregion
				}
			});
		}
		private Vector CorrectiveVelocity;
		private void RepairVelocity(Ball other)
		{
			lock (this)
			{
				Vector distVec;
				lock(Position)
					distVec = new Vector(other.Position, this.Position);
				if (Simulation.IsZero(distVec.Length - this.Radius - other.Radius) == false)
				{
					var correction = distVec.Normalized() * ((other.Mass)/(this.Mass+other.Mass)) * (this.Radius + other.Radius - distVec.Length) * (double)Simulation.TicksPerSecond;
					lock (CorrectiveVelocity)
						CorrectiveVelocity = CorrectiveVelocity + correction;
				}
			}
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
						_collisionExclusionList.Clear();
						WallCollisionEvent(this, wall);
					}
				}
			}
		}
		public bool IsColliding(Line wall)
		{
			if (disposed) throw new ObjectDisposedException(this.ToString());
			if (_collisionExclusionList.Contains(wall))
				return false;
			double distance = wall.Distance(this.Position);
			if(distance <= this.Radius || Simulation.IsZero(distance-this.Radius))
				return true;
			return false;
		}
		private void WallCollisionEventHandler(object sender, Line wall)
		{
			lock (Velocity)
			{
				Vector incomingPartial = Velocity - Velocity.Projection(wall);
				Velocity = Velocity + incomingPartial * (-2+Simulation.WallCollisionLooseFactor);
			}
			lock (Position)
			{
				if (Simulation.IsZero(wall.Distance(this.Position) - this.Radius) == false)
				{
					Vector tmp = new Vector(wall.First, this.Position);
					Vector proj = tmp.Projection(wall);
					Vector dist = new Vector(proj.Ending, tmp.Ending);
					Position = (dist.Normalized() * (this.Radius)).OriginatedAt(dist.Beginning).Ending;
				}
			}
			//_collisionExclusionList.Add(wall, (1 / (1 - Simulation.WallCollisionLooseFactor)) * 1000 / Simulation.TicksPerSecond);
		}
		#endregion
		
		#endregion

		#region Moving
		public ElapsedEventHandler IntervalTimeElapsed;
		private void Acceleration(object sender, EventArgs e)
		{
			if (disposed) throw new ObjectDisposedException(this.ToString());
			lock (this)
				lock (Velocity)
					Velocity = Velocity + Simulation.Gravity / Simulation.TicksPerSecond;
		}
		private void Displacement(object sender, EventArgs e)
		{
			if (disposed) throw new ObjectDisposedException(this.ToString());
			lock (this)
				lock (Velocity)
					lock(Position)
						lock (CorrectiveVelocity)
						{
							Position = ((Velocity + CorrectiveVelocity).OriginatedAt(Position) / Simulation.TicksPerSecond).Ending;
							CorrectiveVelocity = Vector.ZeroVector;
						}
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
