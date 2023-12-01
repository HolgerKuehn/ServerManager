namespace blog.dachs.ServerManager
{
    using System.Diagnostics;
    using System.Text;

    public class CommandLine : GlobalExtention
    {
        private Process process;

        public CommandLine(Configuration configuration) : base(configuration)
        {
            this.Process = new Process();
        }

        public Process Process
        {
            get { return this.process; }
            set { this.process = value; }
        }

        public List<string> Command(ProcessStartInfo processStartInfo)
        {
            Process serverManager = Process.GetCurrentProcess();
            ProcessPriorityClass processPriorityClass = serverManager.PriorityClass;

            string result = string.Empty;
            List<string> results = new List<string>();

            // set default values for list-output
            this.Process.StartInfo.FileName = processStartInfo.FileName;
            this.Process.StartInfo.Arguments = processStartInfo.Arguments;
            this.Process.StartInfo.WorkingDirectory = processStartInfo.WorkingDirectory;

            this.Process.StartInfo.UseShellExecute = false;
            this.Process.StartInfo.RedirectStandardOutput = true;
            this.Process.StartInfo.StandardOutputEncoding = Encoding.UTF8;
            this.Process.StartInfo.CreateNoWindow = true;

            try
            {
                serverManager.PriorityClass = ProcessPriorityClass.Idle;
                this.Process.Start();
                serverManager.PriorityClass = processPriorityClass;

                while (!Process.StandardOutput.EndOfStream)
                {
                    result = this.Process.StandardOutput.ReadLine();
                    if (result != null)
                        results.Add(result);
                }

                Process.WaitForExit();
            }
            catch (Exception ex)
            {

            }
            finally
            {
                serverManager.PriorityClass = processPriorityClass;
            }

            return results;
        }
    }
}
