namespace blog.dachs.ServerManager
{
    public partial class GuiExtention : Form
    {
        private Configuration configuration;
        private HandlerDatabase handlerDatabase;
        private HandlerPowerShell handlerPowerShell;
        private HandlerWebRequest handlerWebRequest;

        public GuiExtention(Configuration configuration) : base()
        {
            Configuration = configuration;
        }

        public Configuration Configuration
        {
            get { return this.configuration; }
            set { this.configuration = value; }
        }

        public HandlerDatabase HandlerDatabase
        {
            get
            {
                if (this.handlerDatabase == null)
                    this.handlerDatabase = HandlerDatabase.GetHandlerDatabase(this.Configuration);

                return this.handlerDatabase;
            }
            set { this.handlerDatabase = value; }
        }

        public HandlerPowerShell HandlerPowerShell
        {
            get
            {
                if (this.handlerPowerShell == null)
                    this.handlerPowerShell = new HandlerPowerShell(this.Configuration);

                return this.handlerPowerShell;
            }
            set { this.handlerPowerShell = value; }
        }

        public HandlerWebRequest HandlerWebRequest
        {
            get
            {
                if (this.handlerWebRequest == null)
                    this.handlerWebRequest = new HandlerWebRequest(this.Configuration);

                return this.handlerWebRequest;
            }
            set { this.handlerWebRequest = value; }
        }
    }
}
