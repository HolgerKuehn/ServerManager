namespace blog.dachs.ServerManager
{
    public class KeePassEntry
    {
        private string path = string.Empty;
        private string title = string.Empty;
        private string password = string.Empty;

        public KeePassEntry(string title, string path)
        {
            this.Path = path;
            this.Title = title;
        }

        public string Path
        {
            get { return this.path; }
            set { this.path = value; }
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
