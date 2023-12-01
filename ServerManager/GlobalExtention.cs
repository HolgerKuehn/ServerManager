namespace blog.dachs.ServerManager
{
    public abstract class GlobalExtention
    {
        private Configuration configuration;
        private Database database;
        private CommandLine commandLine;
        private SevenZip sevenZip;
        private PowerShell powerShell;
        private WebRequest webRequest;
        private KeePassDatabaseCollection keePass;
        private bool changed;

        public GlobalExtention(Configuration configuration)
        {
            this.configuration = configuration;
        }

        public Configuration Configuration
        {
            get { return this.configuration; }
            set { this.configuration = value; }
        }

        public Database Database
        {
            get
            {
                if (this.database == null)
                    this.database = Database.GetHandlerDatabase(this.Configuration);

                return this.database;
            }
            set { this.database = value; }
        }

        public CommandLine CommandLine
        {
            get
            {
                if (this.commandLine == null)
                    this.commandLine = new CommandLine(this.Configuration);

                return this.commandLine;
            }
            set { this.commandLine = value; }
        }

        public SevenZip SevenZip
        {
            get
            {
                if (this.sevenZip == null)
                    this.sevenZip = new SevenZip(this.Configuration);

                return this.sevenZip;
            }
            set { this.sevenZip = value; }
        }

        public PowerShell PowerShell
        {
            get
            {
                if (this.powerShell == null)
                    this.powerShell = new PowerShell(this.Configuration);

                return this.powerShell;
            }
            set { this.powerShell = value; }
        }

        public WebRequest WebRequest
        {
            get
            {
                if (this.webRequest == null)
                    this.webRequest = new WebRequest(this.Configuration);

                return this.webRequest;
            }
            set { this.webRequest = value; }
        }

        public KeePassDatabaseCollection KeePass
        {
            get
            {
                if (this.keePass == null)
                    this.keePass = new KeePassDatabaseCollection(this.Configuration);

                return this.keePass;
            }
            set { this.keePass = value; }
        }

        public bool Changed
        {
            get { return this.changed; }
            set { this.changed = value; }
        }
    }
}
