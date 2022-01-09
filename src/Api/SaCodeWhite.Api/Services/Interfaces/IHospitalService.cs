using SaCodeWhite.Shared.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SaCodeWhite.Api.Services
{
    public interface IHospitalService
    {
        public Task<IList<Hospital>> GetHospitalsAsync(CancellationToken cancellationToken);
        public Task<IDictionary<string, Hospital>> GetHospitalsByCodeAsync(CancellationToken cancellationToken);

        public Task<ISet<string>> GetValidHospitalCodesAsync(CancellationToken cancellationToken);
        public Task<IDictionary<string, string>> GetAlternateHospitalCodesMapAsync(CancellationToken cancellationToken);
    }
}
