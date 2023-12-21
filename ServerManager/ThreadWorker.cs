namespace blog.dachs.ServerManager
{
    public abstract class ThreadWorker : GlobalExtention
    {
        private bool terminate;

        public ThreadWorker(Configuration configuration) : base(configuration)
        {
            this.Terminate = false;
        }

        public bool Terminate
        {
            get { return this.terminate; }
            set { this.terminate = value; }
        }

        public abstract void Work();
    }
}
