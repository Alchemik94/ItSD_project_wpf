using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItSD_project_wpf
{
	public class Line
	{
		#region Primary properties
		public Point First { get; set; }
		public Point Second { get; set; }
		public double A
		{
			get
			{
				if (First.X == Second.X)
					return -1;
				else if (First.Y == Second.Y)
					return 0;
				else
					return (First.Y - Second.Y) / (First.X - Second.X);
			}
			private set { }
		}
		public double B
		{
			get
			{
				if (First.X == Second.X)
					return 0;
				else if (First.Y == Second.Y)
					return -1;
				else
					return -1;
			}
			private set { }
		}
		public double C
		{
			get
			{
				if (First.X == Second.X)
					return First.X;
				else if (First.Y == Second.Y)
					return First.Y;
				else
					return First.Y - First.X * A;
			}
			private set { }
		}
		public double Distance(Point point)
		{
			return Math.Abs(A * point.X + B * point.Y + C) / Math.Sqrt(Math.Pow(A, 2) + Math.Pow(B, 2));
		}
		#endregion

		#region Constructors
		public Line(Point first, Point second)
		{
			First = first;
			Second = second;
		}
		public Line(Line line): this(new Point(line.First), new Point(line.Second))
		{

		}
		#endregion
	}
}
