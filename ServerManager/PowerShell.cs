using System.Diagnostics;

namespace blog.dachs.ServerManager
{
    public class PowerShell : GlobalExtention
    {
        public PowerShell(Configuration configuration) : base(configuration)
        {
        }

        public List<string> Command(string command)
        {
            ProcessStartInfo processStartInfo = new ProcessStartInfo();
            processStartInfo.FileName = "powershell.exe";
            processStartInfo.Arguments = "-Command \"" + command.Replace("\"", "\'").ReplaceLineEndings("; ") + "\"";

            return this.CommandLine.Command(processStartInfo);
        }
    }
}
