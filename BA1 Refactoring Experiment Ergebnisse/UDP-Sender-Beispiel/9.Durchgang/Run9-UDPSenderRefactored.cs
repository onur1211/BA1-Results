using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

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
        IPEndPoint? endpoint = await GetReceiverIPEndPointAsync();

        if (endpoint is null)
        {
            return;
        }

        try
        {
            using UdpClient client = new() { EnableBroadcast = _useBroadcast };

            if (_groupAddress != null)
            {
                client.JoinMulticastGroup(IPAddress.Parse(_groupAddress));
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

                completed = input.Equals("bye", StringComparison.OrdinalIgnoreCase);

                byte[] datagram = Encoding.UTF8.GetBytes(input);
                int sent = await client.SendAsync(datagram, datagram.Length, endpoint);

                Console.WriteLine($"Sent datagram using local EP {client.Client.LocalEndPoint} to {endpoint}");
            } while (!completed);

            if (_groupAddress != null)
            {
                client.DropMulticastGroup(IPAddress.Parse(_groupAddress));
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

            if (_hostName != null)
            {
                IPHostEntry hostEntry = await Dns.GetHostEntryAsync(_hostName);
                IPAddress? address = _useIpv6
                    ? hostEntry.AddressList.FirstOrDefault(a => a.AddressFamily == AddressFamily.InterNetworkV6)
                    : hostEntry.AddressList.FirstOrDefault(a => a.AddressFamily == AddressFamily.InterNetwork);

                if (address == null)
                {
                    string ipversion = _useIpv6 ? "IPv6" : "IPv4";
                    Console.WriteLine($"no {ipversion} address for {_hostName}");
                    return null;
                }

                return new IPEndPoint(address, _port);
            }

            if (_groupAddress != null)
            {
                return new IPEndPoint(IPAddress.Parse(_groupAddress), _port);
            }

            throw new InvalidOperationException($"{nameof(_hostName)}, {nameof(_useBroadcast)}, or {nameof(_groupAddress)} must be set");
        }
        catch (SocketException ex)
        {
            Console.WriteLine($"Error {ex.Message}");
            return null;
        }
    }
}