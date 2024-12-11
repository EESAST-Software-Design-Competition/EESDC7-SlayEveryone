using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using TMPro;
using UnityEditor;

public class EnemyStateManager : NetworkBehaviour
{
    public GameObject[] candiitems;
    public string playerId;//for network
    public enum Identity
    {
        Wizard, Berserker, Gunslinger//
    }
    public Identity identity;
    public float maxHP=100f;
    private NetworkVariable<float> CurrentHP=new NetworkVariable<float>();
    //public GameObject bloodEffect;
    public Color hitColor = Color.red; // 受伤时的颜色
    private Color originalColor;
    private Dictionary<Material, Color> originalColors = new Dictionary<Material, Color>();
    public MusicController musicController;
    private void Start()
    {
        CacheOriginalColors();
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
    private bool sile=false;
    private void currentHPChanged(float pre,float now)
    {
        if(!IsOwner) return;
        if (now <= 0)
        {
            GameOverServerRpc();
        }
    }
    [SerializeField] private int diao=2;
    [ServerRpc(RequireOwnership=false)]
    private void currentHPChangedServerRpc(float now)
    {
        if(now<CurrentHP.Value)
        {
            beingHitClientRpc();
        }
        // Debug.Log("kouxue");
        // Debug.Log(now);
        if(now<=0&&!sile)
        {
            sile=true;
            int siz=candiitems.Length;
            for(int noi=0;noi<diao;noi++)
            {
                // Debug.Log("zaishengcheng");
                int index=Random.Range(0,siz);
                Vector3 off=new Vector3 (0.5f*noi,0,0.5f*noi);
                GameObject cur=Instantiate(candiitems[index],transform.position+off,Quaternion.identity);
                cur.GetComponent<NetworkObject>().Spawn();
            }
        }
        CurrentHP.Value=now;
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
            CurrentMana=value;
        }
    }
    public float maxEnergy =5f;
    private float CurrentEnergy =5f;
    public float currentEnergy
    {
        get{return CurrentEnergy;}
        set
        {
            CurrentEnergy=value;
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
    private int CurrentBulletsInAmmo=30; //现有的上汤的
    private int BulletsRemained=30; //微商堂的子弹
    private int BulletsPerAmmo=30;
    public int currentBulletsInAmmo{
        get{return CurrentBulletsInAmmo;}
        set{
            CurrentBulletsInAmmo=value;
        }
    }
    public int bulletsRemained{
        get{return BulletsRemained;}
        set{
            BulletsRemained=value;
        }
    }
    public int bulletsPerAmmo{
        get{return BulletsPerAmmo;}
        set{
            BulletsPerAmmo=value;
        }
    }
    private void Awake()
    {
        Invoke("RealAwake",0.08f);
    }
    private void RealAwake()
    {
        CurrentHP.OnValueChanged+=currentHPChanged;
        protection=public_protection;
        // Debug.Log(protection);
        // Debug.Log(public_protection);
        if(!IsOwner) return;
        currentHP=maxHP;
        currentEnergy=maxEnergy;
        if(Identity.Wizard==identity) currentMana=maxMana;
        // Debug.Log("123");
    }
    public void DamageEffect(float damage)
    {
        // Debug.Log(protection);
        //if(!IsOwner) return;
        currentHP -= damage * (1f - protection);
    }
    void OnCollisionStay(Collision collision){
    if(collision.gameObject.CompareTag("EdgeWall")){
        int buff=Random.Range(2,4);
        currentHP-=(float)buff;
    }
}
}