namespace blog.dachs.ServerManager
{
    public abstract class ThreadWorker : GlobalExtention
    {
        public ThreadWorker(Configuration configuration) : base(configuration)
        {
        }

        public abstract void Work();
    }
}
