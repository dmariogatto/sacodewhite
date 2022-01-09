using Polly;
using Polly.Retry;
using Refit;
using SaCodeWhite.Api;
using SaCodeWhite.Shared.Models;
using SaCodeWhite.Shared.Models.AmbulanceService;
using SaCodeWhite.Shared.Models.EmergencyDepartment;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Essentials.Interfaces;

namespace SaCodeWhite.Services
{
    public class CodeWhiteService : BaseService, ICodeWhiteService
    {
        private readonly static TimeSpan NotBefore = TimeSpan.FromSeconds(30);
        private readonly static TimeSpan DashboardStaleAfter = TimeSpan.FromMinutes(5);

        private readonly IConnectivity _connectivity;

        private readonly ISaCodeWhiteApi _api;

        private readonly AsyncRetryPolicy _retryPolicy;

        private readonly SemaphoreSlim _hospitalServiceSemaphore = new SemaphoreSlim(1, 1);
        private readonly SemaphoreSlim _triageCategorySemaphore = new SemaphoreSlim(1, 1);
        private readonly SemaphoreSlim _amboSemaphore = new SemaphoreSlim(1, 1);
        private readonly SemaphoreSlim _edSemaphore = new SemaphoreSlim(1, 1);

        private readonly List<Hospital> _hospitals = new List<Hospital>();
        private readonly List<TriageCategory> _triageCategories = new List<TriageCategory>();

        private readonly List<AmbulanceServiceDashboard> _amboDashboards = new List<AmbulanceServiceDashboard>();
        private readonly List<EmergencyDepartmentDashboard> _edDashboards = new List<EmergencyDepartmentDashboard>();

        private DateTime _amboUpdatedUtc = DateTime.MinValue;
        private DateTime _amboFetchedUtc = DateTime.MinValue;

        private DateTime _edUpdatedUtc = DateTime.MinValue;
        private DateTime _edFetchedUtc = DateTime.MinValue;

        public CodeWhiteService(
            IConnectivity connectivity,
            ISaCodeWhiteApi api,
            IRetryPolicyFactory retryPolicyFactory,
            ILogger logger) : base(logger)
        {
            _connectivity = connectivity;

            _api = api;

            _retryPolicy =
                retryPolicyFactory.GetNetRetryPolicy()
                    .WaitAndRetryAsync
                    (
                        retryCount: 2,
                        sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
                    );
        }

        public async Task<IList<Hospital>> GetHospitalsAsync(CancellationToken cancellationToken)
        {
            await _hospitalServiceSemaphore.WaitAsync().ConfigureAwait(false);

            try
            {
                if (!_hospitals.Any())
                {
                    var data = await GetResponseAsync(ct => _api.GetHospitalsAsync(Constants.ApiKeyHospitals, ct), cancellationToken).ConfigureAwait(false);
                    if (data?.Any() == true)
                        _hospitals.AddRange(data);
                }

                return _hospitals;
            }
            finally
            {
                _hospitalServiceSemaphore.Release();
            }
        }

        public async Task<IList<TriageCategory>> GetTriageCategoriesAsync(CancellationToken cancellationToken)
        {
            await _triageCategorySemaphore.WaitAsync().ConfigureAwait(false);

            try
            {
                if (!_triageCategories.Any())
                {
                    var data = await GetResponseAsync(ct => _api.GetTriageCategoriesAsync(Constants.ApiKeyTriageCategories, ct), cancellationToken).ConfigureAwait(false);
                    if (data?.Any() == true)
                        _triageCategories.AddRange(data);
                }

                return _triageCategories;
            }
            finally
            {
                _triageCategorySemaphore.Release();
            }
        }

        public async Task<IList<AmbulanceServiceDashboard>> GetAmbulanceServiceDashboardsAsync(CancellationToken cancellationToken)
        {
            await _amboSemaphore.WaitAsync().ConfigureAwait(false);

            try
            {
                if (!_amboFetchedUtc.InTheLastUtc(NotBefore) && !_amboUpdatedUtc.InTheLastUtc(DashboardStaleAfter))
                {
                    var data = await GetResponseAsync(ct => _api.GetAmbulanceServiceDashboardsAsync(Constants.ApiKeyAmbulanceDashboards, ct), cancellationToken).ConfigureAwait(false);
                    if (data?.Any() == true)
                    {
                        _amboFetchedUtc = DateTime.UtcNow;
                        _amboUpdatedUtc = data.Max(i => i.UpdatedUtc);

                        _amboDashboards.Clear();
                        _amboDashboards.AddRange(data);
                    }
                }

                return _amboDashboards.ToList();
            }
            finally
            {
                _amboSemaphore.Release();
            }
        }

        public async Task<IList<EmergencyDepartmentDashboard>> GetEmergencyDepartmentDashboardsAsync(CancellationToken cancellationToken)
        {
            await _edSemaphore.WaitAsync().ConfigureAwait(false);

            try
            {
                if (!_edFetchedUtc.InTheLastUtc(NotBefore) && !_edUpdatedUtc.InTheLastUtc(DashboardStaleAfter))
                {
                    var data = await GetResponseAsync(ct => _api.GetEmergencyDepartmentDashboardsAsync(Constants.ApiKeyEmergencyDepartmentDashboards, ct), cancellationToken).ConfigureAwait(false);
                    if (data?.Any() == true)
                    {
                        _edFetchedUtc = DateTime.UtcNow;
                        _edUpdatedUtc = data.Max(i => i.UpdatedUtc);

                        _edDashboards.Clear();
                        _edDashboards.AddRange(data);
                    }
                }

                return _edDashboards.ToList();
            }
            finally
            {
                _edSemaphore.Release();
            }
        }

        public async Task<Hospital> GetHospitalAsync(string code, CancellationToken cancellationToken)
        {
            var hospitals = await GetHospitalsAsync(cancellationToken).ConfigureAwait(false);
            return hospitals.Where(i => i.Code == code).FirstOrDefault();
        }

        public async Task<AmbulanceServiceDashboard> GetAmbulanceServiceDashboardAsync(string code, CancellationToken cancellationToken)
        {
            var dashboards = await GetAmbulanceServiceDashboardsAsync(cancellationToken).ConfigureAwait(false);
            return dashboards.Where(i => i.HospitalCode == code).FirstOrDefault();
        }

        public async Task<EmergencyDepartmentDashboard> GetEmergencyDepartmentDashboardAsync(string code, CancellationToken cancellationToken)
        {
            var dashboards = await GetEmergencyDepartmentDashboardsAsync(cancellationToken).ConfigureAwait(false);
            return dashboards.Where(i => i.HospitalCode == code).FirstOrDefault();
        }

        private async Task<TResponse> GetResponseAsync<TResponse>(Func<CancellationToken, Task<TResponse>> apiRequest, CancellationToken cancellationToken)
            where TResponse : class
        {
            if (_connectivity.NetworkAccess != NetworkAccess.Internet)
                return default;

            try
            {
                return await _retryPolicy.ExecuteAsync(apiRequest, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                var url = string.Empty;

                switch (ex)
                {
                    case ApiException apiEx:
                        url = apiEx.Uri.ToString();
                        break;
                }

                Logger.Error(ex, !string.IsNullOrEmpty(url)
                    ? new Dictionary<string, string>() { { nameof(url), url } }
                    : null);
            }

            return default;
        }
    }
}