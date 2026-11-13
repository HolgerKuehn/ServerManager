namespace blog.dachs.ServerManager
{
    public class KeePassEntry
    {
        private String path;
        private String title;
        private String userName;
        private String password;

        public KeePassEntry(String title, String path)
        {
            this.Path = path;
            this.Title = title;
            this.UserName = String.Empty;
            this.Password = String.Empty;
        }

        public String Path
        {
            get { return this.path; }
            set { this.path = value; }
        }

        public String UserName
        {
            get { return this.userName; }
            set { this.userName = value; }
        }

        public String Title
        {
            get { return this.title; }
            set { this.title = value; }
        }

        public String Password
        {
            get { return this.password; }
            set { this.password = value; }
        }
    }
}
