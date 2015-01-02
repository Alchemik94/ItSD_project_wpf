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
		public Ball(Ball ball): this(ball.Position,ball.Velocity,ball.Radius,ball.Mass)
		{
			_collisionExclusionList = ball._collisionExclusionList;
		}
		#endregion

		#region Collisions
		ExclusionList _collisionExclusionList;
		#region Ball collisions
		private EventHandler<Ball> BallCollisionEvent;
		public void RecalculateCollisions(IEnumerable<Ball> otherBalls)
		{
			lock (this)
			{
				foreach (var ball in otherBalls)
				{
					if (this.IsColliding(ball) && this != ball)
					{
						BallCollisionEvent(this, ball);
						#region Time of leaving the collision area calculation
						Vector distanceVector = new Vector(this.Position, ball.Position);
						//If collision was catched at almost perfect time
						//if (Vector.IsZero(distanceVector.Length() - this.Radius - ball.Radius))
						//	continue;
						//Else we have to calculate time of leaving
						var tmp = new Ball(ball);
						tmp.BallCollisionEventHandler(this, this);
						Vector relativeVelocity = (this.Velocity + tmp.Velocity).Projection(new Line(this.Position, ball.Position));
						double leavingTime = -((distanceVector.Length() - this.Radius - ball.Radius) / relativeVelocity.Length());
						if (Simulation.IsZero(leavingTime)==false && leavingTime < 1000)
							_collisionExclusionList.Add(ball, (leavingTime * (double)1000) / (double)Simulation.TicksPerSecond);
						else
							_collisionExclusionList.Add(ball, 1000 / Simulation.TicksPerSecond);
						#endregion
					}
				}
			}
		}
		public bool IsColliding(Ball other)
		{
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
			Velocity = Velocity - distanceVector * constant ;
		}
		#endregion
		#region Wall collisions
		private EventHandler<Line> WallCollisionEvent;
		public void RecalculateCollisions(IEnumerable<Line> walls)
		{
			lock (this)
			{
				foreach (var wall in walls)
				{
					if (this.IsColliding(wall))
					{
						WallCollisionEvent(this, wall);
						#region Adding to exclusion list for a short time
						//It's enough to add wall for just 1 tick, because the ball will keep its velocity relative to the wall but in the opposite direction. If there was no collision a tick ago, there will be no collision after 1 tick also.
						_collisionExclusionList.Add(wall, 1000 / Simulation.TicksPerSecond);
						#endregion
					}
				}
			}
		}
		public bool IsColliding(Line wall)
		{
			if (_collisionExclusionList.Contains(wall))
				return false;
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
			lock (this)
			{
				Velocity = Velocity + Simulation.Gravity / Simulation.TicksPerSecond;
			}
		}
		private void Displacement(object sender, EventArgs e)
		{
			lock (this)
			{
				Position = (Velocity.OriginatedAt(Position) / Simulation.TicksPerSecond).Ending;
			}
		}
		#endregion
		public void Dispose()
		{
			_collisionExclusionList.Dispose();
		}
	}
}
