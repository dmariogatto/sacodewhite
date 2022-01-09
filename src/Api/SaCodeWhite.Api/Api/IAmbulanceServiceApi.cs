using Refit;
using SaCodeWhite.Api.Models.AmbulanceService;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SaCodeWhite.Api
{
    public interface IAmbulanceServiceApi
    {
        [Get("/wps/themes/html/Portal/js/OBI_DATA/json/MHS001.json?_={epochSecondsAdl}")]
        Task<List<StatusDto>> GetStatusesAsync(long epochSecondsAdl, CancellationToken cancellationToken);

        [Get("/wps/themes/html/Portal/js/OBI_DATA/json/MHS002.json?_={epochSecondsAdl}")]
        Task<List<TriageCategoryDto>> GetTriageCategoriesAsync(long epochSecondsAdl, CancellationToken cancellationToken);

        [Get("/wps/themes/html/Portal/js/OBI_DATA/json/MHS003.json?_={epochSecondsAdl}")]
        Task<List<InpatientBedStatusDto>> GetInpatientBedStatusesAsync(long epochSecondsAdl, CancellationToken cancellationToken);

        [Get("/wps/themes/html/Portal/js/OBI_DATA/json/MHS004.json?_={epochSecondsAdl}")]
        Task<List<SpecialityBedStatusDto>> GetSpecialityBedStatusesAsync(long epochSecondsAdl, CancellationToken cancellationToken);

        [Get("/wps/themes/html/Portal/js/OBI_DATA/json/MHS005.json?_={epochSecondsAdl}")]
        Task<List<ClearRate3HrsDto>> Get3HrClearRatesAsync(long epochSecondsAdl, CancellationToken cancellationToken);

        [Get("/wps/themes/html/Portal/js/OBI_DATA/json/MHS006.json?_={epochSecondsAdl}")]
        Task<List<ActivityDto>> GetActivityLast24HoursAsync(long epochSecondsAdl, CancellationToken cancellationToken);
    }
}
