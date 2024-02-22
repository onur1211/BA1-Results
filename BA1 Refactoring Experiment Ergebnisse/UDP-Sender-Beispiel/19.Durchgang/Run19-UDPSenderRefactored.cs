using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

public record Sender(int Port, string? Hostname, bool UseBroadcast, string? GroupAddress, bool UseIpv6)
{
    public async Task RunAsync()
    {
        IPEndPoint? endpoint = await GetReceiverIPEndPointAsync();

        if (endpoint is null)
        {
            return;
        }

        try
        {
            using UdpClient client = new() { EnableBroadcast = UseBroadcast };

            if (GroupAddress != null)
            {
                client.JoinMulticastGroup(IPAddress.Parse(GroupAddress));
            }

            bool completed = false;
            do
            {
                Console.WriteLine($"{Environment.NewLine}Enter a message or \"bye\" to exit");
                string input = Console.ReadLine() ?? string.Empty;

                completed = input.Equals("bye", StringComparison.CurrentCultureIgnoreCase);
                await SendDatagramAsync(client, endpoint, input);

            } while (!completed);

            if (GroupAddress != null)
            {
                client.DropMulticastGroup(IPAddress.Parse(GroupAddress));
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
            return (UseBroadcast, Hostname, GroupAddress) switch
            {
                (true, _, _) => new IPEndPoint(IPAddress.Broadcast, Port),
                (_, { } host, _) => await GetEndPointForHostAsync(host),
                (_, _, { } group) => new IPEndPoint(IPAddress.Parse(group), Port),
                _ => throw new InvalidOperationException($"{nameof(Hostname)}, {nameof(UseBroadcast)}, or {nameof(GroupAddress)} must be set")
            };
        }
        catch (SocketException ex)
        {
            Console.WriteLine($"Error {ex.Message}");
            return null;
        }
    }

    private async Task<IPEndPoint?> GetEndPointForHostAsync(string host)
    {
        try
        {
            IPHostEntry hostEntry = await Dns.GetHostEntryAsync(host);
            IPAddress? address = UseIpv6
                ? hostEntry.AddressList.FirstOrDefault(a => a.AddressFamily == AddressFamily.InterNetworkV6)
                : hostEntry.AddressList.FirstOrDefault(a => a.AddressFamily == AddressFamily.InterNetwork);

            if (address == null)
            {
                Console.WriteLine($"no {GetIpVersion()} address for {host}");
                return null;
            }

            return new IPEndPoint(address, Port);
        }
        catch (SocketException ex)
        {
            Console.WriteLine($"Error {ex.Message}");
            return null;
        }
    }

    private string GetIpVersion() => UseIpv6 ? "IPv6" : "IPv4";

    private async Task SendDatagramAsync(UdpClient client, IPEndPoint endpoint, string input)
    {
        byte[] datagram = Encoding.UTF8.GetBytes(input);
        int sent = await client.SendAsync(datagram, datagram.Length, endpoint);
        Console.WriteLine($"Sent datagram using local EP {client.Client.LocalEndPoint} to {endpoint}");
    }
}