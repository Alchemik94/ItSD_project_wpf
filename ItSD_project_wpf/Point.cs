using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItSD_project_wpf
{
	public class Point
	{
		#region Coordinates
		public double X { get; set; }
		public double Y { get; set; }
		#endregion

		#region Constructors
		public Point()
		{
			X = Y = 0;
		}
		public Point(double x, double y)
		{
			X = x;
			Y = y;
		}
		public Point(Point pt):this(pt.X,pt.Y)
		{

		}
		#endregion

		#region Simple point operations
		public static Point operator+(Point first, Point second)
		{
			return new Point(second.X + first.X, second.Y + first.Y);
		}
		public static Point operator-(Point first,Point second)
		{
			return new Point(first.X - second.X, first.Y - second.Y);
		}
		public static Point operator*(Point pt, double number)
		{
			return new Point(pt.X*number,pt.Y*number);
		}
		public static bool operator!=(Point first, Point second)
		{
			if (ReferenceEquals(first, second)) return false;
			if (ReferenceEquals(first, null) || ReferenceEquals(second, null)) return true;
			if (Simulation.IsZero(first.X - second.X)==false || Simulation.IsZero(first.Y - second.Y)==false)
				return true;
			return false;
		}
		public static bool operator==(Point first, Point second)
		{
			return (first != second) == false;
		}
		public override bool Equals(object obj)
		{
			if (obj as Point == null) return false;
			return this == (obj as Point);
		}
		public override int GetHashCode()
		{
			int hash = 17;
			int anotherPrime = 486187739;
			//23
			hash = hash * anotherPrime + X.GetHashCode();
			hash = hash * anotherPrime + Y.GetHashCode();
			return hash;
		}
		#endregion

		#region Origin
		private static Point _origin;
		private static object _lockObj = new object();
		public static Point Origin
		{
			get
			{
				if (_origin == null)
					lock (_lockObj)
					{
						if(_origin == null)
							_origin = new Point(0, 0);
					}
				return new Point(_origin);
			}
			private set
			{
				_origin = value;
			}
		}
		#endregion
	}
}
