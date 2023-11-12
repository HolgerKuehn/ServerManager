namespace blog.dachs.ServerManager
{
    public class Filesystem : GlobalExtention
    {
        public Filesystem(Configuration configuration) : base(configuration)
        {
        }

        public List<string> GetFilesFromPath(string path)
        {
            foreach (string file in Directory.EnumerateFiles(path, "*.*", SearchOption.AllDirectories))
            {
                Console.WriteLine(file);
            }

            return new List<string>();
        }
    }
}
