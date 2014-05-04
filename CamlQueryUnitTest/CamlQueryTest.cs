using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mokshin.CamlQuery;

namespace CamlQueryUnitTest
{
	[TestClass]
	public class CamlQueryTest
	{
		[TestMethod]
		public void TestScopeNotSet()
		{
			Assert.IsNull(new CamlQuery(CamlQuery.Eq("F", 3)).GetSPQuery().ViewAttributes);
		}

		[TestMethod]
		public void TestScopeFilesOnly()
		{
			Assert.AreEqual("Scope=\"FilesOnly\"", new CamlQuery(CamlQuery.Eq("F", 3)).Scope(CamlScope.FilesOnly).GetSPQuery().ViewAttributes);
		}
	}
}