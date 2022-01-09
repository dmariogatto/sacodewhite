using SaCodeWhite.Api.Extensions;
using SaCodeWhite.Shared.Models.EmergencyDepartment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SaCodeWhite.Api.Services
{
    public class EmergencyDepartmentService : BaseService, IEmergencyDepartmentService
    {
        private readonly IEmergencyDepartmentApi _edApi;

        public EmergencyDepartmentService(
            IEmergencyDepartmentApi edApi,
            IHospitalService hospitalService,
            ILogService logService) : base(hospitalService, logService)
        {
            _edApi = edApi;
        }

        public async Task<(int, DateTime)> GetEtlAsync(CancellationToken cancellationToken)
        {
            var etlDto = await _edApi.GetEtlAsync(cancellationToken).ConfigureAwait(false);

            var id = -1;
            var timestamp = DateTime.MinValue;

            if (int.TryParse(etlDto?.FirstOrDefault()?.Id, out var val))
                id = val;

            if (TryParseDateTime(etlDto?.FirstOrDefault()?.TimestampAdelaide, out var timestampAdl))
                timestamp = TimeZoneInfo.ConvertTimeToUtc(timestampAdl, AdelaideTimeZoneInfo);

            return (id, timestamp);
        }

        public async Task<IList<EmergencyDepartmentDashboard>> GetDashboardsAsync(CancellationToken cancellationToken)
        {
            var statusesTask = GetStatusesAsync(cancellationToken);
            var departuresTask = GetDeparturesLast1HrAsync(cancellationToken);
            var triCatsTask = GetTriageCategoriesAsync(cancellationToken);
            var waitingTimesTask = GetWaitingTimesAsync(cancellationToken);

            await Task.WhenAll(statusesTask, departuresTask, triCatsTask, waitingTimesTask).ConfigureAwait(false);

            var statusLookup = statusesTask.Result
                .ToDictionary(i => i.HospitalCode, i => i);
            var departuresLookup = departuresTask.Result
                .GroupBy(i => i.HospitalCode)
                .ToDictionary(g => g.Key, g => g.ToList());
            var triCatLookup = triCatsTask.Result
                .GroupBy(i => i.HospitalCode)
                .ToDictionary(g => g.Key, g => g.ToList());
            var waitingTimesLookup = waitingTimesTask.Result
                .GroupBy(i => i.HospitalCode)
                .ToDictionary(g => g.Key, g => g.ToList());

            var result = new List<EmergencyDepartmentDashboard>();

            foreach (var status in statusLookup)
            {
                var dashboard = new EmergencyDepartmentDashboard()
                {
                    HospitalCode = status.Key,
                    ExpectedArrivals = status.Value.ExpectedArrivals,
                    WaitingToBeSeen = status.Value.WaitingToBeSeen,
                    CommencedTreatment = status.Value.CommencedTreatment,
                    Capacity = status.Value.Capacity,
                    AverageWaitMins = status.Value.AverageWaitMins,
                    UpdatedUtc = status.Value.UpdatedUtc,
                };

                if (departuresLookup.TryGetValue(status.Key, out var deps))
                    dashboard.DeparturesLast1Hr = deps;
                if (triCatLookup.TryGetValue(status.Key, out var triCats))
                    dashboard.TriageCategories = triCats;
                if (waitingTimesLookup.TryGetValue(status.Key, out var wtimes))
                    dashboard.WaitingTimes = wtimes;

                result.Add(dashboard);
            }

            return result;
        }

        public async Task<IList<EmergencyDepartmentStatus>> GetStatusesAsync(CancellationToken cancellationToken)
        {
            var statusDtos = await _edApi.GetStatusesAsync(AdelaideNow.ToEpochSeconds(), cancellationToken).ConfigureAwait(false);

            var result = new List<EmergencyDepartmentStatus>();

            if (statusDtos?.Any() == true)
            {
                await foreach (var dto in ValidateHospitalsAsync(statusDtos, cancellationToken))
                {
                    if (dto.TimestampAdelaide.HasValue)
                    {
                        var status = new EmergencyDepartmentStatus() { HospitalCode = dto.HospitalCode };

                        if (int.TryParse(dto.ExpectedArrivals, out var ea))
                            status.ExpectedArrivals = ea;
                        if (int.TryParse(dto.WaitingToBeSeen, out var wtbs))
                            status.WaitingToBeSeen = wtbs;
                        if (int.TryParse(dto.CommencedTreatment, out var comTreat))
                            status.CommencedTreatment = comTreat;
                        if (int.TryParse(dto.Capacity, out var cap))
                            status.Capacity = cap;
                        if (double.TryParse(dto.AverageWaitMins, out var avgWaitMins))
                            status.AverageWaitMins = avgWaitMins;

                        status.UpdatedUtc = TimeZoneInfo.ConvertTimeToUtc(dto.TimestampAdelaide.Value, AdelaideTimeZoneInfo);

                        result.Add(status);
                    }
                }
            }

            return result;
        }

        public async Task<IList<EmergencyDepartmentDeparturesLast1Hr>> GetDeparturesLast1HrAsync(CancellationToken cancellationToken)
        {
            var departureDtos = await _edApi.Get1HrDepartureTypesAsync(AdelaideNow.ToEpochSeconds(), cancellationToken).ConfigureAwait(false);

            var result = new List<EmergencyDepartmentDeparturesLast1Hr>();

            if (departureDtos?.Any() == true)
            {
                await foreach (var dto in ValidateHospitalsAsync(departureDtos, cancellationToken))
                {
                    var dep = new EmergencyDepartmentDeparturesLast1Hr()
                    {
                        HospitalCode = dto.HospitalCode,
                        Category = dto.Type,
                    };

                    if (int.TryParse(dto.Total, out var total))
                        dep.Count = total;

                    result.Add(dep);
                }
            }

            return result;
        }

        public async Task<IList<EmergencyDepartmentTriageCategory>> GetTriageCategoriesAsync(CancellationToken cancellationToken)
        {
            var triCatDtos = await _edApi.GetTriageCategoriesAsync(AdelaideNow.ToEpochSeconds(), cancellationToken).ConfigureAwait(false);

            var result = new List<EmergencyDepartmentTriageCategory>();

            if (triCatDtos?.Any() == true)
            {
                await foreach (var dto in ValidateHospitalsAsync(triCatDtos, cancellationToken))
                {
                    var triCat = new EmergencyDepartmentTriageCategory() { HospitalCode = dto.HospitalCode };

                    if (int.TryParse(dto.Category, out var catId))
                        triCat.TriageCategoryId = catId;
                    if (int.TryParse(dto.WaitingToBeSeen, out var wtbs))
                        triCat.WaitingToBeSeen = wtbs;
                    if (int.TryParse(dto.WaitingOverTime, out var wot))
                        triCat.WaitingOverTime = wot;
                    if (int.TryParse(dto.CommencedTreatment, out var comTreat))
                        triCat.CommencedTreatment = comTreat;

                    result.Add(triCat);
                }
            }

            return result
                .OrderBy(i => i.HospitalCode)
                .ThenBy(i => i.TriageCategoryId).ToList();
        }

        public async Task<IList<EmergencyDepartmentWaitingTime>> GetWaitingTimesAsync(CancellationToken cancellationToken)
        {
            var waitingDtos = await _edApi.GetWaitingTimesAsync(AdelaideNow.ToEpochSeconds(), cancellationToken).ConfigureAwait(false);

            var result = new List<EmergencyDepartmentWaitingTime>();

            if (waitingDtos?.Any() == true)
            {
                await foreach (var dto in ValidateHospitalsAsync(waitingDtos, cancellationToken))
                {
                    var waitTime = new EmergencyDepartmentWaitingTime()
                    {
                        HospitalCode = dto.HospitalCode,
                        Category = dto.Category
                    };

                    if (int.TryParse(dto.LessThan2Hours, out var less2))
                        waitTime.LessThan2Hours = less2;
                    if (int.TryParse(dto.Between2And4Hours, out var b2and4))
                        waitTime.Between2And4Hours = b2and4;
                    if (int.TryParse(dto.Between4And8Hours, out var b4and8))
                        waitTime.Between4And8Hours = b4and8;
                    if (int.TryParse(dto.Between8And12Hours, out var b8and12))
                        waitTime.Between8And12Hours = b8and12;
                    if (int.TryParse(dto.Between12And24Hours, out var b12and24))
                        waitTime.Between12And24Hours = b12and24;
                    if (int.TryParse(dto.Over24Hours, out var over24))
                        waitTime.Over24Hours = over24;

                    result.Add(waitTime);
                }
            }

            return result;
        }
    }
}
