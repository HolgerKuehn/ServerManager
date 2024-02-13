/// <summary>
/// Namespace for ServiceManager
/// Copyright Holger Kühn, 2023
/// </summary>
namespace blog.dachs.ServerManager
{
    using blog.dachs.ServerManager.Backup;
    using blog.dachs.ServerManager.DynDNS;

    /// <summary>
    /// Manages all Thread used in ServerManager
    /// </summary>
    public class ThreadCollection : GlobalExtention
    {
        /// <summary>
        /// list of active threads
        /// </summary>
        private static List<ThreadWorker> threadWorkers = new List<ThreadWorker>();

        /// <summary>
        /// initializes ThreadCollection
        /// </summary>
        /// <param name="configuration"></param>
        public ThreadCollection(Configuration configuration) : base(configuration)
        {
        }

        /// <summary>
        /// Gets and sets list of active threads
        /// </summary>
        private static List<ThreadWorker> ThreadWorkers
        {
            get { return threadWorkers; }
            set { threadWorkers = value; }
        }

        public void ThreadBackup(Configuration configuration)
        {
            ThreadBackup threadBackup = new ThreadBackup(configuration);
            Thread threadBackupThread = new Thread(threadBackup.Work);
            threadBackupThread.Name = "ThreadBackup";
            threadBackupThread.Start();

            ThreadWorkers.Add(threadBackup);
        }

        /// <summary>
        /// starts DynDns Client
        /// </summary>
        /// <param name="configuration"></param>
        public void ThreadDynDns(Configuration configuration)
        {
            ThreadDynDns threadDynDns = new ThreadDynDns(configuration);
            Thread threadDnsThread = new Thread(threadDynDns.Work);
            threadDnsThread.Name = "ThreadDynDns";
            threadDnsThread.Start();

            ThreadWorkers.Add(threadDynDns);
        }

        public void ThreadFirewallRules(Configuration configuration)
        {
            ThreadFirewallRules threadFirewallRules = new ThreadFirewallRules(configuration);
            Thread threadFirewallRulesThread = new Thread(threadFirewallRules.Work);
            threadFirewallRulesThread.Name = "ThreadFirewallRules";
            threadFirewallRulesThread.Start();

            ThreadWorkers.Add(threadFirewallRules);
        }

        public void TerminateThreads()
        {
            foreach (ThreadWorker threadWorker in ThreadWorkers)
            {
                threadWorker.Terminate = true;
            }
        }
    }
}
