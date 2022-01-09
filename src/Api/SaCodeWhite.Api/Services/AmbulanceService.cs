using SaCodeWhite.Api.Extensions;
using SaCodeWhite.Api.Models.AmbulanceService;
using SaCodeWhite.Shared.Models.AmbulanceService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SaCodeWhite.Api.Services
{
    public class AmbulanceService : BaseService, IAmbulanceService
    {
        private readonly IAmbulanceServiceApi _amboApi;

        public AmbulanceService(
            IAmbulanceServiceApi amboApi,
            IHospitalService hospitalService,
            ILogService logService) : base(hospitalService, logService)
        {
            _amboApi = amboApi;
        }

        public async Task<IList<AmbulanceServiceDashboard>> GetDashboardsAsync(CancellationToken cancellationToken)
        {
            var statusesTask = GetStatusesAsync(cancellationToken);
            var triCatsTask = GetTriageCategoriesAsync(cancellationToken);
            var inpatientBedStatusesTask = GetInpatientBedStatusesAsync(cancellationToken);
            var specialityBedStatusesTask = GetSpecialityBedStatusesAsync(cancellationToken);

            await Task.WhenAll(statusesTask, triCatsTask, inpatientBedStatusesTask, specialityBedStatusesTask).ConfigureAwait(false);

            var statusLookup = statusesTask.Result
                .ToDictionary(i => i.HospitalCode, i => i);
            var triCatLookup = triCatsTask.Result
                .GroupBy(i => i.HospitalCode)
                .ToDictionary(g => g.Key, g => g.ToList());
            var inpatientBedStatusLookup = inpatientBedStatusesTask.Result
                .ToDictionary(i => i.HospitalCode, i => i);
            var specialityBedStatusLookup = specialityBedStatusesTask.Result
                .ToDictionary(i => i.HospitalCode, i => i);

            var result = new List<AmbulanceServiceDashboard>();

            foreach (var status in statusLookup)
            {
                var dashboard = new AmbulanceServiceDashboard()
                {
                    HospitalCode = status.Key,
                    ExpectedArrivals = status.Value.ExpectedArrivals,
                    WaitingToBeSeen = status.Value.WaitingToBeSeen,
                    CommencedTreatment = status.Value.CommencedTreatment,
                    Capacity = status.Value.Capacity,
                    Resuscitation = status.Value.Resuscitation,
                    ClearanceLast3Hrs = status.Value.ClearanceLast3Hrs,
                    UpdatedUtc = status.Value.UpdatedUtc,
                };

                if (triCatLookup.TryGetValue(status.Key, out var triCats))
                    dashboard.TriageCategories = triCats;
                if (inpatientBedStatusLookup.TryGetValue(status.Key, out var inpatientBedStatus))
                    dashboard.InpatientBedStatus = inpatientBedStatus;
                if (specialityBedStatusLookup.TryGetValue(status.Key, out var specialityBedStatus))
                    dashboard.SpecialityBedStatus = specialityBedStatus;

                result.Add(dashboard);
            }

            return result;
        }

        public async Task<IList<AmbulanceStatus>> GetStatusesAsync(CancellationToken cancellationToken)
        {
            var statusDtosTask = _amboApi.GetStatusesAsync(AdelaideNow.ToEpochSeconds(), cancellationToken);
            var last3HrsTask = _amboApi.Get3HrClearRatesAsync(AdelaideNow.ToEpochSeconds(), cancellationToken);

            await Task.WhenAll(statusDtosTask, last3HrsTask).ConfigureAwait(false);

            var statusDtos = statusDtosTask.Result;

            var result = new List<AmbulanceStatus>();

            if (statusDtos?.Any() == true)
            {
                var last3HrsLookup = new Dictionary<string, ClearRate3HrsDto>();

                if (last3HrsTask.Result?.Any() == true)
                {
                    await foreach (var dto in ValidateHospitalsAsync(last3HrsTask.Result, cancellationToken))
                    {
                        last3HrsLookup[dto.HospitalCode] = dto;
                    }
                }

                await foreach (var dto in ValidateHospitalsAsync(statusDtos, cancellationToken))
                {
                    if (TryParseDateTime(dto.TimestampAdelaide, out var timestampAdl))
                    {
                        var status = new AmbulanceStatus() { HospitalCode = dto.HospitalCode };

                        if (int.TryParse(dto.ExpectedAmbulanceArrivals, out var ea))
                            status.ExpectedArrivals = ea;
                        if (int.TryParse(dto.WaitingToBeSeen, out var wtbs))
                            status.WaitingToBeSeen = wtbs;
                        if (int.TryParse(dto.CommencedTreatment, out var comTreat))
                            status.CommencedTreatment = comTreat;
                        if (int.TryParse(dto.Capacity, out var cap))
                            status.Capacity = cap;
                        if (int.TryParse(dto.Resus, out var resus))
                            status.Resuscitation = resus;

                        status.UpdatedUtc = TimeZoneInfo.ConvertTimeToUtc(timestampAdl, AdelaideTimeZoneInfo);

                        if (last3HrsLookup.TryGetValue(dto.HospitalCode, out var clear3HrsDto))
                        {
                            status.ClearanceLast3Hrs = new AmbulanceLast3Hrs();

                            if (int.TryParse(clear3HrsDto.Cleared, out var clr))
                                status.ClearanceLast3Hrs.Cleared = clr;
                            if (int.TryParse(clear3HrsDto.AvgClearTimeMins, out var avgClr))
                                status.ClearanceLast3Hrs.AvgClearedMins = avgClr;
                            if (int.TryParse(clear3HrsDto.WaitingMoreThan30Mins, out var moreThan30Mins))
                                status.ClearanceLast3Hrs.WaitingMoreThan30Mins = moreThan30Mins;
                        }

                        result.Add(status);
                    }
                }
            }

            return result;
        }

        public async Task<IList<AmbulanceTriageCategory>> GetTriageCategoriesAsync(CancellationToken cancellationToken)
        {
            var triCatDtos = await _amboApi.GetTriageCategoriesAsync(AdelaideNow.ToEpochSeconds(), cancellationToken).ConfigureAwait(false);

            var result = new List<AmbulanceTriageCategory>();

            if (triCatDtos?.Any() == true)
            {
                await foreach (var dto in ValidateHospitalsAsync(triCatDtos, cancellationToken))
                {
                    var triCat = new AmbulanceTriageCategory() { HospitalCode = dto.HospitalCode };

                    if (int.TryParse(dto.Category, out var catId))
                        triCat.TriageCategoryId = catId;
                    if (int.TryParse(dto.WaitingToBeSeen, out var wtbs))
                        triCat.WaitingToBeSeen = wtbs;
                    if (int.TryParse(dto.CommencedTreatment, out var comTreat))
                        triCat.CommencedTreatment = comTreat;

                    result.Add(triCat);
                }
            }

            return result
                .OrderBy(i => i.HospitalCode)
                .ThenBy(i => i.TriageCategoryId).ToList();
        }

        public async Task<IList<AmbulanceInpatientBedStatus>> GetInpatientBedStatusesAsync(CancellationToken cancellationToken)
        {
            var inpatientDtos = await _amboApi.GetInpatientBedStatusesAsync(AdelaideNow.ToEpochSeconds(), cancellationToken).ConfigureAwait(false);

            var result = new List<AmbulanceInpatientBedStatus>();

            if (inpatientDtos?.Any() == true)
            {
                await foreach (var dto in ValidateHospitalsAsync(inpatientDtos, cancellationToken))
                {
                    if (TryParseDateTime(dto.TimestampAdelaide, out var timestampAdl))
                    {
                        var inpatientStatus = new AmbulanceInpatientBedStatus() { HospitalCode = dto.HospitalCode };

                        if (int.TryParse(dto.WaitingForBed, out var wfb))
                            inpatientStatus.WaitingForBed = wfb;
                        if (int.TryParse(dto.GeneralWardOccupancy, out var occ))
                            inpatientStatus.GeneralWardOccupancy = occ;
                        if (int.TryParse(dto.GeneralWardCapacity, out var baseCap))
                            inpatientStatus.GeneralWardCapacity = baseCap;

                        inpatientStatus.UpdatedUtc = TimeZoneInfo.ConvertTimeToUtc(timestampAdl, AdelaideTimeZoneInfo);

                        result.Add(inpatientStatus);
                    }
                }
            }

            return result;
        }

        public async Task<IList<AmbulanceSpecialityBedStatus>> GetSpecialityBedStatusesAsync(CancellationToken cancellationToken)
        {
            var specialityDtos = await _amboApi.GetSpecialityBedStatusesAsync(AdelaideNow.ToEpochSeconds(), cancellationToken).ConfigureAwait(false);

            var result = new List<AmbulanceSpecialityBedStatus>();

            if (specialityDtos?.Any() == true)
            {
                await foreach (var dto in ValidateHospitalsAsync(specialityDtos, cancellationToken))
                {
                    if (TryParseDateTime(dto.TimestampAdelaide, out var timestampAdl))
                    {
                        var status = new AmbulanceSpecialityBedStatus() { HospitalCode = dto.HospitalCode };

                        if (int.TryParse(dto.BurnOccupancy, out var burnOcc))
                            status.BurnOccupancy = burnOcc;
                        if (int.TryParse(dto.BurnCapacity, out var burnCap))
                            status.BurnCapacity = burnCap;

                        if (int.TryParse(dto.CoronaryCareOccupancy, out var ccuOcc))
                            status.CoronaryCareOccupancy = ccuOcc;
                        if (int.TryParse(dto.CoronaryCareCapacity, out var ccuCap))
                            status.CoronaryCareCapacity = ccuCap;

                        if (int.TryParse(dto.IntensiveCareOccupancy, out var icuOcc))
                            status.IntensiveCareOccupancy = icuOcc;
                        if (int.TryParse(dto.IntensiveCareCapacity, out var icuCap))
                            status.IntensiveCareCapacity = icuCap;

                        if (int.TryParse(dto.MentalHealthOccupancy, out var mhOcc))
                            status.MentalHealthOccupancy = mhOcc;
                        if (int.TryParse(dto.MentalHealthCapacity, out var mhCap))
                            status.MentalHealthCapacity = mhCap;

                        if (int.TryParse(dto.NeonatalOccupancy, out var neoOcc))
                            status.NeonatalOccupancy = neoOcc;
                        if (int.TryParse(dto.NeonatalCapacity, out var neoCap))
                            status.NeonatalCapacity = neoCap;

                        if (int.TryParse(dto.ObstetricOccupancy, out var obstOcc))
                            status.ObstetricOccupancy = obstOcc;
                        if (int.TryParse(dto.ObstetricCapacity, out var obstCap))
                            status.ObstetricCapacity = obstCap;

                        if (int.TryParse(dto.PaediatricOccupancy, out var paedOcc))
                            status.PaediatricOccupancy = paedOcc;
                        if (int.TryParse(dto.PaediatricCapacity, out var paedCap))
                            status.PaediatricCapacity = obstCap;

                        status.UpdatedUtc = TimeZoneInfo.ConvertTimeToUtc(timestampAdl, AdelaideTimeZoneInfo);

                        result.Add(status);
                    }
                }
            }

            return result;
        }
    }
}
