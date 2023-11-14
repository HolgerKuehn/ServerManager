namespace blog.dachs.ServerManager
{
    public class ThreadCollection : GlobalExtention
    {
        private static List<ThreadWorker> threadWorker = new List<ThreadWorker>();

        public ThreadCollection(Configuration configuration) : base(configuration)
        {
        }

        private static List<ThreadWorker> ThreadWorker
        {
            get { return threadWorker; }
            set { threadWorker = value; }
        }

        public void ThreadBackup(Configuration configuration)
        {
            ThreadBackup threadBackup = new ThreadBackup(configuration);
            Thread threadBackupThread = new Thread(threadBackup.Work);
            threadBackupThread.Name = "ThreadBackup";
            threadBackupThread.Start();

            ThreadWorker.Add(threadBackup);
        }

        public void ThreadDynDns(Configuration configuration)
        {
            ThreadDynDns threadDynDns = new ThreadDynDns(configuration);
            Thread threadDnsThread = new Thread(threadDynDns.Work);
            threadDnsThread.Name = "ThreadDynDns";
            threadDnsThread.Start();

            ThreadWorker.Add(threadDynDns);
        }

        public void ThreadFirewallRuleBaseProperties(Configuration configuration, DynDnsServerLocal server)
        {
            ThreadFirewallRuleBaseProperties threadFirewallRuleBaseProperties = new ThreadFirewallRuleBaseProperties(configuration, server);
            Thread threadFirewallRuleBasePropertiesThread = new Thread(threadFirewallRuleBaseProperties.Work);
            threadFirewallRuleBasePropertiesThread.Name = "ThreadFirewallRuleBaseProperties";
            threadFirewallRuleBasePropertiesThread.Start();

            ThreadWorker.Add(threadFirewallRuleBaseProperties);
        }
    }
}
