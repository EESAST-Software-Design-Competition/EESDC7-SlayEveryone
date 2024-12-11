using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System.Net.NetworkInformation;
using UnityEngine.UI;
using TMPro;
public class ChooseCharacter : NetworkBehaviour
{
    static public ulong numberOfPlayers=0;
    public PlayerStateManager BerserkerPrefab,GunslingerPrefab,WizardPrefab;
    [SerializeField] private Button BerserkerBtn;
    [SerializeField] private Button GunslingerBtn;
    [SerializeField] private Button WizardBtn;
    [SerializeField] private TextMeshProUGUI UIforIP;
    [SerializeField] private Button startplayButtonPrefab;
    private Canvas thisCanva;
    private void ActivateButtonsHost()
    {
        Canvas now=transform.Find("CharacterChoose").GetComponent<Canvas>();
        now.enabled=true;
        MapController mapc=GameObject.Find("MapController").GetComponent<MapController>();
        float x=Random.Range(0,(mapc.mapSize-1)*(mapc.blockSize+mapc.gapSize));
        float z=Random.Range(0,(mapc.mapSize-1)*(mapc.blockSize+mapc.gapSize));
        BerserkerBtn.onClick.AddListener(()=>{
            var instance=Instantiate(BerserkerPrefab,new Vector3(x,0,z),Quaternion.identity);
            instance.GetComponent<PlayerMovement>().CustomIsOwner=true;
            instance.GetComponent<NetworkObject>().Spawn();
            thisCanva.enabled=now.enabled=false;
        });
        GunslingerBtn.onClick.AddListener(()=>{
            var instance=Instantiate(GunslingerPrefab,new Vector3(x,0,z),Quaternion.identity);
            instance.GetComponent<PlayerMovement>().CustomIsOwner=true;
            instance.GetComponent<NetworkObject>().Spawn();
            thisCanva.enabled=now.enabled=false;
        });
        WizardBtn.onClick.AddListener(()=>{
            var instance=Instantiate(WizardPrefab,new Vector3(x,0,z),Quaternion.identity);
            instance.GetComponent<PlayerMovement>().CustomIsOwner=true;
            instance.GetComponent<NetworkObject>().Spawn();
            thisCanva.enabled=now.enabled=false;
        });
    }
    private void Awake()
    {
        Invoke("RealAwake",0.08f);
    }
    [ClientRpc]
    private void ActivateButtonsClientRpc()
    {
        if (IsHost) return;
        GameObject me=null;
        GameObject[] networkPlayers=GameObject.FindGameObjectsWithTag("NetworkPlayer");
        foreach(GameObject networkPlayer in networkPlayers)
        {
            if(networkPlayer.GetComponent<NetworkObject>().IsOwner==true)
            {
                me=networkPlayer;
                break;
            }
        }                                                                   //找到自己
        if(me==null) Debug.Log("写了一坨屎");
        Transform fa=me.transform.Find("Temp");
        if(fa==null) Debug.Log("咋会找不到呢");
        foreach(Transform son in fa)
        {
            if(son.CompareTag("UIforIP"))
            {
                Destroy(son.gameObject);
                break;
            }
        }
        // Debug.Log("jin");
        // GameObject tobedeleted=me.transform.Find("Temp").transform.CompareTag("UIforIP").gameObject;
        // Debug.Log("chu");
        // if(tobedeleted==null) Debug.Log("你在删你的妈");
        // Debug.Log("zaichu");
        // Destroy(tobedeleted);
        me.GetComponent<ChooseCharacter>().SetClientButtons();      //需要你真实的拿心化冰，不讲虚的话听。
    }
    public void SetClientButtons()
    {
        Canvas now=transform.Find("CharacterChoose").GetComponent<Canvas>();
        now.enabled=true;
        BerserkerBtn.onClick.AddListener(()=>{
            now.enabled=false;
            SyncNewCharacterServerRpc("Berserker",GetComponent<NetworkObject>().OwnerClientId);
        });
        GunslingerBtn.onClick.AddListener(()=>{
            now.enabled=false;
            SyncNewCharacterServerRpc("Gunslinger",GetComponent<NetworkObject>().OwnerClientId);
        });
        WizardBtn.onClick.AddListener(()=>{
            now.enabled=false;
            SyncNewCharacterServerRpc("Wizard",GetComponent<NetworkObject>().OwnerClientId);
         });
    }
    private string tohex(int lai)
    {
        string ans = "";
        char tmp;
        while(lai>0)
        {
            int g=lai%16;
            if(g<=9) tmp=(char)(g+48);
            else tmp=(char)(g+55);
            ans=tmp.ToString()+ans;
            lai/=16;
        }
        if(ans.Length<2) ans="0"+ans;
        return ans;
    }
    private string encode(string cur)
    {
        string encoded = "";
        int now=0;
        for(int i=0,len=cur.Length;i<len;i++)
        {
            char c=cur[i];
            if(c>='0'&&c<='9') now=now*10+(int)c-48;
            else
            {
                encoded+=tohex(now);
                now=0;
            }
        }
        encoded+=tohex(now);
        return encoded;
    }
    private void RealAwake()
    {
        numberOfPlayers++;
        Canvas staticinfo=GameObject.Find("StaticInfo").GetComponent<Canvas>();
        TextMeshProUGUI playerconnectinfo=staticinfo.transform.Find("PlayerConnected").GetComponent<TextMeshProUGUI>();
        playerconnectinfo.text="Player Connected:"+numberOfPlayers.ToString();
        if(!IsOwner) return;
        if(IsHost)
        {
            staticinfo.enabled=true;
            thisCanva=transform.Find("Temp").GetComponent<Canvas>();
            TextMeshProUGUI ins=Instantiate(UIforIP,thisCanva.transform);
            string localWLANAddress=GetWLANIPAddress();
            ins.text="Your Id:"+encode(localWLANAddress);
            Button startplay=Instantiate(startplayButtonPrefab,thisCanva.transform);
            startplay.onClick.AddListener(()=>{
                Destroy(ins.gameObject);
                // GameObject.Find("NetworkManager").GetComponent<NetworkManager>().NetworkConfig.ConnectionApproval=false;
                ActivateButtonsHost();
                ActivateButtonsClientRpc();
                GameObject.Find("MapController").GetComponent<MapController>().mapSize=(int)(System.Math.Sqrt((double)numberOfPlayers))+2;
                GameObject.Find("MapController").GetComponent<MapController>().Startmap();
                thisCanva.enabled=false;
            });
            return;
        }
        if(IsClient)
        {
            thisCanva=transform.Find("Temp").GetComponent<Canvas>();
            TextMeshProUGUI ins=Instantiate(UIforIP,thisCanva.transform);
            ins.text="Waiting for the game to start";
        }
    }
    [ServerRpc]
    private void SyncNewCharacterServerRpc(string typ,ulong onwerClientId)
    {
        MapController mapc=GameObject.Find("MapController").GetComponent<MapController>();
        float x=Random.Range(0,(mapc.mapSize-1)*(mapc.blockSize+mapc.gapSize));
        float z=Random.Range(0,(mapc.mapSize-1)*(mapc.blockSize+mapc.gapSize));
        ulong cur=0;
        if(typ=="Berserker")
        {
            var instance=Instantiate(BerserkerPrefab,new Vector3(x,0,z),Quaternion.identity);
            instance.GetComponent<NetworkObject>().Spawn();
            instance.NetworkObject.ChangeOwnership(onwerClientId);
            cur=instance.GetComponent<NetworkObject>().NetworkObjectId;
        }
        if(typ=="Gunslinger")
        {
            var instance=Instantiate(GunslingerPrefab,new Vector3(x,0,z),Quaternion.identity);
            instance.GetComponent<NetworkObject>().Spawn();
            instance.NetworkObject.ChangeOwnership(onwerClientId);
            cur=instance.GetComponent<NetworkObject>().NetworkObjectId;
        }
        if(typ=="Wizard")
        {
            var instance=Instantiate(WizardPrefab,new Vector3(x,0,z),Quaternion.identity);
            instance.GetComponent<NetworkObject>().Spawn();
            instance.NetworkObject.ChangeOwnership(onwerClientId);
            cur=instance.GetComponent<NetworkObject>().NetworkObjectId;
        }
        // ConfigCustomIsOwnerClientRpc(cur,onwerClientId);
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
