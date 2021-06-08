using AdoReporting.Core.Contracts;
using AdoReporting.Core.Services;
using Microsoft.Extensions.Configuration;
using System;

namespace AdoReporting.Console
{
    class Program
    {
        private static readonly IConfiguration configuration;
        private static readonly IQueryExecutor queryExecutor;
        static Program()
        {
            configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json", true, true).Build();
            queryExecutor = GetQueryExecutor();
        }

        static async System.Threading.Tasks.Task Main(string[] args)
        {
            try
            { 
                string IsContinue;
                do
                {
                    System.Console.WriteLine("------ ADO Reporting Demo ----");
                    System.Console.WriteLine("1. Get all work items \n 2. Update work item\n  Please enter your choice.");
                    int choice = Convert.ToInt32(System.Console.ReadLine());
                    switch (choice)
                    {
                        case 1:
                            var response = await queryExecutor.GetWorkItemHierarchy("Agile Demo");
                            break;
                        case 2:
                            System.Console.WriteLine("Please enter work item id : ");
                            int workItemId = Convert.ToInt32(System.Console.ReadLine());
                            System.Console.WriteLine("Please enter work item title : ");
                            string title = System.Console.ReadLine();
                            bool status = queryExecutor.UpdateWorkItem(workItemId, title);
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
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static IQueryExecutor GetQueryExecutor()
        {
            IQueryExecutor _queryExecutor = new QueryExecutor(configuration?.GetSection("AdoSettings:AdoUrl").Value,
                configuration?.GetSection("AdoSettings:OrgName")?.Value,
                configuration?.GetSection("AdoSettings:PersonalAccessToken")?.Value);
            return _queryExecutor;
        }
    }
}
