using System.Threading.Tasks;
using AdoReporting.Core.Models;

namespace AdoReporting.Core.Contracts
{
    public interface IQueryExecutor
    {
        public Task<ITree<AdoWorkItem>> GetWorkItemHierarchy(string project);
        public bool UpdateWorkItem(int id,string title);
    } 
}


