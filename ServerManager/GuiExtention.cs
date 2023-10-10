namespace blog.dachs.ServerManager
{
    public partial class GuiExtention : Form
    {
        private Configuration configuration;
        private HandlerDatabase handlerDatabase;

        public GuiExtention(Configuration configuration) : base()
        {
            Configuration = configuration;
            this.handlerDatabase = HandlerDatabase.GetHandlerDatabase();
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
    }
}
