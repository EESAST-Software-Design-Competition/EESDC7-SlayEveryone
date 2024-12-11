using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.NetworkInformation;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
public class NetHelper : MonoBehaviour
{
    // public NetworkManager networkManager;
    // private void Start()
    // {
    //     if (networkManager != null)
    //     { // 设置服务器监听端口
    //         // 获取本地 IP 地址
    //         // string localIpAddress = GetLocalIpAddress();
    //         string localIpAddress=GetWLANIPAddress();
    //         networkManager.GetComponent<UnityTransport>().ConnectionData.Address=localIpAddress;
    //     }
    // }
    // private string GetLocalIpAddress()
    // {
    //     // 获取本地IP地址
    //     string localIpAddress = "";
    //     // 获取当前网络接口的IP地址
    //     var host = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());
    //     foreach (var ip in host.AddressList)
    //     {
    //         if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
    //         {
    //             localIpAddress = ip.ToString();
    //             break;
    //         }
    //     }
    //     return localIpAddress;
    // }
    // private string GetWLANIPAddress()
    // {
    //     string wlanIP = "";

    //     NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();
    //     foreach (NetworkInterface iface in interfaces)
    //     {
    //         if (iface.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 && iface.OperationalStatus == OperationalStatus.Up)
    //         {
    //             IPInterfaceProperties properties = iface.GetIPProperties();
    //             foreach (UnicastIPAddressInformation ip in properties.UnicastAddresses)
    //             {
    //                 if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
    //                 {
    //                     wlanIP = ip.Address.ToString();
    //                     break;
    //                 }
    //             }
    //         }
    //     }

    //     return wlanIP;
    // }
}
