using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameIDManager : MonoBehaviour
{
    private ulong cur=0;
    public ulong[] saved=new ulong [10];
    public ulong GetNewID(ulong ActualNetworkObjectID)
    {
        saved[++cur]=ActualNetworkObjectID;
        // Debug.Log(cur);
        return cur;
    }
    public ulong GetActualNetworkObjectID(ulong index)
    {
        return saved[index];
    }
}
