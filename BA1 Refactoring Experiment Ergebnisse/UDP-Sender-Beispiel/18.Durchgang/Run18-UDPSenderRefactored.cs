using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

public class Sender
{
    private readonly int _port;
    private readonly string? _hostName;
    private readonly bool _useBroadcast;
    private readonly string? _groupAddress;
    private readonly bool _useIpv6;

    public Sender(int port, string? hostname, bool useBroadcast, string? groupAddress, bool useIpv6)
    {
        _port = port;
        _hostName = hostname;
        _useBroadcast = useBroadcast;
        _groupAddress = groupAddress;
        _useIpv6 = useIpv6;
    }

    public async Task RunAsync()
    {
        try
        {
            IPEndPoint? endpoint = await GetReceiverIPEndPointAsync();

            if (endpoint is null)
            {
                return;
            }

            using UdpClient client = InitializeUdpClient();
            await ProcessMessagesAsync(client, endpoint);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    private UdpClient InitializeUdpClient()
    {
        UdpClient client = new();
        client.EnableBroadcast = _useBroadcast;

        if (_groupAddress != null)
        {
            client.JoinMulticastGroup(IPAddress.Parse(_groupAddress));
        }

        return client;
    }

    private async Task ProcessMessagesAsync(UdpClient client, IPEndPoint endpoint)
    {
        bool completed = false;
        do
        {
            Console.WriteLine($"{Environment.NewLine}Enter a message or 'bye' to exit");
            string? input = Console.ReadLine();

            if (input is null)
            {
                continue;
            }

            completed = input.Equals("bye", StringComparison.CurrentCultureIgnoreCase);

            byte[] datagram = Encoding.UTF8.GetBytes(input);
            int sent = await client.SendAsync(datagram, datagram.Length, endpoint);

            Console.WriteLine($"Sent datagram using local EP {client.Client.LocalEndPoint} to {endpoint}");
        } while (!completed);

        if (_groupAddress != null)
        {
            client.DropMulticastGroup(IPAddress.Parse(_groupAddress));
        }
    }

    private async Task<IPEndPoint?> GetReceiverIPEndPointAsync()
    {
        try
        {
            if (_useBroadcast)
            {
                return new IPEndPoint(IPAddress.Broadcast, _port);
            }

            if (_hostName != null)
            {
                return await GetIPEndPointFromHostNameAsync();
            }

            if (_groupAddress != null)
            {
                return new IPEndPoint(IPAddress.Parse(_groupAddress), _port);
            }

            throw new InvalidOperationException($"{nameof(_hostName)}, {nameof(_useBroadcast)}, or {nameof(_groupAddress)} must be set");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return null;
        }
    }

    private async Task<IPEndPoint?> GetIPEndPointFromHostNameAsync()
    {
        try
        {
            IPHostEntry hostEntry = await Dns.GetHostEntryAsync(_hostName);
            IPAddress? address = GetIPAddressBasedOnVersion(hostEntry.AddressList);

            if (address == null)
            {
                string ipversion = _useIpv6 ? "IPv6" : "IPv4";
                Console.WriteLine($"No {ipversion} address for {_hostName}");
                return null;
            }

            return new IPEndPoint(address, _port);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return null;
        }
    }

    private IPAddress? GetIPAddressBasedOnVersion(IPAddress[] addressList)
    {
        return _useIpv6
            ? addressList.FirstOrDefault(a => a.AddressFamily == AddressFamily.InterNetworkV6)
            : addressList.FirstOrDefault(a => a.AddressFamily == AddressFamily.InterNetwork);
    }
}