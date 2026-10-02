using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CleanserBlazorUI.Services
{
    public class ScheduledTaskService : IHostedService, IDisposable
    {
        private Timer? _timer;

        public Task StartAsync(CancellationToken cancellationToken)
        {
            // Schedule the task to run every minute
            _timer = new Timer(PerformScheduledTask, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
            return Task.CompletedTask;
        }

        private void PerformScheduledTask(object? state)
        {
            // Add your scheduled task logic here
            Console.WriteLine($"Scheduled task executed at: {AppClock.Now}");
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
