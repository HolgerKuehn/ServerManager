namespace blog.dachs.ServerManager
{
    public class KeePass : GlobalExtention
    {

        public KeePass(Configuration configuration) : base(configuration)
        {
            string keePassConfigurationPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "blog.dachs", "ServerManager", "ServerManager.kdbx");

            // KPScript -c:GenPw -count:1 -profile:"ServerManager"
            // KPScript -c:ListEntries "C:\ProgramData\blog.dachs\ServerManager\ServerManager.Videoarchiv.TMdb.kdbx" -pw:<Password> -ref-Title:"[tmdbid-233639]"
            // KPScript -c:AddEntry "C:\ProgramData\blog.dachs\ServerManager\ServerManager.Videoarchiv.TMdb.kdbx" -pw:<Password> -Title:"[tmdbid-233639]" -UserName:"" -Password:"R5fDlyUBzKhl98v8sHymn21DlMBKsAAaOcJxNd72X0nwIqbGM3vQERLSV6LX3tMY"
        }
    }
}
