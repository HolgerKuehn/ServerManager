namespace blog.dachs.ServerManager
{
    using System.Text;

    public class LogFile : Log
    {
        private String logFilePath;

        public LogFile(Configuration configuration) : base(configuration)
        {
            this.logFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "blog", "dachs", "ServerManager", "ServerManager.log");
        }

        public override void WriteLog(LogEntry logEntry)
        {
            if (this.Configuration.MinimumLogSeverity <= logEntry.LogSeverity)
            {
                using (StreamWriter writer = new StreamWriter(this.logFilePath, true, Encoding.UTF8))
                {
                    String line = String.Empty;

                    line += DateTime.Today + ";";
                    line += logEntry.LogSeverity.ToString() + ";";
                    line += logEntry.LogOrigin.ToString() + ";";
                    line += logEntry.LogMessage;

                    writer.WriteLine(line);
                }

                GC.Collect();
            }
        }
    }
}
