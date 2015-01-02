using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ItSD_project_wpf;

namespace ItSD_project_wpf_test
{
	[TestClass]
	public class PointTest
	{
		private const double EPS = 0.000001;
		[TestMethod]
		public void point_initialization_test()
		{
			double x = 0, y = 0;
			Point point = new Point(x, y);
			Assert.AreEqual(x, point.X,EPS);
			Assert.AreEqual(y, point.Y,EPS);
			x = 3;
			y = 2;
			Assert.AreEqual(0, point.X, EPS);
			Assert.AreEqual(0, point.Y, EPS);
			point = new Point(x, y);
			Assert.AreEqual(x, point.X, EPS);
			Assert.AreEqual(y, point.Y, EPS);
			point = new Point();
			Assert.AreEqual(0, point.X, EPS);
			Assert.AreEqual(0, point.Y, EPS);
			Point another = new Point(point);
			Assert.AreEqual(another.X, point.X);
			Assert.AreEqual(another.Y, point.Y);
			Assert.AreNotSame(point, another);
		}

		[TestMethod]
		public void point_operations_test()
		{
			Point a = new Point(0, 0);
			Point b = new Point(1, 2);
			Point c = a + b;
			Point d = a - b;
			Point e = a * 2;
			Point f = b * 2;
			Point g = new Point(1, 2);
			Assert.AreEqual(c.X, 1, EPS);
			Assert.AreEqual(c.Y, 2, EPS);
			Assert.AreEqual(d.X, -1, EPS);
			Assert.AreEqual(d.Y, -2, EPS);
			Assert.AreEqual(e.X, 0, EPS);
			Assert.AreEqual(e.Y, 0, EPS);
			Assert.AreEqual(f.X, 2, EPS);
			Assert.AreEqual(f.Y, 4, EPS);
			Assert.AreEqual(b, g);
			Assert.AreEqual(b.GetHashCode(), g.GetHashCode());
		}

		[TestMethod]
		public void origin_point_test()
		{
			Point point = new Point(0, 0);
			Assert.AreEqual(point, Point.Origin);
		}
	}
}
