namespace blog.dachs.ServerManager
{
    using blog.dachs.ServerManager.DynDNS;
    using System.Net;
    using System.Net.Http.Headers;
    using System.Net.Sockets;
    using System.Text;

    public class WebRequest : GlobalExtention
    {
        private Dictionary<DynDnsIpAddressVersion, HttpClient> httpClientCollection;

        public WebRequest(Configuration configuration) : base(configuration)
        {
            this.HttpClientCollection = new Dictionary<DynDnsIpAddressVersion, HttpClient>();
            this.HttpClientCollection[DynDnsIpAddressVersion.IPv4] = this.GetHttpClient(DynDnsIpAddressVersion.IPv4);
            this.HttpClientCollection[DynDnsIpAddressVersion.IPv6] = this.GetHttpClient(DynDnsIpAddressVersion.IPv6);
        }

        public Dictionary<DynDnsIpAddressVersion, HttpClient> HttpClientCollection
        {
            get { return this.httpClientCollection; }
            set { this.httpClientCollection = value; }
        }

        public string Request(string url, DynDnsIpAddressVersion ipAddressVersion = DynDnsIpAddressVersion.IPv6)
        {
            return this.Request(url, new NetworkCredential("", ""), ipAddressVersion);
        }

        public string Request(string url, NetworkCredential networkCredential, DynDnsIpAddressVersion ipAddressVersion = DynDnsIpAddressVersion.IPv6)
        {
            string response = string.Empty;
            HttpClient httpClient = this.HttpClientCollection[ipAddressVersion];

            if (networkCredential.UserName != "")
            {
                //string networkCredentialString = networkCredential.UserName /*.Decrypt()*/;
                //networkCredentialString = networkCredential.Password /*.Decrypt()*/;
                byte[] networkCredentialByteArray = Encoding.ASCII.GetBytes(networkCredential.UserName + ":" + networkCredential.Password);
                AuthenticationHeaderValue networkCredentialHeader = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(networkCredentialByteArray));
                httpClient.DefaultRequestHeaders.Authorization = networkCredentialHeader;
            }

            try
            {
                response = httpClient.GetStringAsync(url).Result;
            }
            catch (Exception)
            {
            }

            return response;
        }

        private HttpClient GetHttpClient(DynDnsIpAddressVersion ipAddressVersion)
        {
            HttpClient httpClient = GetHttpClient(AddressFamily.InterNetworkV6);

            if (ipAddressVersion == DynDnsIpAddressVersion.IPv4)
            {
                httpClient = GetHttpClient(AddressFamily.InterNetwork);
            }

            httpClient.DefaultRequestHeaders.Add("User-Agent", "blog.dachs.ServerManager V1 DynDnsClient");
            return httpClient;
        }

        private HttpClient GetHttpClient(AddressFamily addressFamily)
        {
            var client = new HttpClient(new SocketsHttpHandler()
            {
                ConnectCallback = async (context, cancellationToken) =>
                {
                    // Use DNS to look up the IP addresses of the target host:
                    // - IP v4: AddressFamily.InterNetwork
                    // - IP v6: AddressFamily.InterNetworkV6
                    // - IP v4 or IP v6: AddressFamily.Unspecified
                    // note: this method throws a SocketException when there is no IP address for the host
                    var entry = await Dns.GetHostEntryAsync(context.DnsEndPoint.Host, addressFamily, cancellationToken);

                    // Open the connection to the target host/port
                    var socket = new Socket(SocketType.Stream, ProtocolType.Tcp);

                    // Turn off Nagle's algorithm since it degrades performance in most HttpClient scenarios.
                    socket.NoDelay = true;

                    try
                    {
                        await socket.ConnectAsync(entry.AddressList, context.DnsEndPoint.Port, cancellationToken);

                        // If you want to choose a specific IP address to connect to the server
                        // await socket.ConnectAsync(
                        //    entry.AddressList[Random.Shared.Next(0, entry.AddressList.Length)],
                        //    context.DnsEndPoint.Port, cancellationToken);

                        // Return the NetworkStream to the caller
                        return new NetworkStream(socket, ownsSocket: true);
                    }
                    catch
                    {
                        socket.Dispose();
                        throw;
                    }
                }
            });

            return client;
        }
    }
}
