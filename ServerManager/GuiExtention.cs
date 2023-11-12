namespace blog.dachs.ServerManager
{
    public partial class GuiExtention : Form
    {
        private Configuration configuration;
        private Database handlerDatabase;
        private PowerShell handlerPowerShell;
        private WebRequest handlerWebRequest;

        public GuiExtention(Configuration configuration) : base()
        {
            Configuration = configuration;
        }

        public Configuration Configuration
        {
            get { return this.configuration; }
            set { this.configuration = value; }
        }

        public Database HandlerDatabase
        {
            get
            {
                if (this.handlerDatabase == null)
                    this.handlerDatabase = Database.GetHandlerDatabase(this.Configuration);

                return this.handlerDatabase;
            }
            set { this.handlerDatabase = value; }
        }

        public PowerShell HandlerPowerShell
        {
            get
            {
                if (this.handlerPowerShell == null)
                    this.handlerPowerShell = new PowerShell(this.Configuration);

                return this.handlerPowerShell;
            }
            set { this.handlerPowerShell = value; }
        }

        public WebRequest HandlerWebRequest
        {
            get
            {
                if (this.handlerWebRequest == null)
                    this.handlerWebRequest = new WebRequest(this.Configuration);

                return this.handlerWebRequest;
            }
            set { this.handlerWebRequest = value; }
        }
    }
}
