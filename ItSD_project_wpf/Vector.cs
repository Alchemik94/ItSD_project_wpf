using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItSD_project_wpf
{
	public class Vector
	{
		#region Points bordering the vector
		public Point Beginning {get;set;}
		public Point Ending {get;set;}
		#endregion

		#region Partial vectors
		public Vector X
		{
			get
			{
				var tmp = new Vector(this);
				tmp.Y = new Vector(ZeroVector);
				return tmp;
			}
			set
			{
				this.Ending.X = value.OriginatedAt(this.Beginning).Ending.X;
			}
		}
		public Vector Y
		{
			get
			{
				var tmp = new Vector(this);
				tmp.X = new Vector(ZeroVector);
				return tmp;
			}
			set
			{
				this.Ending.Y = value.OriginatedAt(this.Beginning).Ending.Y;
			}
		}
		#endregion

		#region Constructors
		public Vector(Point beginning, Point ending)
		{
			Beginning = new Point(beginning);
			Ending = new Point(ending);
		}
		public Vector(double xBeg, double yBeg, double xEnd, double yEnd)
		{
			Beginning = new Point(xBeg, yBeg);
			Ending = new Point(xEnd, yEnd);
		}
		public Vector(Vector vector):this(new Point(vector.Beginning),new Point(vector.Ending))
		{

		}
		#endregion

		#region Vector standard properties
		public static Vector operator*(Vector vector,double number)
		{
			var originated = vector.OriginatedAt(Point.Origin);
			originated.Ending = originated.Ending * number;
			return originated.OriginatedAt(vector.Beginning);
		}
		public static Vector operator/(Vector vector, double number)
		{
			return vector * (1 / number);
		}
		public static Vector operator+(Vector first, Vector second)
		{
			var firstAtOrigin = first.OriginatedAt(Point.Origin);
			var secondAtOrigin = second.OriginatedAt(Point.Origin);
			var tmp = new Vector(Point.Origin,new Point(firstAtOrigin.Ending.X+secondAtOrigin.Ending.X, firstAtOrigin.Ending.Y+secondAtOrigin.Ending.Y));
			return tmp.OriginatedAt(first.Beginning);
		}
		public static Vector operator-(Vector first, Vector second)
		{
			return first+second*(double)(-1);
		}
		public static bool operator!=(Vector first,Vector second)
		{
			if (ReferenceEquals(first, second)) return false;
			if (ReferenceEquals(first, null) || ReferenceEquals(second, null)) return true;
			var firstOriginated = first.OriginatedAt(Point.Origin);
			var secondOriginated = second.OriginatedAt(Point.Origin);
			if (firstOriginated.Ending != secondOriginated.Ending)
				return true;
			return false;
		}
		public static bool operator==(Vector first,Vector second)
		{
			return (first != second) == false;
		}
		public override bool Equals(object obj)
		{
			if (obj as Vector == null) return false;
			return this == (obj as Vector);
		}
		public override int GetHashCode()
		{
			int hash = 17;
			int anotherPrime = 486187739;
			//23
			hash = hash * anotherPrime + OriginatedAt(Point.Origin).Beginning.GetHashCode();
			hash = hash * anotherPrime + OriginatedAt(Point.Origin).Ending.GetHashCode();
			return hash;
		}
		public double DotProduct(Vector another)
		{
			var thisAtOrigin = this.OriginatedAt(Point.Origin);
			var anotherAtOrigin = another.OriginatedAt(Point.Origin);
			return thisAtOrigin.Ending.X * anotherAtOrigin.Ending.X + thisAtOrigin.Ending.Y * anotherAtOrigin.Ending.Y;
		}
		public double CrossProductLength(Vector another)
		{
			var thisAtOrigin = this.OriginatedAt(Point.Origin);
			var anotherAtOrigin = another.OriginatedAt(Point.Origin);
			return thisAtOrigin.Ending.X * anotherAtOrigin.Ending.Y - thisAtOrigin.Ending.Y * anotherAtOrigin.Ending.X;
		}
		public double Length
		{
			get
			{
				var tmp = this.OriginatedAt(Point.Origin);
				return Math.Sqrt(Math.Pow(tmp.Ending.X, 2) + Math.Pow(tmp.Ending.Y, 2));
			}
			private set { }
		}
		public Vector Normalized()
		{
			if (Simulation.IsZero(Length))
				throw new InvalidOperationException("Cannot normalize zero-length vector.");
			return this * ( ((double)(1))/this.Length );
		}
		public Vector Projection(Line line)
		{
			var originatedOnLine = this.OriginatedAt(line.First);
			double distanceFromEndToLine = line.Distance(originatedOnLine.Ending);
			Vector projection = new Vector(line.First, line.Second);
			projection = projection.Normalized();
			double projectionLength = Math.Sqrt(Math.Pow(originatedOnLine.Length, 2) - Math.Pow(distanceFromEndToLine, 2));
			projection *= projectionLength;
			projection = projection.OriginatedAt(line.First);
			if (new Vector(projection.Ending, originatedOnLine.Ending).Length < new Vector((projection * (-1)).Ending, originatedOnLine.Ending).Length)
				return projection.OriginatedAt(this.Beginning);
			else return (projection * (-1)).OriginatedAt(this.Beginning);
		}
		//Changes beginning point of the vector keeping it's partials of appropriate length
		public Vector OriginatedAt(Point origin)
		{
			return new Vector(origin, new Point(Ending.X - Beginning.X + origin.X, Ending.Y - Beginning.Y + origin.Y));
		}
		#endregion

		#region X and Y axes versors
		private static object _lockObj = new object();
		private static Vector _versorX;
		public static Vector VersorX
		{
			get
			{
				if (_versorX == null)
					lock (_lockObj)
					{
						if (_versorX == null)
							_versorX = new Vector(Point.Origin, new Point(Point.Origin.X + 1, Point.Origin.Y));
					}
				return new Vector(_versorX);
			}
			private set
			{
				_versorX = value;
			}
		}
		
		private static Vector _versorY;
		public static Vector VersorY
		{
			get
			{
				if (_versorY == null)
					lock (_lockObj)
					{
						if (_versorY == null)
							_versorY = new Vector(Point.Origin, new Point(Point.Origin.X, Point.Origin.Y + 1));
					}
				return new Vector(_versorY);
			}
			private set
			{
				_versorY = value;
			}
		}
		#endregion
		#region Zero vector
		private static Vector _zeroVector;
		public static Vector ZeroVector
		{
			get
			{
				if (_zeroVector == null)
					lock (_lockObj)
					{
						if (_zeroVector == null)
							_zeroVector = new Vector(Point.Origin, Point.Origin);
					}
				return new Vector(_zeroVector);
			}
			private set
			{
				_zeroVector = value;
			}
		}
		#endregion
	}
}
