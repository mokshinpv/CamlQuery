using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mokshin.CamlQuery;

namespace CamlQueryUnitTest
{
	[TestClass]
	public class CamlLogicalTest
	{
		[TestMethod]
		public void TestAndZeroConditions()
		{
			Assert.AreEqual(string.Empty, CamlQuery.And().ToString());
		}

		[TestMethod]
		public void TestOrOneCondition()
		{
			var condition = CamlQuery.Eq("F", true);
			Assert.AreEqual(condition.ToString(), CamlQuery.Or(condition).ToString());
		}

		[TestMethod]
		public void TestOrTwoConditions()
		{
			Assert.AreEqual("<Or><IsNull><FieldRef Name='F' /></IsNull><Eq><FieldRef Name='G' /><Value Type='Integer'>3</Value></Eq></Or>", CamlQuery.Or(
				CamlQuery.IsNull("F"),
				CamlQuery.Eq("G", 3)
			).ToString());
		}

		[TestMethod]
		public void TestAndTwoConditions()
		{
			Assert.AreEqual("<And><IsNull><FieldRef Name='F' /></IsNull><Eq><FieldRef Name='G' /><Value Type='Integer'>3</Value></Eq></And>", CamlQuery.And(
				CamlQuery.IsNull("F"),
				CamlQuery.Eq("G", 3)
			).ToString());
		}

		[TestMethod]
		public void TestOrThreeConditions()
		{
			Assert.AreEqual("<Or><Or><IsNull><FieldRef Name='F' /></IsNull><Eq><FieldRef Name='G' /><Value Type='Integer'>3</Value></Eq></Or><IsNotNull><FieldRef Name='H' /></IsNotNull></Or>", CamlQuery.Or(
				CamlQuery.IsNull("F"),
				CamlQuery.Eq("G", 3),
				CamlQuery.IsNotNull("H")
			).ToString());
		}

		[TestMethod]
		public void TestNestedAnds()
		{
			Assert.AreEqual("<And><And><IsNull><FieldRef Name='F' /></IsNull><Eq><FieldRef Name='G' /><Value Type='Integer'>3</Value></Eq></And><IsNotNull><FieldRef Name='H' /></IsNotNull></And>", CamlQuery.And(
				CamlQuery.And(
					CamlQuery.IsNull("F"),
					CamlQuery.Eq("G", 3)
				),
				CamlQuery.IsNotNull("H")
			).ToString());
		}

		[TestMethod]
		public void TestNestedEmptyAnd()
		{
			Assert.AreEqual("<IsNotNull><FieldRef Name='H' /></IsNotNull>", CamlQuery.And(
				CamlQuery.And(),
				CamlQuery.IsNotNull("H")
			).ToString());
		}
	}
}
