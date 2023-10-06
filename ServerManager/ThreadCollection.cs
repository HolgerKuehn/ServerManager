namespace blog.dachs.ServerManager
{
    public class ThreadCollection
    {
        private static List<ThreadWorker> threadWorker = new List<ThreadWorker>();

        private static List<ThreadWorker> ThreadWorker
        {
            get { return threadWorker; }
            set { threadWorker = value; }
        }

        public void ThreadDynDns(Log log)
        {
            ThreadDynDns threadDynDns = new ThreadDynDns(log);
            Thread threadDnsThread = new Thread(threadDynDns.Work);
            threadDnsThread.Name = "ThreadDynDns";
            threadDnsThread.Start();

            ThreadWorker.Add(threadDynDns);
        }

    }
}
