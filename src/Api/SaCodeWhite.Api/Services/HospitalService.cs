using SaCodeWhite.Shared.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SaCodeWhite.Api.Services
{
    public class HospitalService : IHospitalService
    {
        private readonly List<Hospital> _hospitals = new()
        {
            new Hospital()
            {
                Id = 1,
                Code = "FMC",
                DisplayName = "Flinders Medical Centre",
                Address = "Flinders Drive",
                Suburb = "Bedford Park",
                State = "South Australia",
                Postcode = "5042",
                Phone = "(08) 8204 5511",
                Fax = "(08) 8204 5450",
                Website = string.Empty,
                Latitude = -35.0217,
                Longitude = 138.5684
            },
            new Hospital()
            {
                Id = 2,
                Code = "LMH",
                DisplayName = "Lyell McEwin Hospital",
                Address = "Haydown Road",
                Suburb = "Elizabeth Vale",
                State = "South Australia",
                Postcode = "5112",
                Phone = "(08) 8182 9000",
                Fax = "(08) 8182 9499",
                Website = string.Empty,
                Latitude = -34.7475,
                Longitude = 138.6646
            },
            new Hospital()
            {
                Id = 3,
                Code = "MH",
                DisplayName = "Modbury Hospital",
                Address = "Smart Road",
                Suburb = "Modbury",
                State = "South Australia",
                Postcode = "5092",
                Phone = "(08) 8161 2000",
                Fax = string.Empty,
                Website = string.Empty,
                Latitude = -34.8341,
                Longitude = 138.6898
            },
            new Hospital()
            {
                Id = 4,
                Code = "NHS",
                DisplayName = "Noarlunga Hospital",
                Address = "30 Alexander Kelly Drive",
                Suburb = "Noarlunga",
                State = "South Australia",
                Postcode = "5168",
                Phone = "(08) 8384 9222",
                Fax = "(08) 8326 3696",
                Website = string.Empty,
                Latitude = -35.1391,
                Longitude = 138.5006
            },
            new Hospital()
            {
                Id = 5,
                Code = "RAH",
                DisplayName = "Royal Adelaide Hospital",
                Address = "Port Road",
                Suburb = "Adelaide",
                State = "South Australia",
                Postcode = "5000",
                Phone = "(08) 7074 0000",
                Fax = string.Empty,
                Website = "www.rah.sa.gov.au",
                Latitude = -34.9209,
                Longitude = 138.5872
            },
            new Hospital()
            {
                Id = 6,
                Code = "TQEH",
                DisplayName = "The Queen Elizabeth Hospital",
                Address = "28 Woodville Road",
                Suburb = "Woodville",
                State = "South Australia",
                Postcode = "5011",
                Phone = "(08) 8222 6000",
                Fax = "(08) 8222 6010",
                Website = string.Empty,
                Latitude = -34.8837,
                Longitude = 138.5331
            },
            new Hospital()
            {
                Id = 7,
                Code = "WCH",
                DisplayName = "Women's and Children's Hospital",
                Address = "72 King William Road",
                Suburb = "North Adelaide",
                State = "South Australia",
                Postcode = "5006",
                Phone = "(08) 8161 7000",
                Fax = "(08) 8161 7459",
                Website = string.Empty,
                Latitude = -34.9115,
                Longitude = 138.6000,
                AlternateCodes = new List<string>() { "WCHP", "WCHW" }
            }
        };

        private readonly HashSet<string> _validCodes;
        private readonly Dictionary<string, string> _alternateCodes;
        private readonly Dictionary<string, Hospital> _hospitalMap;

        public HospitalService()
        {
            _validCodes = new HashSet<string>(_hospitals.Select(i => i.Code));
            _alternateCodes = new Dictionary<string, string>();

            foreach (var h in _hospitals.Where(i => i.AlternateCodes?.Any() == true))
            {
                foreach (var ac in h.AlternateCodes)
                    _alternateCodes[ac] = h.Code;
            }

            _hospitalMap = _hospitals.ToDictionary(i => i.Code, i => i);
        }

        public Task<IList<Hospital>> GetHospitalsAsync(CancellationToken cancellationToken)
            => Task.FromResult<IList<Hospital>>(_hospitals);

        public Task<IDictionary<string, Hospital>> GetHospitalsByCodeAsync(CancellationToken cancellationToken)
            => Task.FromResult<IDictionary<string, Hospital>>(_hospitalMap);

        public Task<ISet<string>> GetValidHospitalCodesAsync(CancellationToken cancellationToken)
            => Task.FromResult<ISet<string>>(_validCodes);

        public Task<IDictionary<string, string>> GetAlternateHospitalCodesMapAsync(CancellationToken cancellationToken)
            => Task.FromResult<IDictionary<string, string>>(_alternateCodes);

        public Task<int> GetHospitalIdAsync(string code, CancellationToken cancellationToken)
            => Task.FromResult(_hospitalMap.TryGetValue(code, out var h) ? h.Id : -1);
    }
}
