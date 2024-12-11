using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.AI;
using Unity.AI.Navigation;

public class MapController : NetworkBehaviour
{
    public GameObject[] mapBlocks; 
    public int mapSize = 3;
    public NavMeshSurface navMeshSurface;
    private bool[,] occupiedPositions; 
    public float blockSize = 10f;
    public float gapSize = 20f;
    [SerializeField] GameObject planePrefab;
    private GameObject newPlane1,newPlane2,newPlane3,newPlane4;
    private float rand1,rand2;
    public void Startmap()
    {
        rand1 = UnityEngine.Random.Range(0f, 1f);
        rand2 = UnityEngine.Random.Range(0f, 1f);
        if (!IsServer) return;
        occupiedPositions = new bool[mapSize, mapSize];
        GenerateMap();
        PrePareAI();
        GetComponent<GenerateEnemy>().startGenerate((mapSize-1)*(blockSize+gapSize));
        GetComponent<GenerateItem>().startGenerate((mapSize-1)*(blockSize+gapSize));
    }
    private void PrePareAI()
    {
        // 获取场景中所有需要设置权限的物体
    //GameObject[] objects = GameObject.FindGameObjectsWithTag("Wall");

    // foreach (GameObject obj in objects)
    // {
    //     // 设置网格对象的读写权限
    //     MeshRenderer renderer = obj.GetComponent<MeshRenderer>();
    //     if (renderer != null)
    //     {
    //         if(renderer.material.mainTexture==null) continue;
    //         renderer.material.mainTexture.wrapMode = TextureWrapMode.Repeat;
    //     }

    //     // 设置其他可能需要设置权限的组件
    //     // ...
    // }
        Debug.Log("laila");
        navMeshSurface.BuildNavMesh();
        Debug.Log("daola");
    }
    private void GenerateMap()
    {
        for (int i = 0; i < mapSize; i++)
        {
            for (int j = 0; j < mapSize; j++)
            {
               
                Vector2Int position = FindAvailablePosition();
                
        
                GameObject randomBlock = mapBlocks[Random.Range(0, mapBlocks.Length)];

               
                GameObject newBlock = Instantiate(randomBlock, new Vector3(position.x * (blockSize + gapSize), 0, position.y * (blockSize + gapSize)), Quaternion.identity);
                newBlock.GetComponent<NetworkObject>().Spawn();
                newBlock.transform.Rotate(Vector3.up, Random.Range(0, 4) * 90);
                occupiedPositions[position.x, position.y] = true;
            }
        }
        newPlane1 = Instantiate(planePrefab,new Vector3(-(blockSize + gapSize),0f,(mapSize-1)*(blockSize + gapSize)/2),Quaternion.identity);//z90
        newPlane1.transform.Rotate(new Vector3(0f,0f,-90f));
        newPlane1.transform.localScale=new Vector3(1f,1f,mapSize<<2);
        newPlane2 = Instantiate(planePrefab,new Vector3(mapSize*(blockSize+gapSize),0f,(mapSize-1)*(blockSize + gapSize)/2),Quaternion.identity);
        newPlane2.transform.Rotate(new Vector3(0f,0f,90f));
        newPlane2.transform.localScale=new Vector3(1f,1f,mapSize<<2);
        newPlane3 = Instantiate(planePrefab,new Vector3((mapSize-1)*(blockSize + gapSize)/2,0f,-(blockSize + gapSize)),Quaternion.identity);//x90
        newPlane3.transform.Rotate(new Vector3(90f,0f,0f));
        newPlane3.transform.localScale=new Vector3(mapSize<<2,1f,1f);
        newPlane4 = Instantiate(planePrefab,new Vector3((mapSize-1)*(blockSize + gapSize)/2,0f,mapSize*(blockSize+gapSize)),Quaternion.identity);
        newPlane4.transform.Rotate(new Vector3(-90f,0f,0f));
        newPlane4.transform.localScale=new Vector3(mapSize<<2,1f,1f);
        newPlane1.GetComponent<NetworkObject>().Spawn();
        newPlane2.GetComponent<NetworkObject>().Spawn();
        newPlane3.GetComponent<NetworkObject>().Spawn();
        newPlane4.GetComponent<NetworkObject>().Spawn();
    }
    private float sudu=0.15f;
    private void Update()
    {
        if(newPlane4==null) return;
        newPlane1.transform.position+=new Vector3(sudu*Time.deltaTime,0f,0f)*rand1*2;
        newPlane2.transform.position+=new Vector3(-sudu*Time.deltaTime,0f,0f)*(1-rand1)*2;
        newPlane3.transform.position+=new Vector3(0f,0f,sudu*Time.deltaTime)*rand2*2;
        newPlane4.transform.position+=new Vector3(0f,0f,-sudu*Time.deltaTime)*(1-rand2)*2;
    }
    Vector2Int FindAvailablePosition()
    {
        Vector2Int position;
        do
        {
            position = new Vector2Int(Random.Range(0, mapSize), Random.Range(0, mapSize));
        } while (occupiedPositions[position.x, position.y]); 

        return position;
    }
}