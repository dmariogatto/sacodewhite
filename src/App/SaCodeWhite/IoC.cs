using Newtonsoft.Json;
using Plugin.StoreReview;
using Refit;
using SaCodeWhite.Api;
using SaCodeWhite.Services;
using SaCodeWhite.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using TinyIoC;
using Xamarin.Essentials.Interfaces;

[assembly: SaCodeWhite.Attributes.Preserve]
namespace SaCodeWhite
{
    public static class IoC
    {
        private static readonly TinyIoCContainer Container = new TinyIoCContainer();

        static IoC()
        {
            var refitSettings = new RefitSettings(new NewtonsoftJsonContentSerializer(new JsonSerializerSettings()
            {
                DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                NullValueHandling = NullValueHandling.Ignore
            }));

            Container.Register((c, e) =>
            {
                var client = new HttpClient()
                {
                    BaseAddress = new Uri(Constants.ApiUrlBase)
                };

                return RestService.For<ISaCodeWhiteApi>(client, refitSettings);
            }).AsSingleton();

            Container.Register((c, e) => CrossStoreReview.Current).AsSingleton();

            Container.Register<ILogger, Logger>().AsSingleton();
            Container.Register<ICodeWhiteService, CodeWhiteService>().AsSingleton();
            Container.Register<INotificationService, NotificationService>().AsSingleton();
            Container.Register<IAppPreferences, AppPreferences>().AsSingleton();
            Container.Register<IRetryPolicyFactory, RetryPolicyFactory>().AsSingleton();
            Container.Register<IBvmConstructor, BvmConstructor>().AsSingleton();

            foreach (var e in GetEssentialInterfaceAndImplementations())
            {
                Container.Register(e.Key, e.Value).AsSingleton();
            }

            foreach (var vmType in GetViewModelTypes())
            {
                Container.Register(vmType).AsMultiInstance();
            }
        }

        public static IDictionary<Type, Type> GetEssentialInterfaceAndImplementations()
        {
            var result = new Dictionary<Type, Type>();

            var essentialImpls = typeof(IEssentialsImplementation)
                .Assembly
                .GetTypes()
                .Where(t => t.IsClass && t.Namespace.EndsWith(nameof(Xamarin.Essentials.Implementation)));

            foreach (var impl in essentialImpls)
            {
                var implInterface = impl.GetInterfaces().First(i => i != typeof(IEssentialsImplementation));
                result.Add(implInterface, impl);
            }

            return result;
        }

        public static IEnumerable<Type> GetViewModelTypes()
        {
            return typeof(BaseViewModel)
                .Assembly
                .GetTypes()
                .Where(t => t.IsClass &&
                            !t.IsAbstract &&
                            t.GetInterfaces().Contains(typeof(IViewModel)));
        }

        public static T Resolve<T>() where T : class
        {
            return Container.Resolve<T>();
        }

        public static TViewModel ResolveViewModel<TViewModel>() where TViewModel : class, IViewModel
        {
            return Container.Resolve<TViewModel>();
        }

        public static void RegisterSingleton<TService, TImplementation>() where TService : class where TImplementation : class, TService
        {
            Container.Register<TService, TImplementation>().AsSingleton();
        }

        public static void RegisterSingleton(Type serviceType, Type implementationType)
        {
            Container.Register(serviceType, implementationType).AsSingleton();
        }

        public static void RegisterSingleton(Type serviceType, Func<object> instanceCreator)
        {
            Container.Register(serviceType, instanceCreator).AsSingleton();
        }

        public static void RegisterTransient<TService, TImplementation>() where TService : class where TImplementation : class, TService
        {
            Container.Register<TService, TImplementation>().AsMultiInstance();
        }

        public static void RegisterTransient(Type serviceType, Type implementationType)
        {
            Container.Register(serviceType, implementationType).AsMultiInstance();
        }

        public static void RegisterTransient(Type serviceType, Func<object> instanceCreator)
        {
            Container.Register(serviceType, instanceCreator).AsMultiInstance();
        }
    }
}