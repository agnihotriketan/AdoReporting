using AdoReporting.Core.Contracts;
using AdoReporting.Core.Services;
using System;

namespace AdoReporting.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            IQueryExecutor _queryExecutor = new QueryExecutor("https://dev.azure.com/", "keagniho", "apxduwl2ruk6ncvxpw23w4egdphyyow54qw7zq4ie6pk4zpbuvkq");
            string IsContinue;
            do
            {
                System.Console.WriteLine("------ ADO Reporting Demo ----");
                System.Console.WriteLine("1. Get all work items \n 2. Update work item \n Please enter your choice.");
                int choice = Convert.ToInt32(System.Console.ReadLine());
                switch (choice)
                {
                    case 1:
                        var response = _queryExecutor.GetWorkItemHierarchy("Agile Demo");
                        break;
                    case 2:
                        System.Console.WriteLine("Please enter work item id : ");
                        int workItemId = Convert.ToInt32(System.Console.ReadLine());
                        System.Console.WriteLine("Please enter work item title : ");
                        string title = System.Console.ReadLine();
                        bool status = _queryExecutor.UpdateWorkItem(workItemId, title);
                        System.Console.WriteLine("Update Status: " + status);
                        break;
                    default:
                        System.Console.WriteLine("Wrong choice");
                        break;
                }

                System.Console.WriteLine("Do you want to continue? press Y/y");
                IsContinue = System.Console.ReadLine().Trim().ToLower();
            } while (IsContinue == "y");

            System.Console.ReadLine();
        }
    }
}
