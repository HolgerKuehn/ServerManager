namespace blog.dachs.ServerManager
{
    public class KeePassEntry
    {
        private string path;
        private string title;
        private string userName;
        private string password;

        public KeePassEntry(string title, string path)
        {
            this.Path = path;
            this.Title = title;
            this.UserName = string.Empty;
            this.Password = string.Empty;
        }

        public string Path
        {
            get { return this.path; }
            set { this.path = value; }
        }

        public string UserName
        {
            get { return this.userName; }
            set { this.userName = value; }
        }

        public string Title
        {
            get { return this.title; }
            set { this.title = value; }
        }

        public string Password
        {
            get { return this.password; }
            set { this.password = value; }
        }
    }
}
