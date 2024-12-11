using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using TMPro;
using UnityEditor;

public class PlayerStateManager : NetworkBehaviour
{
    private ScoreManager scoreManager;
    // private TextMeshProUGUI scoreUI;
    private static int totalnumber=0;
    public int playerId;//for network
    public enum Identity
    {
        Wizard, Berserker, Gunslinger//��ʦ����սʿ��ǹ��
    }
    public Identity identity;
    public float maxHP=100f;
    private NetworkVariable<float> CurrentHP=new NetworkVariable<float>();
    //public GameObject bloodEffect;
    public Color hitColor = Color.red; // 受伤时的颜色
    private Color originalColor;
    private Dictionary<Material, Color> originalColors = new Dictionary<Material, Color>();
    public MusicController musicController;
    [ServerRpc(RequireOwnership =false)]
    private void GenerateNewPlayerServerRpc()
    {
        GenerateNewPlayerClientRpc();
    }
    [ClientRpc]
    private void GenerateNewPlayerClientRpc()
    {
        TextMeshProUGUI cur=GameObject.Find("ScoreInfo").GetComponent<ScoreManager>().GenerateNewScore();
        // scoreUI=cur;
        if(IsOwner) cur.color=Color.green;
    }
    private void Start()
    {
        scoreManager=GameObject.Find("ScoreInfo").GetComponent<ScoreManager>();
        playerId=++totalnumber;
        CacheOriginalColors();
        Invoke("afterStart",0.1f);
    }
    private void afterStart()
    {
        if(!IsOwner) return;
        GenerateNewPlayerServerRpc();
    }
    void CacheOriginalColors()
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            foreach (Material mat in renderer.materials)
            {
                // 检查材质是否有名为 "_Color" 的属性
                if (mat.HasProperty("_Color"))
                {
                    // 如果材质有 "_Color" 属性且originalColors字典尚未包含该材质
                    if (!originalColors.ContainsKey(mat))
                    {
                        originalColors[mat] = mat.color; // 将原始颜色保存到字典中
                    }
                }
            }
        }
    }
    public float currentHP{
        get{return CurrentHP.Value;}
        set
        {
            currentHPChangedServerRpc(value);
        }
    }
    private void currentHPChanged(float pre,float now)
    {
        // Debug.Log("zuishenyiceng");
        if(!IsOwner) return;
        BloodReset(now);
        if(pre>now) GameObject.Find("Main Camera").GetComponent<CameraFollow>().shake=true;
        // if (!IsOwner) return;
        if (now <= 0)
        {
            //Dying effect
            GameObject cameraObject = GameObject.Find("Main Camera");
            CameraFollow camera = cameraObject.GetComponent<CameraFollow>();
            GameObject[] playerObjects = GameObject.FindGameObjectsWithTag("Player");
            NetworkObject networkObject;
            NetworkObject thisNetworkObject=GetComponent<NetworkObject>();
            foreach (GameObject playerObject in playerObjects)
            {
                networkObject = playerObject.GetComponent<NetworkObject>();
                if (networkObject.NetworkObjectId == thisNetworkObject.NetworkObjectId) continue;
                Camera.main.gameObject.GetComponent<CameraFollow>().targetPlayer = playerObject.transform;
                break;
            }
            GameOverServerRpc();
        }
    }
    [ClientRpc]
    private void CalcFinalRankClientRpc()
    {
        GameObject.Find("ScoreInfo").GetComponent<ScoreManager>().trygameover();
    }
    [ServerRpc(RequireOwnership=false)]
    private void currentHPChangedServerRpc(float now)
    {
        if(now>maxHP) now=maxHP;
        if(now<CurrentHP.Value)
        {
            beingHitClientRpc();
        }
        // Debug.Log("diyiceng");
        // if(!IsServer) Debug.Log("a?");
        // if(IsClient) Debug.Log("buzuole");
        if(now<=0)
        {
            GameObject[] resplayers=GameObject.FindGameObjectsWithTag("Player");
            if(resplayers.Length==1) CalcFinalRankClientRpc();
        }
        CurrentHP.Value=now;
        // Debug.Log("dierceng");
        //GameObject blood=Instantiate(bloodEffect,transform);
        //blood.GetComponent<NetworkObject>().Spawn();
    }
    [ClientRpc]
    private void beingHitClientRpc()
    {
        musicController.PlayGetAttackMusic();
        foreach (var entry in originalColors)
        {
            entry.Key.color = Color.red;
        }
        Invoke("ResetColor", 0.1f);
    }
    private void ResetColor()
    {
        foreach (var entry in originalColors)
        {
            entry.Key.color = entry.Value; // 将颜色重置为原始颜色
        } 
    }
    [ServerRpc]
    void GameOverServerRpc()
    {
        GameOverClientRpc();
    }
    [ClientRpc]
    void GameOverClientRpc()
    {
        Destroy(gameObject);
    }
    public float maxMana=100f;//����
    private float CurrentMana;
    public float currentMana
    {
        get{return CurrentMana;}
        set
        {
            if (value > maxMana) CurrentMana = maxMana;
            else CurrentMana =value;
            ManaReset(CurrentMana);
        }
    }
    public float maxEnergy =5f;
    private float CurrentEnergy =5f;
    public float currentEnergy
    {
        get{return CurrentEnergy;}
        set
        {
            if (value > maxEnergy) CurrentEnergy = maxEnergy;
            else CurrentEnergy=value;
            EnergyReset(CurrentEnergy);
        }
    }
    private NetworkVariable<float> Protection=new NetworkVariable<float>();
    public float protection{
        get{return Protection.Value;}
        set{
            ProtectionChangedServerRpc(value);
        }
    }
    [ServerRpc(RequireOwnership = false)]
    private void ProtectionChangedServerRpc(float now)
    {
        Protection.Value=now;
    }
    public float public_protection;
    private Image bloodImage;
    private Image energyImage;
    private Image manaImage;
    private Image bulletImage;
    private TextMeshProUGUI bulletnumber;
    private float height=5;
    private float oriwidth=60f;
    private TextMeshProUGUI bloodnumber;
    private TextMeshProUGUI mananumber;
    private Canvas cur;
    private int CurrentBulletsInAmmo=30; //现有的上汤的
    private int BulletsRemained=30; //微商堂的子弹
    private int BulletsPerAmmo=30;
    public int currentBulletsInAmmo{
        get{return CurrentBulletsInAmmo;}
        set{
            CurrentBulletsInAmmo=value;
            BulletReset(CurrentBulletsInAmmo,BulletsPerAmmo,BulletsRemained);
        }
    }
    public int bulletsRemained{
        get{return BulletsRemained;}
        set{
            BulletsRemained=value;
            BulletReset(CurrentBulletsInAmmo,BulletsPerAmmo,BulletsRemained);
        }
    }
    public int bulletsPerAmmo{
        get{return BulletsPerAmmo;}
        set{
            BulletsPerAmmo=value;
            BulletReset(CurrentBulletsInAmmo,BulletsPerAmmo,BulletsRemained);
        }
    }
    private void Awake()
    {
        Invoke("RealAwake",0.08f);
    }
    private void RealAwake()
    {
        CurrentHP.OnValueChanged+=currentHPChanged;
        cur=transform.Find("Pictures").GetComponent<Canvas>();
        protection=public_protection;
        // Debug.Log(protection);
        // Debug.Log(public_protection);
        if(!IsOwner)
        {
            cur.enabled=false;
            return;
        }
        bloodImage=cur.transform.Find("Blood").transform.Find("BloodSprite").GetComponentInChildren<Image>();
        energyImage=cur.transform.Find("Energy").transform.Find("EnergySprite").GetComponentInChildren<Image>();
        bloodnumber=cur.transform.Find("Blood").transform.Find("BloodNumber").GetComponentInChildren<TextMeshProUGUI>();
        energyImage.color=Color.yellow;
        currentHP=maxHP;
        currentEnergy=maxEnergy;
        if(Identity.Wizard==identity)
        {
            manaImage=cur.transform.Find("Mana").transform.Find("ManaSprite").GetComponentInChildren<Image>();
            mananumber=cur.transform.Find("Mana").transform.Find("ManaNumber").GetComponentInChildren<TextMeshProUGUI>();
            manaImage.color=mananumber.color=Color.blue;
            currentMana=maxMana;
        }
        if(identity==Identity.Gunslinger)
        {
            bulletImage=cur.transform.Find("Bullet").transform.Find("BulletSprite").GetComponentInChildren<Image>();
            bulletnumber=cur.transform.Find("Bullet").transform.Find("BulletNumber").GetComponentInChildren<TextMeshProUGUI>();
            bulletImage.color=bulletnumber.color=Color.blue;
            BulletReset(CurrentBulletsInAmmo,BulletsPerAmmo,BulletsRemained);
        }
        // Debug.Log("123");
    }
    private void BloodReset(float blood)
    {
        if(bloodImage==null) return;
        if(blood>=((maxHP*2)/3*2)/3&&blood<=maxHP) bloodImage.color=bloodnumber.color=Color.green;
        else if(blood>= maxHP /3&& blood<= (maxHP * 2) / 3) bloodImage.color=bloodnumber.color=Color.yellow;
        else bloodImage.color=bloodnumber.color=Color.red;
        bloodImage.rectTransform.sizeDelta= new Vector2(blood/maxHP*oriwidth,height);
        bloodnumber.text = Mathf.FloorToInt(blood).ToString();//***要修改一下，做一下格式控制
    }
    private void ManaReset(float mana)
    {
        if(identity!=Identity.Wizard) return;
        if(manaImage==null) return;
        manaImage.rectTransform.sizeDelta= new Vector2(mana/maxMana*oriwidth,height);
        mananumber.text = Mathf.FloorToInt(mana).ToString();
    }
    private void EnergyReset(float energy)
    {
        if(energyImage==null) return;
        energyImage.rectTransform.sizeDelta= new Vector2(energy/maxEnergy*oriwidth,height);
    }
    public void DamageEffect(float damage)
    {
        // Debug.Log(protection);
        //if(!IsOwner) return;
        currentHP -= damage * (1f - protection);
    }
    private void BulletReset(int _CurrentBulletsInAmmo,int _BulletsPerAmmo,int _BulletsRemained)
    {
        if(identity!=Identity.Gunslinger) return;
        if(bulletImage==null) return;
        bulletImage.rectTransform.sizeDelta= new Vector2((float)_CurrentBulletsInAmmo/_BulletsPerAmmo*oriwidth,height);
        bulletnumber.text=_CurrentBulletsInAmmo.ToString()+"/"+_BulletsRemained.ToString();
    }
    void OnTriggerStay(Collider collision)
    {
        // Debug.Log("nierlongma");
        if(collision.gameObject.CompareTag("EdgeWall"))
        {
            int buff=Random.Range(2,4);
            currentHP-=(float)buff;
        }
    }
}