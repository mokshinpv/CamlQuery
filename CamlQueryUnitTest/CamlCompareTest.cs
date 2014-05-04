using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mokshin.CamlQuery;

namespace CamlQueryUnitTest
{
	[TestClass]
	public class CamlCompareTest
	{
		[TestMethod]
		public void TestEqTrue()
		{
			Assert.AreEqual("<Eq><FieldRef Name='Localization' /><Value Type='Boolean'>1</Value></Eq>", CamlQuery.Eq("Localization", true).ToString());
		}

		[TestMethod]
		public void TestEqFalse()
		{
			Assert.AreEqual("<Eq><FieldRef Name='Localization' /><Value Type='Boolean'>0</Value></Eq>", CamlQuery.Eq("Localization", false).ToString());
		}

		[TestMethod]
		public void TestEqInt()
		{
			Assert.AreEqual("<Eq><FieldRef Name='Field' /><Value Type='Integer'>5</Value></Eq>", CamlQuery.Eq("Field", 5).ToString());
		}

		[TestMethod]
		public void TestEqNote()
		{
			Assert.AreEqual("<Eq><FieldRef Name='abcde' /><Value Type='Note'><![CDATA[test]]></Value></Eq>", CamlQuery.Eq("abcde", CamlFieldType.Note, "test").ToString());
		}

		[TestMethod]
		public void TestGtDate()
		{
			Assert.AreEqual("<Gt><FieldRef Name='DateField' /><Value Type='DateTime'>2014-06-12T00:00:00Z</Value></Gt>", CamlQuery.Gt("DateField", new DateTime(2014, 6, 12)).ToString());
		}

		[TestMethod]
		public void TestLtDate()
		{
			Assert.AreEqual("<Lt><FieldRef Name='DateField' /><Value Type='DateTime'>2014-06-12T00:00:00Z</Value></Lt>", CamlQuery.Lt("DateField", new DateTime(2014, 6, 12)).ToString());
		}

		[TestMethod]
		public void TestEscapeCData()
		{
			Assert.AreEqual("<Eq><FieldRef Name='Field' /><Value Type='Text'><![CDATA[test]]]]><![CDATA[>]]></Value></Eq>", CamlQuery.Eq("Field", "test]]>").ToString());
		}

		[TestMethod]
		public void TestNeqTrue()
		{
			Assert.AreEqual("<Neq><FieldRef Name='F' /><Value Type='Boolean'>1</Value></Neq>", CamlQuery.Neq("F", true).ToString());
		}

		[TestMethod]
		public void TestGeqNumber()
		{
			Assert.AreEqual("<Geq><FieldRef Name='F' /><Value Type='Number'>1</Value></Geq>", CamlQuery.Geq("F", 1.0).ToString());
		}
	}
}
