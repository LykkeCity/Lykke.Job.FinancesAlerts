using System;
using Autofac;
using AzureStorage.Tables;
using Common;
using Lykke.Common.Log;
using Lykke.Job.FinancesAlerts.AzureRepositories;
using Lykke.Job.FinancesAlerts.Domain.Repositories;
using Lykke.Job.FinancesAlerts.Domain.Services;
using Lykke.Job.FinancesAlerts.DomainServices;
using Lykke.Job.FinancesAlerts.DomainServices.MetricCalculators;
using Lykke.Job.FinancesAlerts.Services;
using Lykke.Job.FinancesAlerts.Settings.JobSettings;
using Lykke.Job.FinancesAlerts.PeriodicalHandlers;
using Lykke.Sdk;
using Lykke.Sdk.Health;
using Lykke.SettingsReader;

namespace Lykke.Job.FinancesAlerts.Modules
{
    public class JobModule : Module
    {
        private readonly FinancesAlertsJobSettings _settings;
        private readonly IReloadingManager<FinancesAlertsJobSettings> _settingsManager;

        public JobModule(FinancesAlertsJobSettings settings, IReloadingManager<FinancesAlertsJobSettings> settingsManager)
        {
            _settings = settings;
            _settingsManager = settingsManager;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<HealthService>()
                .As<IHealthService>()
                .SingleInstance();

            builder.RegisterType<StartupManager>()
                .As<IStartupManager>()
                .SingleInstance();

            builder.RegisterType<ShutdownManager>()
                .As<IShutdownManager>()
                .AutoActivate()
                .SingleInstance();

            builder.RegisterType<PeriodicalHandler>()
                .As<IStopable>()
                .SingleInstance();

            builder.Register(c =>
                    new AlertRuleRepository(
                        AzureTableStorage<AlertRuleEntity>.Create(
                            _settingsManager.ConnectionString(s => s.Db.DataConnString),
                            "AlertRules",
                            c.Resolve<ILogFactory>(),
                            TimeSpan.FromSeconds(30))))
                .As<IAlertRuleRepository>()
                .SingleInstance();

            builder.RegisterType<MetricsChecker>()
                .As<IMetricsChecker>()
                .SingleInstance();

            builder.RegisterType<MetricCalculatorRegistry>()
                .As<IMetricCalculatorRegistry>()
                .SingleInstance();

            RegisterMetricCalculators(builder);
        }

        private void RegisterMetricCalculators(ContainerBuilder builder)
        {
            builder.RegisterType<FuturesLiqPriceRangeMetricCalculator>()
                .As<IMetricCalculator>()
                .SingleInstance()
                .WithParameter(TypedParameter.From(_settings.CryptoFacilities));
        }
    }
}
