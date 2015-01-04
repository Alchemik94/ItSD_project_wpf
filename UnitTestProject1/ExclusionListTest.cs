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
			ExclusionList list = new ExclusionList();
			Assert.IsFalse(list.Contains(new Ball(Point.Origin,Vector.ZeroVector,1,1)));
		}
		[TestMethod]
		public void ball_expiry_test()
		{
			ExclusionList list = new ExclusionList();
			list.Add(new Ball(Point.Origin, Vector.ZeroVector, 1, 1), 1);
			Assert.IsTrue(list.Contains(new Ball(Point.Origin, Vector.ZeroVector, 1, 1)));
			System.Threading.Thread.Sleep(1);
			Assert.IsFalse(list.Contains(new Ball(Point.Origin, Vector.ZeroVector, 1, 1)));
		}
	}
}
