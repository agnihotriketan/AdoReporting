using AdoReporting.Core.Contracts;
using AdoReporting.Core.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace AdoReporting.Test
{
    [TestClass]
    public class QueryExecutorTest
    {
        private readonly IQueryExecutor _queryExecutor;
        public QueryExecutorTest()
        {
            _queryExecutor = new QueryExecutor("https://dev.azure.com/", "keagniho", "apxduwl2ruk6ncvxpw23w4egdphyyow54qw7zq4ie6pk4zpbuvkq");
        }

        [TestMethod]
        public void GetDataTest_Positive()
        {
            var response = _queryExecutor.GetWorkItemHierarchy("Agile Demo");
            Assert.IsNotNull(response);
        }

        [TestMethod]
        public void GetDataTest_Negative()
        {
            try
            {
                var response = _queryExecutor.GetWorkItemHierarchy("");
                var x = response.Result as ArgumentNullException;
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex.Message.Contains("Project name parameter is missing."));
            }
        }

        [TestMethod]
        public void UpdateWorkItemTest_Input_Positive()
        {
            bool response = _queryExecutor.UpdateWorkItem(154, "From Test");
            Assert.IsTrue(response);
        }

        [TestMethod]
        public void UpdateWorkItemTest_Negative()
        {
            try
            {
                bool response = _queryExecutor.UpdateWorkItem(1550, "");
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex.Message.Contains("does not exist"));
            }
        }
    }
}
