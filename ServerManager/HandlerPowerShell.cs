namespace blog.dachs.ServerManager
{
    using System.Diagnostics;

    public class HandlerPowerShell : GlobalExtention
    {
        private Process process;

        public HandlerPowerShell(Configuration configuration) : base(configuration)
        {
            this.Process = new Process();
        }

        public Process Process
        {
            get { return this.process; }
            set { this.process = value; }
        }

        public List<string> Command(string command)
        {
            string result = string.Empty;
            List<string> results = new List<string>();

            this.Process.StartInfo.FileName = "powershell.exe";
            this.Process.StartInfo.UseShellExecute = false;
            this.Process.StartInfo.RedirectStandardOutput = true;
            this.Process.StartInfo.CreateNoWindow = true;
            this.Process.StartInfo.Arguments = "-Command \"" + command.Replace("\"", "\'").ReplaceLineEndings("; ") + "\"";

            try
            {
                this.Process.Start();

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

            return results;
        }
    }
}
