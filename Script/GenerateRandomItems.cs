using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GenerateRandomItems : NetworkBehaviour
{
    public PropertiesForItems[] ItemPrefabs;
    void Start()
    {
        StartCoroutine(GenerateItem());
    }
    private IEnumerator GenerateItem()
    {
        float xr=10f,zr=10f;
        int siz=ItemPrefabs.Length;
        while(true)
        {
            Vector3 pos=new Vector3(Random.Range(-xr,xr),0,Random.Range(-zr,zr));
            int index=Random.Range(0,siz);
            Instantiate(ItemPrefabs[index],pos,Quaternion.identity).GetComponent<NetworkObject>().Spawn();
            yield return new WaitForSeconds(5);
        }
    }
}
