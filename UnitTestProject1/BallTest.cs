using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ItSD_project_wpf;
using System.Collections.Generic;
using System.Linq;

namespace ItSD_project_wpf_test
{
	[TestClass]
	public class BallTest
	{
		[TestMethod]
		public void ball_copy_test()
		{
			Ball first = new Ball(Point.Origin, Vector.VersorX, 1, 1);
			Assert.AreNotSame(first.Velocity, new Ball(first).Velocity);
			Assert.AreNotSame(first.Velocity.Ending, new Ball(first).Velocity.Ending);
			Assert.AreNotSame(first.Velocity.Ending.X, new Ball(first).Velocity.Ending.X);
			Assert.AreNotSame(first.Velocity.Ending.Y, new Ball(first).Velocity.Ending.Y);
		}

		[TestMethod]
		public void ball_collision_test()
		{
			Ball first = new Ball(new Point(250,250),new Vector(Point.Origin,new Point(5,-10)),25,1);
			Ball second = new Ball(new Point(250, 200), new Vector(Point.Origin, new Point(-5, 10)), 25, 1);
			var collection = new List<Ball>();
			collection.Add(first);
			collection.Add(second);
			List<Ball> tmp = (from balls in collection select new Ball(balls)).ToList();
			first.RecalculateCollisions(tmp);
			second.RecalculateCollisions(tmp);
			Assert.AreEqual(new Vector(Point.Origin, new Point(5, 10)).Ending.X, first.Velocity.Ending.X, "First velocity X fails");
			Assert.AreEqual(new Vector(Point.Origin, new Point(5, 10)).Ending.Y, first.Velocity.Ending.Y, "First velocity Y fails");
			Assert.AreEqual(new Vector(Point.Origin, new Point(5, 10)), first.Velocity, "First velocity fails");

			Assert.AreEqual(new Vector(Point.Origin, new Point(-5, -10)).Ending.X, second.Velocity.Ending.X, "Second velocity X fails");
			Assert.AreEqual(new Vector(Point.Origin, new Point(-5, -10)).Ending.Y, second.Velocity.Ending.Y, "Second velocity Y fails");
			Assert.AreEqual(new Vector(Point.Origin, new Point(-5, -10)), second.Velocity, "Second velocity fails");

			//first = new Ball();
			//second = new Ball();
		}
	}
}
