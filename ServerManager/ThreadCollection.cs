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

        public void ThreadDynDns(Configuration configuration)
        {
            ThreadDynDns threadDynDns = new ThreadDynDns(configuration);
            Thread threadDnsThread = new Thread(threadDynDns.Work);
            threadDnsThread.Name = "ThreadDynDns";
            threadDnsThread.Start();

            ThreadWorker.Add(threadDynDns);
        }

    }
}
