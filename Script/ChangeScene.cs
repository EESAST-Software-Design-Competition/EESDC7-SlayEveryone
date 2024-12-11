using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.NetworkInformation;
using UnityEngine.SceneManagement;
using Unity.Netcode.Transports.UTP;
using Unity.Netcode;
using UnityEngine.UI;
using TMPro;


public class ChangeScene : MonoBehaviour
{
    private string id;
    private bool startgame=false;
    private NetworkManager networkManager;
    private TMP_InputField inputHostID;
    private int toint(char c)
    {
        if(c>='0'&&c<='9') return c-48;
        return c-55;
    }
    private string decode(string cur)
    {
        string decoded = "";
        for(int i=0;i<6;i+=2)
        {
            int gui=toint(cur[i])*16+toint(cur[i+1]);
            decoded=decoded+gui.ToString();
            decoded=decoded+".";
        }
        int lai=toint(cur[6])*0x10+toint(cur[7]);
        decoded=decoded+lai.ToString();
        return decoded;
    }
    
    private void SceneChange(Scene scene, LoadSceneMode loadSceneMode)
    {
        if(!startgame) return;
        networkManager=GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
        if(networkManager==null) return;
        if(id=="Client")
        {
            Canvas clientUI=GameObject.Find("ClientUI").GetComponent<Canvas>();
            clientUI.enabled=true;
            inputHostID=clientUI.transform.Find("InputHostID").GetComponent<TMP_InputField>();
            Button joinGame=clientUI.transform.Find("JoinGame").GetComponent<Button>();
            joinGame.onClick.AddListener(()=>{
                networkManager.GetComponent<UnityTransport>().ConnectionData.Address=decode(inputHostID.text);
                clientUI.enabled=false;
                NetworkManager.Singleton.StartClient();
            });
        }
        if(id=="Host")
        {
            // string WlanAddress=GetWLANIPAddress();
            // networkManager.GetComponent<UnityTransport>().ConnectionData.Address=WlanAddress;
            NetworkManager.Singleton.StartHost();
        }
        startgame=false;
    }
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded+=SceneChange;
    }
    private void Update()
    {
        // if(Input.GetKeyDown(KeyCode.T)) SceneManager.LoadScene("GameScene");
    }
    public void SetID(string cur)
    {
        id=cur;
        startgame=true;
    }
    private string GetWLANIPAddress()
    {
        string wlanIP = "";

        NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();
        foreach (NetworkInterface iface in interfaces)
        {
            if (iface.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 && iface.OperationalStatus == OperationalStatus.Up)
            {
                IPInterfaceProperties properties = iface.GetIPProperties();
                foreach (UnicastIPAddressInformation ip in properties.UnicastAddresses)
                {
                    if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        wlanIP = ip.Address.ToString();
                        break;
                    }
                }
            }
        }
        return wlanIP;
    }
}
