namespace blog.dachs.ServerManager
{
    public abstract class GlobalExtention
    {
        private Configuration configuration;
        private HandlerDatabase handlerDatabase;
        private HandlerPowerShell handlerPowerShell;

        public GlobalExtention(Configuration configuration)
        {
            this.configuration = configuration;
            this.handlerDatabase = HandlerDatabase.GetHandlerDatabase();
            this.handlerPowerShell = new HandlerPowerShell(configuration);
        }

        public Configuration Configuration
        {
            get { return this.configuration; }
            set { this.configuration = value; }
        }

        public HandlerDatabase HandlerDatabase
        {
            get { return this.handlerDatabase; }
            set { this.handlerDatabase = value; }
        }

        public HandlerPowerShell HandlerPowerShell
        {
            get { return this.handlerPowerShell; }
            set { this.handlerPowerShell = value; }
        }
    }
}
