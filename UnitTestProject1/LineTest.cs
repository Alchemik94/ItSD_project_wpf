using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ItSD_project_wpf;

namespace ItSD_project_wpf_test
{
	[TestClass]
	public class LineTest
	{
		private const double EPS = 0.00001;
		[TestMethod]
		public void distance_test()
		{
			Line line = new Line(Point.Origin, new Point(0, 1));
			Assert.AreEqual(line.Distance(line.First), 0, EPS);
			Assert.AreEqual(line.Distance(line.Second), 0, EPS);
			Assert.AreEqual(line.Distance(new Point(1, 0)), 1, EPS);
			Assert.AreEqual(line.Distance(new Point(-1, 0)), 1, EPS);
		}
	}
}
