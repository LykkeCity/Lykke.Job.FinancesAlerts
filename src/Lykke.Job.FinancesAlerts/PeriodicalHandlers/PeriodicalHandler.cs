using System;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Common;
using Common.Log;
using Lykke.Common.Log;
using Lykke.Job.FinancesAlerts.Domain.Services;

namespace Lykke.Job.FinancesAlerts.PeriodicalHandlers
{
    public class PeriodicalHandler : IStartable, IStopable
    {
        private readonly ILog _log;
        private readonly IMetricsChecker _metricsChecker;
        private readonly TimerTrigger _timerTrigger;

        public PeriodicalHandler(ILogFactory logFactory, IMetricsChecker metricsChecker)
        {
            _log = logFactory.CreateLog(this);
            _metricsChecker = metricsChecker;
            _timerTrigger = new TimerTrigger(nameof(PeriodicalHandler), TimeSpan.FromSeconds(60), logFactory);
            _timerTrigger.Triggered += Execute;
        }

        public void Start()
        {
            _timerTrigger.Start();
        }

        public void Stop()
        {
            _timerTrigger.Stop();
        }

        public void Dispose()
        {
            _timerTrigger.Stop();
            _timerTrigger.Dispose();
        }

        private async Task Execute(
            ITimerTrigger timer,
            TimerTriggeredHandlerArgs args,
            CancellationToken cancellationToken)
        {
            try
            {
                await _metricsChecker.CheckAllMetricsAsync();
            }
            catch (Exception e)
            {
                _log.Error(e);
            }
        }
    }
}
