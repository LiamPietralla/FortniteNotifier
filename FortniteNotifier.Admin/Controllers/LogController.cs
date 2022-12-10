using FortniteNotifier.Admin.Helpers;
using FortniteNotifier.Admin.ViewModels;
using FortniteNotifier.Shared.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Serilog.Core;
using System;

namespace FortniteNotifier.Admin.Controllers
{
    [Authorize]
    public class LogController : Controller
    {
        private readonly ConfigHelper _configHelper;

        public LogController(IConfiguration configuration)
        {
            _configHelper = new ConfigHelper(configuration);
        }

        public IActionResult Index(bool loadAll = false)
        {
            try
            {
                // Get the log directory
                string logDirectory = Path.GetDirectoryName(_configHelper.LogLocation) ?? throw new Exception("Unable to get directory name");

                // Get a list of all the log files from the log directory
                string[] logFiles = Directory.GetFiles(logDirectory, "*log*");

                // Get the file info for each file
                List<FileInfo> logFileInfo = new();
                foreach (string file in logFiles)
                {
                    logFileInfo.Add(new FileInfo(file));
                }

                // Sort the files by last write time
                logFileInfo.Sort((x, y) => y.LastWriteTime.CompareTo(x.LastWriteTime));

                // If load all is not set only take the top 10
                if (!loadAll)
                {
                    logFileInfo = logFileInfo.Take(10).ToList();
                }

                // Create the view model
                LogsViewModel vm = new()
                {
                    LogFiles = new List<LogFileViewModel>()
                };

                // Add each file to the view model
                foreach (FileInfo file in logFileInfo)
                {
                    vm.LogFiles.Add(new LogFileViewModel()
                    {
                        Name = file.Name,
                        LastWriteTime = file.LastWriteTime,
                        SizeKB = (file.Length / 1000).ToString()
                    });
                }

                return View(vm);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while navigating to the log page.");
                return RedirectToAction("Index", "Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> DownloadLog(string logFileName)
        {
            try
            {
                Log.Information("Downloading log file: {logFileName}", logFileName);

                // Get the log directory
                string logDirectory = Path.GetDirectoryName(_configHelper.LogLocation) ?? throw new Exception("Unable to get directory name");

                // Get the full file path
                string path = CrossPlatformHelper.PathCombine(logDirectory, logFileName);

                // Get the file contents with read write share enabled
                using FileStream fileStream = new(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                byte[] fileBytes = new byte[fileStream.Length];
                await fileStream.ReadAsync(fileBytes.AsMemory(0, (int)fileStream.Length));

                // Return the file
                return File(fileBytes, "application/octet-stream", logFileName);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while dowloading the log file: {fileName}.", logFileName);
                return RedirectToAction("Index", "Error");
            }
        }
    }
}