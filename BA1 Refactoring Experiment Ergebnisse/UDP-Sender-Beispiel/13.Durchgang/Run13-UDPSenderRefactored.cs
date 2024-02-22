using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

public class Sender : IDisposable
{
    private readonly int _port;
    private readonly string? _hostName;
    private readonly bool _useBroadcast;
    private readonly string? _groupAddress;
    private readonly bool _useIpv6;

    private UdpClient _udpClient;

    public Sender(int port, string? hostname, bool useBroadcast, string? groupAddress, bool useIpv6)
    {
        _port = port;
        _hostName = hostname;
        _useBroadcast = useBroadcast;
        _groupAddress = groupAddress;
        _useIpv6 = useIpv6;
        _udpClient = new UdpClient();
    }

    public async Task RunAsync()
    {
        IPEndPoint? endpoint = await GetReceiverIPEndPointAsync();

        if (endpoint is null)
        {
            return;
        }

        try
        {
            _udpClient.EnableBroadcast = _useBroadcast;

            if (_groupAddress != null)
            {
                _udpClient.JoinMulticastGroup(IPAddress.Parse(_groupAddress));
            }

            bool completed = false;
            do
            {
                Console.WriteLine($"{Environment.NewLine}Enter a message or \"bye\" to exit");
                string? input = Console.ReadLine();

                if (input is null)
                {
                    continue;
                }

                completed = input.Equals("bye", StringComparison.CurrentCultureIgnoreCase);

                byte[] datagram = Encoding.UTF8.GetBytes(input);
                int sent = await _udpClient.SendAsync(datagram, datagram.Length, endpoint);

                Console.WriteLine($"Sent datagram using local EP {_udpClient.Client.LocalEndPoint} to {endpoint}");
            } while (!completed);

            if (_groupAddress != null)
            {
                _udpClient.DropMulticastGroup(IPAddress.Parse(_groupAddress));
            }
        }
        catch (SocketException ex)
        {
            Console.WriteLine($"Error {ex.Message}");
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
            else if (_hostName != null)
            {
                IPHostEntry hostEntry = await Dns.GetHostEntryAsync(_hostName);
                IPAddress? address = GetIpAddress(hostEntry);

                if (address == null)
                {
                    PrintNoIpAddressMessage();
                    return null;
                }

                return new IPEndPoint(address, _port);
            }
            else if (_groupAddress != null)
            {
                return new IPEndPoint(IPAddress.Parse(_groupAddress), _port);
            }
            else
            {
                throw new InvalidOperationException($"{nameof(_hostName)}, {nameof(_useBroadcast)}, or {nameof(_groupAddress)} must be set");
            }
        }
        catch (SocketException ex)
        {
            Console.WriteLine($"Error {ex.Message}");
            return null;
        }
    }

    private IPAddress? GetIpAddress(IPHostEntry hostEntry)
    {
        return _useIpv6
            ? hostEntry.AddressList.FirstOrDefault(a => a.AddressFamily == AddressFamily.InterNetworkV6)
            : hostEntry.AddressList.FirstOrDefault(a => a.AddressFamily == AddressFamily.InterNetwork);
    }

    private void PrintNoIpAddressMessage()
    {
        string ipVersion = _useIpv6 ? "IPv6" : "IPv4";
        Console.WriteLine($"No {ipVersion} address for {_hostName}");
    }

    public void Dispose()
    {
        _udpClient.Dispose();
    }
}