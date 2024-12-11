using Unity.VisualScripting;
using UnityEngine;
using System;
using System.Collections.Generic;
using Unity.Netcode;

public class ItemManager : NetworkBehaviour
{
    private BagManager BagGUI;
    public string playerId;
    public Weapon thisWeapon;
    public PlayerStateManager playerStateManager;
    public SkinnedMeshRenderer bodyMeshRenderer;//用于隐身
    public MusicController musicController;
    public void SetPlayerId(string id)
    {
        playerId = id;
    }

    // public List<ItemList> itemList=new List<ItemList>();
    private ItemList[] itemList = new ItemList[5];

    public int maxLengthOfArray = 5;

    bool isUsingItem;

    public Vector3 direction;
    private void Awake()
    {
        Invoke("GetGUI", 0.1f);
    }
    private void GetGUI()
    {
        if (!IsOwner) return;
        for (int i = 0; i < 5; i++)
            itemList[i] = ScriptableObject.CreateInstance<ItemList>();
        BagGUI = GameObject.Find("Bag").GetComponent<BagManager>();
    }
    private void SyncDestroy(GameObject now)
    {
        SyncDestroyServerRpc(now.GetComponent<NetworkObject>());
    }
    [ServerRpc]
    private void SyncDestroyServerRpc(NetworkObjectReference cur)
    {
        if(!cur.TryGet(out NetworkObject networkObject))
        {
            Debug.Log("zhaobudaoyidian");
            return;
        }
        networkObject.Despawn();
    }
    void OnTriggerEnter(Collider collision)
    {
        //if (!Input.GetKeyDown(KeyCode.E))
        //{
        //    return;
        //}
        if (!IsOwner) return;
        // Debug.Log("hello");
        if (!collision) // ��ֹ���õ��յ���ײ��
        {
            Debug.LogError("Other collider is null");
            return;
        }
        if (!collision.CompareTag("Item")) return;
        PropertiesForItems properties = collision.gameObject.GetComponent<PropertiesForItems>();

        if (collision.gameObject.GetComponent<PropertiesForItems>() != null)
        {
            Debug.Log(properties);
            bool isInList = false; int position = 0;
            foreach (var item in itemList)
            {
                if (item.itemNumber == 0)
                {
                    break;
                }
                else if (item.itemName == properties.nameOfItems && item.itemNumber < properties.maxItemNumber)
                {
                    isInList = true; break;
                }
                else
                {
                    position++;
                }
            }
            if (position == maxLengthOfArray) return;
            if (isInList)
            {
                var item = itemList[position];
                // if (item.itemNumber < item.maxItemNumber)
                {
                    SyncDestroy(collision.gameObject);
                    // NetworkObject.Dispose(collision.gameObject);
                    item.itemNumber++;
                    musicController.PlayGetObjectMusic();
                    itemList[position] = item; // �޸ĺ����¸�ֵ
                    BagGUI.NumberReset(position, item.itemNumber);
                }
            }
            else
            {
                ItemList newItem = new ItemList
                {
                    itemName = properties.nameOfItems,
                    itemNumber = 1,
                    maxItemNumber = properties.maxItemNumber
                };
                // itemList.Add(newItem);
                itemList[position] = newItem;
                musicController.PlayGetObjectMusic();
                BagGUI.PictureRender(position, properties.id);
                BagGUI.NumberReset(position, 1);
                SyncDestroy(collision.gameObject);
                // NetworkObject.Dispose(collision.gameObject);
            }
            Debug.Log(itemList[position].itemName + ":" + itemList[position].itemNumber);//���ڼ����������
        }
    }
    public static ItemManager GetItemsManagerForPlayer(string playerId)
    {
        ItemManager[] allManagers = FindObjectsOfType<ItemManager>();
        foreach (var manager in allManagers)
        {
            if (manager.playerId == playerId)
            {
                return manager;
            }
        }
        return null; // ���û�ҵ������� null
    }
    private void GetBuff(string name)
    {
        if (name == "AmmoPack") AmmoBuff();
        else if (name == "RedBottle") RedBottleBuff();
        else if (name == "BlueBootle") BlueBottleBuff();
        else if (name == "InvisibleBottle")
        {
            //InvisibleBottleBuff();
            //if (IsClient) InvisibleBottleBuffServerRpc();
            //if (IsServer) InvisibleBottleBuffClientRpc();
            InvisibleBottleBuffServerRpc();
        }
        else if (name == "M67") M67BurstBuff();
        else if (name == "EnergyBottle") EnergyBottleBuff();
    }
    private void ItemUsing(int bagid)
    {
        if (bagid >= maxLengthOfArray) return;
        if (itemList[bagid].itemNumber == 0) return;
        Debug.Log(itemList[bagid].itemNumber);
        itemList[bagid].itemNumber--;
        GetBuff(itemList[bagid].itemName);
        if (itemList[bagid].itemNumber == 0) BagGUI.destroyGUI(bagid);
        else BagGUI.NumberReset(bagid, itemList[bagid].itemNumber);
    }
    private void Update()
    {
        if (!IsOwner) return;
        for (int i = 0, j = 1; j <= 5 && i <= 4; i++, j++)//Bug Fixed
            if (Input.GetKeyDown(KeyCode.Alpha0 + j) || Input.GetKeyDown(KeyCode.Keypad0 + j))
                ItemUsing(i);
    }
    private void AmmoBuff()
    {
        if(thisWeapon==null) return;
        for(int i = 0; i < 3; i++)
        {
            thisWeapon.shootingMode=(Weapon.ShootingMode)((int)(thisWeapon.shootingMode+1)%3);
            if (thisWeapon.shootingMode == Weapon.ShootingMode.Auto)
            {
                thisWeapon.bulletsRemained += 60;
            }
            else if (thisWeapon.shootingMode == Weapon.ShootingMode.Burst)
            {
                thisWeapon.bulletsRemained += 10;
            }
            else if (thisWeapon.shootingMode == Weapon.ShootingMode.Single)
            {
                thisWeapon.bulletsRemained += 18;
            }
        }
        
    }
    private void RedBottleBuff()
    {
        playerStateManager.currentHP += playerStateManager.maxHP * 0.3f;
        playerStateManager.currentEnergy += 2f;
    }
    private void BlueBottleBuff()
    {
        playerStateManager.currentMana += playerStateManager.maxMana * 0.8f;
        playerStateManager.currentEnergy += 2f;
    }
    private void InvisibleBottleBuff()
    {
        // 获取该 GameObject 及其所有子对象（无论它们是否被禁用）的所有渲染器
        Renderer[] renderers = GetComponentsInChildren<Renderer>(true);

        // 遍历所有渲染器并将它们禁用
        foreach (Renderer r in renderers)
        {
            r.enabled = false;
        }
        if (playerStateManager.identity == PlayerStateManager.Identity.Berserker)
        {
            gameObject.GetComponent<TrailRenderer>().enabled = true;
        }
        Invoke("BeVisibleAgain", 10);
    }
    [ServerRpc]
    private void InvisibleBottleBuffServerRpc()
    {
        InvisibleBottleBuffClientRpc();
    }
    [ClientRpc]
    private void InvisibleBottleBuffClientRpc()
    {
        InvisibleBottleBuff();
    }
    private void BeVisibleAgain()
    {
        // 获取该 GameObject 及其所有子对象（无论它们是否被禁用）的所有渲染器
        Renderer[] renderers = GetComponentsInChildren<Renderer>(true);

        // 遍历所有渲染器并将它们启用
        foreach (Renderer r in renderers)
        {
            r.enabled = true;
        }
        if (playerStateManager.identity == PlayerStateManager.Identity.Berserker)
        {
            gameObject.GetComponent<TrailRenderer>().enabled = false;
        }
    }

    public GameObject grenadePrefab;
    public float throwForce = 20f;
    public Transform Spawn;
    private void M67BurstBuff()
    {
        //Spawn = gameObject.transform.Find("PolyArtWizardStandardMat").transform.Find("Spawn");//一定要放在第一层级，不然gg
        M67ServerRpc(Spawn.position, Quaternion.identity, direction);

    }
    [ServerRpc]
    private void M67ServerRpc(Vector3 pos, Quaternion rot, Vector3 dir)
    {
        GameObject M67 = Instantiate(grenadePrefab, pos, rot);
        M67.GetComponent<NetworkObject>().Spawn();
        GrenadeBurst grenadeBurst = M67.GetComponent<GrenadeBurst>();
        grenadeBurst.enabled = true;
        M67.GetComponent<Rigidbody>().isKinematic = false;
        if (!IsServer) Debug.Log("RpcCHUWENTRILE");
        Rigidbody rb = M67.GetComponent<Rigidbody>();
        //float x = UnityEngine.Random.Range(-spreadIntensity / 100, spreadIntensity / 100);
        //float y = UnityEngine.Random.Range(-spreadIntensity / 100, spreadIntensity / 100);
        //rb.velocity = (dir + new Vector3(x, 0, 0)) * throwForce;
        rb.velocity = (dir.normalized+new Vector3(0,0.8f,0)).normalized * throwForce;
    }
    private void EnergyBottleBuff()
    {
        playerStateManager.currentEnergy += 2f;
        GetComponent<PlayerMovement>().exhaustingRate /= 2;
        GetComponent<PlayerMovement>().speed *= 1.2f;
        Invoke("ResetEnergyBuff", 10f);
    }
    private void ResetEnergyBuff()
    {
        GetComponent<PlayerMovement>().exhaustingRate *= 2;
        GetComponent<PlayerMovement>().speed /= 1.2f;
    }
}
