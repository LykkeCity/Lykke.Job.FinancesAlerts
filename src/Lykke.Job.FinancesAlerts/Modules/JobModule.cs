﻿using System;
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
using Lykke.Job.FinancesAlerts.Settings;
using Lykke.Sdk;
using Lykke.Sdk.Health;
using Lykke.Service.EmailPartnerRouter;
using Lykke.Service.SmsSender.Client;
using Lykke.SettingsReader;

namespace Lykke.Job.FinancesAlerts.Modules
{
    public class JobModule : Module
    {
        private readonly AppSettings _settings;
        private readonly IReloadingManager<FinancesAlertsJobSettings> _settingsManager;

        public JobModule(IReloadingManager<AppSettings> settingsManager)
        {
            _settings = settingsManager.CurrentValue;
            _settingsManager = settingsManager.Nested(s => s.FinancesAlertsJob);
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
                .AsSelf()
                .As<IStopable>()
                .SingleInstance();

            builder.RegisterType<AlertNotifier>()
                .As<IAlertNotifier>()
                .SingleInstance();

            builder.RegisterType<MetricsChecker>()
                .As<IMetricsChecker>()
                .SingleInstance();

            builder.RegisterType<MetricCalculatorRegistry>()
                .As<IMetricCalculatorRegistry>()
                .SingleInstance();

            RegisterAzureRepositories(builder);

            RegisterNotificationComponents(builder);

            RegisterMetricCalculators(builder);
        }

        private void RegisterMetricCalculators(ContainerBuilder builder)
        {
            builder.RegisterType<FuturesLiqPriceRangeMetricCalculator>()
                .As<IMetricCalculator>()
                .SingleInstance()
                .WithParameter(TypedParameter.From(_settings.FinancesAlertsJob.CryptoFacilities));
        }

        private void RegisterAzureRepositories(ContainerBuilder builder)
        {
            builder.Register(c =>
                    new AlertRuleRepository(
                        AzureTableStorage<AlertRuleEntity>.Create(
                            _settingsManager.ConnectionString(s => s.Db.DataConnString),
                            "AlertRules",
                            c.Resolve<ILogFactory>(),
                            TimeSpan.FromSeconds(30))))
                .As<IAlertRuleRepository>()
                .SingleInstance();

            builder.Register(c =>
                    new AlertSubscriptionRepository(
                        AzureTableStorage<AlertSubscriptionEntity>.Create(
                            _settingsManager.ConnectionString(s => s.Db.DataConnString),
                            "AlertSubscriptions",
                            c.Resolve<ILogFactory>(),
                            TimeSpan.FromSeconds(30))))
                .As<IAlertSubscriptionRepository>()
                .SingleInstance();
        }

        private void RegisterNotificationComponents(ContainerBuilder builder)
        {
            builder.Register(ctx =>
                    new EmailPartnerRouterClient(_settings.EmailPartnerRouterServiceClient.ServiceUrl, ctx.Resolve<ILogFactory>().CreateLog(nameof(EmailPartnerRouterClient))))
                .As<IEmailPartnerRouter>()
                .SingleInstance();

            if (_settings.FinancesAlertsJob.UseSmsMocks)
            {
                builder.Register(ctx => new SmsMockRepository(
                        AzureTableStorage<SmsMessageMockEntity>.Create(
                            _settingsManager.ConnectionString(s => s.Db.DataConnString),
                            "MockSms",
                            ctx.Resolve<ILogFactory>(),
                            TimeSpan.FromSeconds(30))))
                    .As<ISmsMockRepository>()
                    .SingleInstance();
                builder.RegisterType<SmsMockSender>().As<ISmsSenderClient>().SingleInstance();
            }
            else
            {
                builder.RegisterSmsSenderClient(_settings.SmsSenderServiceClient);
            }
        }
    }
}
