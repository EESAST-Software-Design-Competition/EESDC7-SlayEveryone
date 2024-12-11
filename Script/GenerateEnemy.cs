using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.Netcode;
using Unity.AI.Navigation;

public class GenerateEnemy : NetworkBehaviour
{
    public GameObject[] enemyPrefabs;
    private int total;
    private float enemyperiod=20f;
    private float siz;
    private float maxdis=10f;
    private NavMeshHit hit;
    public void startGenerate(float _siz)
    {
        siz=_siz;
        total=enemyPrefabs.Length;
        int maps=GameObject.Find("MapController").GetComponent<MapController>().mapSize;
        enemyperiod/=((float)(maps-2));
        StartCoroutine(PlaceEnemy());
    }
    private IEnumerator PlaceEnemy()
    {
        int seed = Mathf.FloorToInt(Time.time);
        Random.InitState(seed);
        while(true)
        {
            int index=Random.Range(0,total);
            while(true)
            {
                float x=Random.Range(0f,siz),z=Random.Range(0f,siz);
                bool fanhuizhi=NavMesh.SamplePosition(new Vector3(x,0f,z),out hit,maxdis,NavMesh.AllAreas);
                // Debug.Log(fanhuizhi);
                if(!fanhuizhi||hit.position.y>0.5f) continue;
                // Debug.Log(hit.position);
                GameObject cur=Instantiate(enemyPrefabs[index],hit.position,Quaternion.identity);
                cur.GetComponent<NetworkObject>().Spawn();
                // cur.transform.position=hit.position;
                // Debug.Log(hit.position);
                yield return new WaitForSeconds(enemyperiod);
                break;
            }
        }
    }
}
