using Refit;
using SaCodeWhite.Api.Models.EmergencyDepartment;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SaCodeWhite.Api
{
    public interface IEmergencyDepartmentApi
    {
        [Get("/wps/themes/html/Portal/js/OBI_DATA/json/ED_Etl_Cntrl.json")]
        Task<List<EtlDto>> GetEtlAsync(CancellationToken cancellationToken);

        [Get("/wps/themes/html/Portal/js/OBI_DATA/json/ED001.json?_={epochSecondsAdl}")]
        Task<List<StatusDto>> GetStatusesAsync(long epochSecondsAdl, CancellationToken cancellationToken);

        [Get("/wps/themes/html/Portal/js/OBI_DATA/json/ED002.json?_={epochSecondsAdl}")]
        Task<List<ActivityDto>> GetActivityLast24HoursAsync(long epochSecondsAdl, CancellationToken cancellationToken);

        [Get("/wps/themes/html/Portal/js/OBI_DATA/json/ED003.json?_={epochSecondsAdl}")]
        Task<List<StreamDto>> GetStreamsAsync(long epochSecondsAdl, CancellationToken cancellationToken);

        [Get("/wps/themes/html/Portal/js/OBI_DATA/json/ED004.json?_={epochSecondsAdl}")]
        Task<List<WaitingTimeDto>> GetWaitingTimesAsync(long epochSecondsAdl, CancellationToken cancellationToken);

        [Get("/wps/themes/html/Portal/js/OBI_DATA/json/ED006.json?_={epochSecondsAdl}")]
        Task<List<TriageCategoryDto>> GetTriageCategoriesAsync(long epochSecondsAdl, CancellationToken cancellationToken);

        [Get("/wps/themes/html/Portal/js/OBI_DATA/json/ED007.json?_={epochSecondsAdl}")]
        Task<List<ExpectedArrivalDto>> GetExpectedArrivalsAsync(long epochSecondsAdl, CancellationToken cancellationToken);

        [Get("/wps/themes/html/Portal/js/OBI_DATA/json/ED009.json?_={epochSecondsAdl}")]
        Task<List<DepartureLast1HrDto>> Get1HrDepartureTypesAsync(long epochSecondsAdl, CancellationToken cancellationToken);
    }
}
