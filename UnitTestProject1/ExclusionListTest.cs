using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ItSD_project_wpf;

namespace ItSD_project_wpf_test
{
	[TestClass]
	public class ExclusionListTest
	{
		[TestMethod]
		public void initialization_test()
		{
			using(ExclusionList list = new ExclusionList())
			{
				Assert.IsFalse(list.Contains(new Ball(Point.Origin,Vector.ZeroVector,1,1)));
				Assert.IsFalse(list.Contains(new Line(Point.Origin, new Point(0, 1))));
			}
		}
		[TestMethod]
		public void ball_expiry_test()
		{
			using (ExclusionList list = new ExclusionList())
			{
				list.Add(new Ball(Point.Origin, Vector.ZeroVector, 1, 1), 1);
				Assert.IsTrue(list.Contains(new Ball(Point.Origin, Vector.ZeroVector, 1, 1)));
				System.Threading.Thread.Sleep(1);
				Assert.IsFalse(list.Contains(new Ball(Point.Origin, Vector.ZeroVector, 1, 1)));
			}
		}
		[TestMethod]
		public void wall_expiry_test()
		{
			using (ExclusionList list = new ExclusionList())
			{
				list.Add(new Line(Point.Origin, new Point(1,0)), 1);
				Assert.IsTrue(list.Contains(new Line(Point.Origin, new Point(1,0))));
				System.Threading.Thread.Sleep(1);
				Assert.IsFalse(list.Contains(new Line(Point.Origin, new Point(1,0))));
			}
		}
	}
}
