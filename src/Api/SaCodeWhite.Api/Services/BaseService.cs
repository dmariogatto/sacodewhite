using SaCodeWhite.Api.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace SaCodeWhite.Api.Services
{
    public abstract class BaseService
    {
        public const string DateTimeFormat = "dd/MM/yyyy HH:mm";
        public const string AdelaideTimeZoneId = "Cen. Australia Standard Time";

        protected readonly TimeZoneInfo AdelaideTimeZoneInfo;

        protected readonly IHospitalService HospitalService;

        protected readonly ILogService LogService;

        public BaseService(
            IHospitalService hospitalService,
            ILogService logService)
        {
            AdelaideTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(AdelaideTimeZoneId);

            HospitalService = hospitalService;

            LogService = logService;
        }

        public DateTime AdelaideNow
            => TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, AdelaideTimeZoneInfo);

        protected bool TryParseDateTime(string value, out DateTime dateTime)
            => DateTime.TryParseExact(value, DateTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, out dateTime);

        protected async IAsyncEnumerable<T> ValidateHospitalsAsync<T>(IEnumerable<T> dtos, [EnumeratorCancellation] CancellationToken ct) where T : BaseDto
        {
            var validCodesTask = HospitalService.GetValidHospitalCodesAsync(ct);
            var alternateCodesMapTask = HospitalService.GetAlternateHospitalCodesMapAsync(ct);

            await Task.WhenAll(validCodesTask, alternateCodesMapTask);

            var validCodes = validCodesTask.Result;
            var alternateCodesMap = alternateCodesMapTask.Result;

            foreach (var dto in dtos.Where(i => !string.IsNullOrEmpty(i.HospitalCode)))
            {
                if (validCodes.Contains(dto.HospitalCode))
                    yield return dto;
                else if (alternateCodesMap.ContainsKey(dto.HospitalCode))
                {
                    dto.HospitalCode = alternateCodesMap[dto.HospitalCode];
                    yield return dto;
                }
            }
        }
    }
}
