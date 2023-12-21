using System.Diagnostics;

namespace blog.dachs.ServerManager
{
    public class PowerShell : GlobalExtention
    {
        public PowerShell(Configuration configuration) : base(configuration)
        {
        }

        public ProcessOutput ExecuteCommand(string command)
        {
            ProcessStartInfo processStartInfo = new ProcessStartInfo();
            processStartInfo.FileName = "powershell.exe";
            processStartInfo.Arguments = "-Command \"$OutputEncoding = [console]::InputEncoding = [console]::OutputEncoding = [System.Text.UTF8Encoding]::new(); " + command.Replace("\"", "\'").ReplaceLineEndings("; ") + "\"";

            return this.CommandLine.ExecuteCommand(processStartInfo);
        }
    }
}
