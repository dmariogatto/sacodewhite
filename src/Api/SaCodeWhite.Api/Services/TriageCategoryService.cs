using SaCodeWhite.Shared.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SaCodeWhite.Api.Services
{
    public class TriageCategoryService : ITriageCategoryService
    {
        private readonly List<TriageCategory> _triageCategories = new()
        {
            new TriageCategory()
            {
                Id = 1,
                DisplayName = "Resuscitation",
                Description = "Maximum wait time 2 minutes",
                MaxWaitMins = 2,
            },
            new TriageCategory()
            {
                Id = 2,
                DisplayName = "Emergency",
                Description = "Maximum wait time 10 minutes",
                MaxWaitMins = 10,
            },
            new TriageCategory()
            {
                Id = 3,
                DisplayName = "Urgent",
                Description = "Maximum wait time 30 minutes",
                MaxWaitMins = 30,
            },
            new TriageCategory()
            {
                Id = 4,
                DisplayName = "Semi-urgent",
                Description = "Maximum wait time 60 minutes",
                MaxWaitMins = 60,
            },
            new TriageCategory()
            {
                Id = 5,
                DisplayName = "Non-urgent",
                Description = "Maximum wait time 120 minutes",
                MaxWaitMins = 120,
            },
        };

        public TriageCategoryService()
        {
        }

        public Task<IList<TriageCategory>> GetTriageCategories(CancellationToken cancellationToken)
            => Task.FromResult<IList<TriageCategory>>(_triageCategories.ToList());
    }
}
