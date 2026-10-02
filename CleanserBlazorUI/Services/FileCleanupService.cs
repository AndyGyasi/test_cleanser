using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

public class FileCleanupService : BackgroundService
{
    private readonly ILogger<FileCleanupService> _logger;
    private readonly string _tempFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "temp");
    private DateTime _lastRunDate = DateTime.MinValue;

    public FileCleanupService(ILogger<FileCleanupService> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("FileCleanupService started");
        
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var now = AppClock.Now;
                // Check if current time is between 1 AM and 4 AM, and hasn't run today
                if (now.Hour >= 1 && now.Hour < 4 && _lastRunDate.Date != now.Date)
                {
                    _logger.LogInformation("Starting scheduled file cleanup at {Time}", now);
                    CleanupOldFiles();
                    _lastRunDate = now.Date;
                    _logger.LogInformation("Scheduled file cleanup completed");
                }
                // Sleep for 10 minutes to avoid tight loop
                await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during file cleanup service execution");
                // Continue running even if there's an error
                await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken);
            }
        }
        
        _logger.LogInformation("FileCleanupService stopped");
    }

    private void CleanupOldFiles()
    {
        if (!Directory.Exists(_tempFolderPath))
        {
            _logger.LogDebug("Temp folder does not exist: {TempPath}", _tempFolderPath);
            return;
        }

        try
        {
            var files = Directory.GetFiles(_tempFolderPath, "*.xlsx"); // Target Excel files
            _logger.LogInformation("Found {FileCount} Excel files to clean up in {TempPath}", files.Length, _tempFolderPath);
            
            int deletedCount = 0;
            foreach (var file in files)
            {
                try
                {
                    var lastModified = File.GetLastWriteTime(file);
                    var fileInfo = new FileInfo(file);
                    
                    File.Delete(file);
                    deletedCount++;
                    
                    _logger.LogInformation("Deleted file: {FileName} (Size: {FileSize} bytes, Last Modified: {LastModified})", 
                        Path.GetFileName(file), fileInfo.Length, lastModified);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to delete file: {FileName}", file);
                }
            }
            
            _logger.LogInformation("File cleanup completed. Deleted {DeletedCount} out of {TotalCount} files", deletedCount, files.Length);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during file cleanup in {TempPath}", _tempFolderPath);
        }
    }
}
