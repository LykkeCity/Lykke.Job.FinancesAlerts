using System;
using Autofac;
using JetBrains.Annotations;
using Lykke.HttpClientGenerator;
using Lykke.HttpClientGenerator.Infrastructure;

namespace Lykke.Job.FinancesAlerts.Client
{
    /// <summary>
    /// Extension for client registration
    /// </summary>
    [PublicAPI]
    public static class AutofacExtension
    {
        /// <summary>
        /// Registers <see cref="IFinancesAlertsClient"/> in Autofac container using <see cref="FinancesAlertsJobClientSettings"/>.
        /// </summary>
        /// <param name="builder">Autofac container builder.</param>
        /// <param name="settings">LykkeService client settings.</param>
        /// <param name="builderConfigure">Optional <see cref="HttpClientGeneratorBuilder"/> configure handler.</param>
        public static void RegisterLykkeServiceClient(
            [NotNull] this ContainerBuilder builder,
            [NotNull] FinancesAlertsJobClientSettings settings,
            [CanBeNull] Func<HttpClientGeneratorBuilder, HttpClientGeneratorBuilder> builderConfigure)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));
            if (string.IsNullOrWhiteSpace(settings.Url))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(FinancesAlertsJobClientSettings.Url));

            var clientBuilder = HttpClientGenerator.HttpClientGenerator.BuildForUrl(settings.Url)
                .WithAdditionalCallsWrapper(new ExceptionHandlerCallsWrapper());

            clientBuilder = builderConfigure?.Invoke(clientBuilder) ?? clientBuilder.WithoutRetries();

            builder.RegisterInstance(new FinancesAlertsClient(clientBuilder.Create()))
                .As<IFinancesAlertsClient>()
                .SingleInstance();
        }
    }
}
