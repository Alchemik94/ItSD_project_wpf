using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ItSD_project_wpf;

namespace ItSD_project_wpf_test
{
	[TestClass]
	public class VectorTest
	{
		private const double EPS = 0.00001;
		#region Constructors tests
		[TestMethod]
		public void points_vector_initialization_test()
		{
			#region Points initialization
			Point first = new Point();
			Point second = new Point(1, 2);
			Point third = new Point(4, 3);
			#endregion
			#region Vectors initializations
			Vector vector = new Vector(first, second);
			Assert.AreEqual(vector.Beginning, first);
			Assert.AreEqual(vector.Ending, second);
			Assert.AreNotSame(vector.Ending, second);
			Assert.AreNotSame(vector.Beginning, first);

			vector = new Vector(second, third);
			Assert.AreEqual(vector.Beginning.X, second.X, EPS);
			Assert.AreEqual(vector.Beginning.Y, second.Y, EPS);
			Assert.AreEqual(vector.Ending.X, third.X, EPS);
			Assert.AreEqual(vector.Ending.Y, third.Y, EPS);
			#endregion
		}
		[TestMethod]
		public void doubles_vector_initialization_test()
		{
			#region Points initialization
			Point first = new Point();
			Point second = new Point(1, 2);
			#endregion
			#region Double initialization
			Vector vector = new Vector(0, 0, 1, 2);
			Assert.AreEqual(vector.Beginning.X, 0, EPS);
			Assert.AreEqual(vector.Beginning.Y, 0, EPS);
			Assert.AreEqual(vector.Ending.X, 1, EPS);
			Assert.AreEqual(vector.Ending.Y, 2, EPS);
			Assert.AreEqual(vector.Beginning, first);
			Assert.AreEqual(vector.Ending, second);
			#endregion
		}
		[TestMethod]
		public void vector_vector_initialization_test()
		{
			#region Points initialization
			Point second = new Point(1, 2);
			Point third = new Point(4, 3);
			#endregion
			Vector vector = new Vector(second, third);
			#region Initialization from another vector
			Vector another = new Vector(vector);
			Assert.AreEqual(another.Beginning, vector.Beginning);
			Assert.AreEqual(another.Ending, vector.Ending);
			Assert.AreNotSame(another, vector);
			Assert.AreNotSame(another.Ending, vector.Ending);
			Assert.AreNotSame(another.Beginning, vector.Beginning);
			#endregion
			vector = Vector.ZeroVector;
			vector = vector - new Vector(Point.Origin,new Point(0,-50)) * (-0.4);
			Assert.AreEqual(-20, vector.Ending.Y);
			Assert.AreEqual(0, vector.Ending.X);
		}
		#endregion
		#region Basic vector operations tests
		[TestMethod]
		public void constant_multiplication_test()
		{
			Vector vector = new Vector(Point.Origin, new Point(1, 2));
			Vector multiplied = vector * 2;
			Assert.AreEqual(vector.Length() * 2, multiplied.Length(), EPS);
			Assert.AreEqual(vector.Beginning, multiplied.Beginning);
			Assert.AreEqual(vector.Ending * 2, multiplied.Ending);
			
			vector = new Vector(new Point(0, 1), new Point(1, 1));
			multiplied = vector * 2;
			Assert.AreEqual(vector.Length() * 2, multiplied.Length(), EPS);
			Assert.AreEqual(vector.Beginning, multiplied.Beginning);

			vector = new Vector(new Point(2, 1), new Point(1, 2));
			multiplied = vector * 2;
			Assert.AreEqual(vector.Length() * 2, multiplied.Length(), EPS);
			Assert.AreEqual(multiplied.Ending, new Point(0,3));
			Assert.AreEqual(vector.Beginning, multiplied.Beginning);
		}
		[TestMethod]
		public void constant_division_test()
		{
			Vector vector = new Vector(Point.Origin, new Point(1, 2));
			Vector multiplied = vector / (0.5);
			Assert.AreEqual(vector.Length() * 2, multiplied.Length(), EPS);
			Assert.AreEqual(vector.Beginning, multiplied.Beginning);
			Assert.AreEqual(vector.Ending * 2, multiplied.Ending);

			vector = new Vector(new Point(0, 1), new Point(1, 1));
			multiplied = vector / (0.5);
			Assert.AreEqual(vector.Length() * 2, multiplied.Length(), EPS);
			Assert.AreEqual(vector.Beginning, multiplied.Beginning);
		}
		[TestMethod]
		public void vector_addition_test()
		{
			Vector first = new Vector(Point.Origin, new Point(1, 1));
			Vector second = new Vector(Point.Origin, new Point(1, 2));
			Vector third = new Vector(Point.Origin, new Point(2, 3));
			Vector sum = first + second;
			Assert.AreEqual(sum, third);
			Assert.AreNotSame(sum, third);
			first = new Vector(new Point(1, 1), new Point(2, 2));
			second = new Vector(new Point(4, 3), new Point(5, 5));
			sum = first + second;
			Assert.AreEqual(sum, third);
		}
		[TestMethod]
		public void vector_subtraction_test()
		{
			Vector first = new Vector(Point.Origin, new Point(1, 1));
			Vector second = new Vector(Point.Origin, new Point(1, 2));
			Vector third = new Vector(Point.Origin, new Point(2, 3));
			Vector subtraction = third - first;
			Assert.AreNotSame(subtraction, second);
			first = new Vector(new Point(1, 1), new Point(2, 2));
			second = new Vector(new Point(4, 3), new Point(5, 5));
			subtraction = third - second;
			Assert.AreEqual(subtraction, first);
			first = new Vector(Point.Origin, new Point(0, -2));
			second = new Vector(Point.Origin, new Point(0, -3));
			subtraction = second - first;
			Assert.AreEqual(subtraction,new Vector(Point.Origin,new Point(0,-1)));
			first = new Vector(Point.Origin, new Point(0, -229));
			second = new Vector(Point.Origin, new Point(0, -250));
			subtraction = second - first;
			Assert.AreEqual(subtraction, new Vector(Point.Origin, new Point(0, -21)),"Subtraction of two negative vectors");
			first = new Vector(Point.Origin, new Point(-66, 30));
			second = first.Projection(new Line(Vector.VersorY.Beginning, Vector.VersorY.Ending));
			Assert.AreEqual(first - second, new Vector(Point.Origin,new Point(-66,0)));
		}
		[TestMethod]
		public void equality_test()
		{
			Vector vector = new Vector(Point.Origin, new Point(1, 2));
			Vector theSame = new Vector(new Point(4, 3), new Point(5, 5));
			Assert.IsTrue(vector == theSame);
			Vector another = new Vector(new Point(4, 3), new Point(3, 1));
			Assert.IsFalse(vector == another);
		}
		[TestMethod]
		public void inequality_test()
		{
			Vector vector = new Vector(Point.Origin, new Point(1, 2));
			Vector theSame = new Vector(new Point(4, 3), new Point(5, 5));
			Assert.IsFalse(vector != theSame);
			Vector another = new Vector(new Point(4, 3), new Point(3, 1));
			Assert.IsTrue(vector != another);
		}
		[TestMethod]
		public void dot_product_test()
		{
			Vector first = new Vector(Point.Origin, new Point(1, 2));
			Vector second = new Vector(Point.Origin, new Point(-1, -3));
			Assert.AreEqual(first.DotProduct(second), second.DotProduct(first));
			Assert.AreEqual(first.DotProduct(second),-7,EPS);
			first = new Vector(Point.Origin, new Point(5, -10));
			second = first * (-1);
			Assert.AreEqual(-1000, (first - second).DotProduct(new Vector(Point.Origin, new Point(0, 50))));
			Assert.AreEqual(2500, (new Vector(Point.Origin, new Point(0, 50))).DotProduct(new Vector(Point.Origin, new Point(0, 50))));
		}
		[TestMethod]
		public void cross_product_test()
		{
			Vector first = new Vector(Point.Origin, new Point(1, 2));
			Vector second = new Vector(Point.Origin, new Point(-1, -3));
			Assert.AreEqual(first.CrossProductLength(second), -1, EPS);
			Assert.AreEqual(second.CrossProductLength(first), 1, EPS);
			Assert.AreEqual(first.CrossProductLength(second), -second.CrossProductLength(first));
		}
		[TestMethod]
		public void normalization_test()
		{
			Vector vector = new Vector(Point.Origin, new Point(1, 1));
			Vector normalized = vector.Normalized();
			Assert.AreEqual(vector, normalized * Math.Sqrt(2));
			vector = vector * (-1);
			normalized = vector.Normalized();
			Assert.AreEqual(vector, normalized * Math.Sqrt(2));
		}
		[TestMethod]
		public void origin_change_vector_test()
		{
			Vector vector = new Vector(0, 0, 1, 2);
			Vector originated = vector.OriginatedAt(new Point(1, 1));
			Assert.AreEqual(vector, originated);
			Assert.AreNotSame(vector, originated);
			Assert.AreNotEqual(vector.Beginning, originated.Beginning);
			Assert.AreNotEqual(vector.Ending, originated.Ending);
			Assert.AreEqual(originated.Ending, new Point(2, 3));
			Assert.AreEqual(originated.Beginning, new Point(1, 1));
		}
		[TestMethod]
		public void partial_vectors_test()
		{
			Point first = Point.Origin;
			Point second = new Point(1, 3);
			Vector vector = new Vector(first, second);
			Vector xPartial = new Vector(first, new Point(second.X, 0));
			Vector yPartial = new Vector(first, new Point(0, second.Y));
			Assert.AreEqual(vector.X, xPartial);
			Assert.AreEqual(vector.Y, yPartial);
			Assert.AreNotSame(vector.X, xPartial);
			Assert.AreNotSame(vector.Y, yPartial);
		}
		[TestMethod]
		public void projection_on_line_test()
		{
			Vector y = new Vector(Point.Origin, new Point(0, 1));
			Vector x = new Vector(Point.Origin, new Point(1, 0));
			Vector any = new Vector(Point.Origin, new Point(3, 4));
			Line ox = new Line(Point.Origin, new Point(1,0));
			Line oy = new Line(Point.Origin, new Point(0, 1));
			Line xy = new Line(Point.Origin, new Point(1, 1));
			Assert.AreEqual(x.Projection(ox), x);
			Assert.AreEqual(y.Projection(oy), y);
			Assert.AreEqual(x.Projection(oy), Vector.ZeroVector);
			Assert.AreEqual(y.Projection(ox), Vector.ZeroVector);
			Assert.AreEqual(any.Projection(ox), new Vector(Point.Origin, new Point(any.Ending.X, 0)));
			Assert.AreEqual(any.Projection(oy), new Vector(Point.Origin, new Point(0, any.Ending.Y)));
			Assert.AreEqual(x.Projection(xy), new Vector(Point.Origin, new Point(1, 1)).Normalized() * Math.Sqrt(0.5));
			Assert.AreEqual(y.Projection(xy), new Vector(Point.Origin, new Point(1, 1)).Normalized() * Math.Sqrt(0.5));
			Vector v = new Vector(Point.Origin, new Point(205,-1));
			Assert.AreEqual(v.Projection(oy), Vector.VersorY * (-1));
			v = new Vector(Point.Origin, new Point(-66, 30));
			Assert.AreEqual(v.Projection(oy), Vector.VersorY * (30));
			
			Vector first = new Vector(Point.Origin, new Point(-66, 30));
			Vector second = first.Projection(new Line(new Point(0,500), new Point(0,250)));
			Assert.AreEqual(second, new Vector(Point.Origin, new Point(0,30)));
		}
		[TestMethod]
		public void zero_vector_test()
		{
			Assert.AreEqual(Vector.ZeroVector.Length(), 0, EPS);
			Assert.AreEqual(Vector.ZeroVector.Beginning, Vector.ZeroVector.Ending);
			Assert.AreEqual(Vector.ZeroVector.CrossProductLength(new Vector(0, 0, 1, 3)), 0, EPS);
			Assert.AreEqual(Vector.ZeroVector.DotProduct(new Vector(0, 0, 1, 3)), 0, EPS);
			Vector any = new Vector(Point.Origin, new Point(-5, -8));
			Assert.AreEqual(any, any + Vector.ZeroVector);
		}
		[TestMethod]
		public void versors_test()
		{
			Vector versorX = new Vector(Point.Origin, new Point(1, 0));
			Vector versorY = new Vector(Point.Origin, new Point(0, 1));
			Assert.AreEqual(versorX, Vector.VersorX);
			Assert.AreEqual(versorY, Vector.VersorY);
			Assert.AreEqual(Vector.VersorX.DotProduct(Vector.VersorY), 0, EPS);
		}
		#endregion
	}
}
