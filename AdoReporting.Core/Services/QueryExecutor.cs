using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using AdoReporting.Core.Common;
using AdoReporting.Core.Contracts;
using AdoReporting.Core.Models;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Patch;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;

namespace AdoReporting.Core.Services
{
    public class QueryExecutor : IQueryExecutor
    {
        private readonly Uri uri;
        private readonly VssBasicCredential vssBasicCredential;

        public QueryExecutor(string adoUrl, string _orgName, string _personalAccessToken)
        {
            uri = new Uri(adoUrl + _orgName);
            vssBasicCredential = new VssBasicCredential(_orgName, _personalAccessToken);
        }

        /// <summary>
        /// Get Work Item Hierarchy
        /// </summary>
        /// <param name="project">The project name.</param>
        /// <returns></returns>
        public async Task<ITree<AdoWorkItem>> GetWorkItemHierarchy(string project)
        {
            try
            {
                Wiql wiql = new Wiql()
                {
                    Query = GetAdoQuery(project),
                };
                using (WorkItemTrackingHttpClient httpClient = new WorkItemTrackingHttpClient(uri, vssBasicCredential))
                {
                    WorkItemQueryResult result = await httpClient.QueryByWiqlAsync(wiql).ConfigureAwait(false);

                    if (result != null)
                    {
                        if (result.WorkItemRelations != null)
                        {
                            List<AdoWorkItem> WorkItemList = new List<AdoWorkItem>();

                            Parallel.ForEach(result.WorkItemRelations, workItemRelation =>
                            {
                                if (workItemRelation.Source != null)
                                {
                                    WorkItem wiParent = httpClient.GetWorkItemAsync(workItemRelation.Source.Id).GetAwaiter().GetResult();
                                    WorkItem wiChild = httpClient.GetWorkItemAsync(workItemRelation.Target.Id).Result;
                                    WorkItemList.Add(new AdoWorkItem { Id = wiChild.Id, ParentId = wiParent.Id, Title = wiChild.Fields["System.Title"].ToString() });
                                }
                            });
                            ITree<AdoWorkItem> tree = WorkItemList.ToTree((parent, child) => child.ParentId == parent.Id);
                            PrintTreeNode(tree);
                            return tree;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString(CultureInfo.InvariantCulture));
                throw ex;
            }
            return null;
        }

        /// <summary>
        /// Update Work Item
        /// </summary>
        /// <param name="workItemId">The work item id.</param>
        /// <param name="title">The title.</param>
        /// <returns></returns>
        public bool UpdateWorkItem(int workItemId, string title)
        {
            try
            {
                VssConnection connection = new VssConnection(uri, vssBasicCredential);
                WorkItemTrackingHttpClient WitClient = connection.GetClient<WorkItemTrackingHttpClient>();
                Dictionary<string, object> fields = new Dictionary<string, object>
                {
                    { "Title", title },
                    { "History", "Comment from app"}
                };

                JsonPatchDocument patchDocument = new JsonPatchDocument();

                foreach (var key in fields?.Keys)
                    patchDocument.Add(new JsonPatchOperation()
                    {
                        Operation = Operation.Add,
                        Path = "/fields/" + key,
                        Value = fields[key]
                    });

                var x = WitClient.UpdateWorkItemAsync(patchDocument, workItemId).Result;
                return x != null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Get Ado query to execute
        /// </summary>
        /// <param name="project">The project name.</param>
        /// <returns></returns>
        private static string GetAdoQuery(string project)
        {
            if (!string.IsNullOrWhiteSpace(project))
            {

                string q = @"SELECT
                    [System.Id],
                    [System.WorkItemType],
                    [System.Title],
                    [System.AssignedTo],
                    [System.State],
                    [System.Tags]
                    FROM workitemLinks
                    WHERE
                    (
                    [Source].[System.TeamProject] = @project
                    AND [Source].[System.WorkItemType] <> ''
                    AND [Source].[System.State] <> ''
                    )
                    AND (
                    [System.Links.LinkType] = 'System.LinkTypes.Hierarchy-Forward'
                    )
                    AND (
                    [Target].[System.TeamProject] = @project
                    AND [Target].[System.WorkItemType] <> ''
                    )
                    MODE (Recursive)";
                q = q.Replace("@project", "'" + project + "'");
                return q;
            }
            else
                throw new ArgumentNullException("Project name parameter is missing.");
        }

        /// <summary>
        /// Print Tree Node
        /// </summary>
        /// <param name="node">The node.</param>
        private void PrintTreeNode(ITree<AdoWorkItem> node)
        {
            string indent = new string('-', node.Level);
            string type = node.Data?.Title ?? "--TREE ROOT--";

            Console.WriteLine($"{indent}{type}");

            foreach (var child in node.Children)
            {
                PrintTreeNode(child);
            }
        }
    }
}


