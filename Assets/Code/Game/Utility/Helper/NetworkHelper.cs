using System.Net.NetworkInformation;
using System.Net.Sockets;

public static class NetworkHelper
{
    public static string GetLocalIPv4(NetworkInterfaceType interfaceType)
    {
        var output = "";
        foreach (var item in NetworkInterface.GetAllNetworkInterfaces())
        {
            if (item.NetworkInterfaceType != interfaceType || item.OperationalStatus != OperationalStatus.Up) continue;
            
            foreach (var ip in item.GetIPProperties().UnicastAddresses)
            {
                if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                {
                    output = ip.Address.ToString();
                }
            }
        }
        return output;
    }
}
