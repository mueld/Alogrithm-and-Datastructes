using Microsoft.Extensions.Hosting;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Bruderer.Core.Infrastructure.Services.BackgroundServices
{
    public class BackgroundLoopService : BackgroundService
    {
        #region fields

        private Stopwatch _Stopwatch = new Stopwatch();
        private ulong _LoopCounter = 0;

        private Func<CancellationToken, long, Task> _Function = null;

        #endregion
        #region ctor

        public BackgroundLoopService(Func<CancellationToken, long, Task> function)
        {
            _Function = function;

            AverageLoopTime = Period.TotalMilliseconds;
        }

        #endregion
        #region props

        public TimeSpan Period { get; set; } = TimeSpan.FromMilliseconds(100);

        public int MaxElapsedPeriodTime { get; set; } = 200;

        public double AverageLoopTime { get; private set; }

        #endregion
        #region methods

        protected override async Task ExecuteAsync(CancellationToken stopToken)
        {
            while (!stopToken.IsCancellationRequested)
            {
                _Stopwatch.Restart();
                await _Function(stopToken, DateTime.Now.Ticks);

                if (_Stopwatch.ElapsedMilliseconds < MaxElapsedPeriodTime)
                {
                    _LoopCounter++;
                    AverageLoopTime = ((AverageLoopTime * (_LoopCounter - 1)) + _Stopwatch.ElapsedMilliseconds) / _LoopCounter;
                }

                var newPeriod = Period - _Stopwatch.Elapsed;
                if (newPeriod.Ticks < 0)
                    newPeriod = TimeSpan.FromTicks(0);

                await Task.Delay(newPeriod, stopToken);
            }
        }

        #endregion
    }
}
