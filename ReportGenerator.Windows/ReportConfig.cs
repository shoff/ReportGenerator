namespace ReportGenerator.Windows
{
    public class ReportConfig
    {
        public string Reports { get; set; }
        public string TargetDirectory { get; set;}
        public string HistoryDirectory { get; set; }
        public string[] ReportTypes { get; set; }
        public string[] SourceDirectories { get; set; }
        public string[] Filters { get; set; }
        public string VerbosityLevel { get; set; }

    }
}