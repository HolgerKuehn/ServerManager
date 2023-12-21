namespace blog.dachs.ServerManager
{
    using System.Data;
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

        public ProcessOutput ExecuteCommand(ProcessStartInfo processStartInfo)
        {
            string sqlCommandOriginal = string.Empty;
            string sqlCommandReplace = string.Empty;
            int processOutputRow = 0;

            Process serverManager = Process.GetCurrentProcess();
            ProcessPriorityClass processPriorityClass = serverManager.PriorityClass;
            ProcessOutput processOutput = new ProcessOutput(0);

            sqlCommandOriginal = this.Database.GetCommand(Command.CommandLine_Command_Insert);

            string result = string.Empty;

            // set default values for list-output
            this.Process.StartInfo.FileName = processStartInfo.FileName;
            this.Process.StartInfo.Arguments = processStartInfo.Arguments;
            this.Process.StartInfo.WorkingDirectory = processStartInfo.WorkingDirectory;

            this.Process.StartInfo.UseShellExecute = false;
            this.Process.StartInfo.StandardInputEncoding = Encoding.UTF8;
            this.Process.StartInfo.StandardOutputEncoding = Encoding.UTF8;
            this.Process.StartInfo.RedirectStandardInput = true;
            this.Process.StartInfo.RedirectStandardOutput = true;
            this.Process.StartInfo.CreateNoWindow = true;

            try
            {
                serverManager.PriorityClass = ProcessPriorityClass.Idle;
                this.Process.Start();
                processOutput = new ProcessOutput(this.Process.Handle);
                serverManager.PriorityClass = processPriorityClass;

                while (!Process.StandardOutput.EndOfStream)
                {
                    result = this.Process.StandardOutput.ReadLine();
                    if (result != null)
                    {
                        processOutputRow++;
                        
                        sqlCommandReplace = sqlCommandOriginal;
                        sqlCommandReplace = sqlCommandReplace.Replace("<ProcessOutputHandle>", processOutput.handle.ToString());
                        sqlCommandReplace = sqlCommandReplace.Replace("<ProcessOutputRandomNumber>", processOutput.randomNumber);
                        sqlCommandReplace = sqlCommandReplace.Replace("<ProcessOutputRow>", processOutputRow.ToString());
                        sqlCommandReplace = sqlCommandReplace.Replace("<ProcessOutputText>", result);
                        this.Database.ExecuteCommand(sqlCommandReplace);
                    }
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

            return processOutput;
        }

        public string? GetProcessOutput(ProcessOutput processOutput, int row = 0, string filter = "")
        {
            string sqlCommandOriginal;
            string sqlCommandProcess;
            string? processOutputLine = null;

            sqlCommandOriginal = this.Database.GetCommand(Command.CommandLine_Command_Select);
            sqlCommandProcess = sqlCommandOriginal;
            sqlCommandProcess = sqlCommandProcess.Replace("<ProcessOutputHandle>", processOutput.handle.ToString());
            sqlCommandProcess = sqlCommandProcess.Replace("<ProcessOutputRandomNumber>", processOutput.randomNumber);
            
            DataRow dataRow = this.Database.GetDataRow(sqlCommandProcess, row, filter);
           
            if (dataRow != null)
                processOutputLine = dataRow[0].ToString();

            return processOutputLine;
        }

        public void DeleteProcessOutput(ProcessOutput processOutput)
        {
            string sqlCommand;
            
            sqlCommand = this.Database.GetCommand(Command.CommandLine_Command_Delete);
            sqlCommand = sqlCommand.Replace("<ProcessOutputHandle>", processOutput.handle.ToString());
            sqlCommand = sqlCommand.Replace("<ProcessOutputRandomNumber>", processOutput.randomNumber);

            this.Database.ExecuteCommand(sqlCommand);
        }
    }
}
