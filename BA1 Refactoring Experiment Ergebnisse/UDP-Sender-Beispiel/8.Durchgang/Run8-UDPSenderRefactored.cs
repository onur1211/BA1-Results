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

    private const string ByeCommand = "bye";

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
            return;

        try
        {
            using UdpClient client = new() { EnableBroadcast = _useBroadcast };

            if (_groupAddress != null)
                client.JoinMulticastGroup(IPAddress.Parse(_groupAddress));

            bool completed = false;

            do
            {
                Console.WriteLine($"{Environment.NewLine}Enter a message or \"{ByeCommand}\" to exit");
                string? input = Console.ReadLine();

                if (input is null)
                    continue;

                completed = input.Equals(ByeCommand, StringComparison.CurrentCultureIgnoreCase);

                byte[] datagram = Encoding.UTF8.GetBytes(input);
                int sent = await client.SendAsync(datagram, datagram.Length, endpoint);

                Console.WriteLine($"Sent datagram using local EP {client.Client.LocalEndPoint} to {endpoint}");
            } while (!completed);

            if (_groupAddress != null)
                client.DropMulticastGroup(IPAddress.Parse(_groupAddress));
        }
        catch (SocketException ex)
        {
            HandleSocketException(ex);
        }
    }

    private async Task<IPEndPoint?> GetReceiverIPEndPointAsync()
    {
        try
        {
            return _useBroadcast ? await GetBroadcastEndpointAsync() :
                   _hostName != null ? await GetHostEndpointAsync() :
                   _groupAddress != null ? new IPEndPoint(IPAddress.Parse(_groupAddress), _port) :
                   throw new InvalidOperationException($"{nameof(_hostName)}, {nameof(_useBroadcast)}, or {nameof(_groupAddress)} must be set");
        }
        catch (SocketException ex)
        {
            HandleSocketException(ex);
            return null;
        }
    }

    private async Task<IPEndPoint?> GetBroadcastEndpointAsync()
    {
        return new IPEndPoint(IPAddress.Broadcast, _port);
    }

    private async Task<IPEndPoint?> GetHostEndpointAsync()
    {
        IPHostEntry hostEntry = await Dns.GetHostEntryAsync(_hostName);
        IPAddress? address = SelectIPAddress(hostEntry.AddressList);

        if (address == null)
        {
            string ipversion = _useIpv6 ? "IPv6" : "IPv4";
            Console.WriteLine($"No {ipversion} address for {_hostName}");
            return null;
        }

        return new IPEndPoint(address, _port);
    }

    private IPAddress? SelectIPAddress(IPAddress[] addressList)
    {
        return _useIpv6 ?
            addressList.FirstOrDefault(a => a.AddressFamily == AddressFamily.InterNetworkV6) :
            addressList.FirstOrDefault(a => a.AddressFamily == AddressFamily.InterNetwork);
    }

    private void HandleSocketException(SocketException ex)
    {
        Console.WriteLine($"Error {ex.Message}");
    }
}