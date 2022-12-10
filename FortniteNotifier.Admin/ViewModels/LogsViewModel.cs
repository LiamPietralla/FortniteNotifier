namespace FortniteNotifier.Admin.ViewModels
{
    public class LogsViewModel
    {
        public List<LogFileViewModel> LogFiles { get; set; } = null!;
        public string? SearchTermn { get; set; }
    }

    public class LogFileViewModel
    {
        public string Name { get; set; } = null!;
        //public string Type { get; set; } = string.Empty;
        public DateTime LastWriteTime { get; set; }
        public string SizeKB { get; set; } = null!;
    }
}