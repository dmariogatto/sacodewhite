using SaCodeWhite.Shared.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SaCodeWhite.Api.Services
{
    public interface ITriageCategoryService
    {
        public Task<IList<TriageCategory>> GetTriageCategories(CancellationToken cancellationToken);
    }
}
