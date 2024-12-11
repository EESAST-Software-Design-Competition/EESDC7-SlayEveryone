using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class GenerateItem : NetworkBehaviour
{
    public GameObject[] itemlist;
    private float itemperiod=7f;
    private int total;
    private float siz;
    public void startGenerate(float _siz)
    {
        siz=_siz;
        total=itemlist.Length;
        StartCoroutine(PlaceItems());
    }
    private IEnumerator PlaceItems()
    {
       
        while(true)
        {
            int index=Random.Range(0,total);
            yield return new WaitForSeconds(itemperiod);
            float x=Random.Range(0f,siz),z=Random.Range(0f,siz);
            // GameObject cur=Instantiate(itemlist[index],new Vector3 (x,0.5f,z),Quaternion.identity);
            // cur.GetComponent<NetworkObject>().Spawn();
            GenerateItemServerRpc(index,x,z);
        }
    }
    [ServerRpc]
    void GenerateItemServerRpc(int index,float x,float z)
    {
        GameObject cur=Instantiate(itemlist[index],new Vector3 (x,0.5f,z),Quaternion.identity);
        cur.GetComponent<NetworkObject>().Spawn();
        if(index==5) M67enabledClientRpc(cur);
    }
    [ClientRpc]
    private void M67enabledClientRpc(NetworkObjectReference cur)
    {
        if(!cur.TryGet(out NetworkObject networkObject))
        {
            Debug.Log("cuola");
        }
        networkObject.GetComponent<Collider>().enabled=true;
    }
}

