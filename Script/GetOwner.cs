using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class GetOwner : MonoBehaviour
{
    public ulong customNetworkID;
    private void Awake()
    {
        Invoke("RealAwake",0.05f);
    }
    private void RealAwake()
    {
        NetworkObject networkObject=GetComponent<NetworkObject>();
        GameIDManager cur=GameObject.Find("GameIDManager").GetComponent<GameIDManager>();
        customNetworkID=cur.GetNewID(networkObject.NetworkObjectId);
    }
}
