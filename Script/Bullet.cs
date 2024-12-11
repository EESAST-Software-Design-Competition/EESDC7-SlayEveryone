using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Unity.Netcode;
public class Bullet : NetworkBehaviour
{
    private NetworkVariable<int> sourceId=new NetworkVariable<int>(-1);
    private NetworkVariable<int> sourcePlayerId=new NetworkVariable<int>(-1);
    private NetworkVariable<float> damage = new NetworkVariable<float>(4f);
    public void SetDamage(float cur)
    {
        damage.Value = cur;
    }
    public void SetSourceID(int cur)
    {
        sourceId.Value=cur;
    }
    public void SetSourcePlayerID(int cur)
    {
        sourcePlayerId.Value=cur;
    }
    public float lifetime = 3f; // �ӵ����ڵ�ʱ�䣬֮����Զ�����
    //public GameObject bulletImpactEffectPrefab;
    private void Start()
    {
        lifetime -= 0.08f;
        Invoke("RealStart", 0.08f);
        
    }
    void RealStart()
    {
        if (IsServer)
        {
            StartCoroutine(DestroyAfterTime());
        }
    }

    private IEnumerator DestroyAfterTime()
    {
        yield return new WaitForSeconds(lifetime);
        GetComponent<NetworkObject>().Despawn();
    }

    //private void OnCollisionEnter(Collision collision)
    //{
    //    if (collision.gameObject.CompareTag("Enemy"))
    //    {
    //        Destroy(collision.gameObject); // ���ٱ����еĵ���
    //        Destroy(gameObject); // �����ӵ�
    //    }
    //    if (collision.gameObject.CompareTag("Wall"))
    //    {
    //        Destroy(gameObject);
    //    }
    //}
    private NetworkObject gai;
    private float chi=0.12f;
    private IEnumerator DespawnCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        if(gai.IsSpawned) gai.Despawn();
    }
    private void cuihui()
    {
        gai=GetComponent<NetworkObject>();
        // Debug.Log("kehuduanbuyinggaidaozheli");
        GetComponent<MeshRenderer>().enabled=false;
        GetComponent<TrailRenderer>().enabled=false;
        GetComponent<Collider>().enabled=false;
        StartCoroutine(DespawnCoroutine(chi));
    }
    private void OnTriggerEnter(Collider collision)
    {
        if(!IsServer) return;
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("dazhongle");
            PlayerStateManager psm=collision.gameObject.GetComponent<PlayerStateManager>();
            NetworkObject networkObject=collision.gameObject.GetComponent<NetworkObject>();
            if((int)(networkObject.OwnerClientId)==sourceId.Value||sourceId.Value==-1) return;
            psm.DamageEffect(damage.Value);
            GameObject.Find("ScoreInfo").GetComponent<ScoreManager>().ModifyByIndex(sourcePlayerId.Value,damage.Value);
            if(IsServer)
            {
                if (GetComponent<NetworkObject>().IsSpawned) cuihui();
            }
        }
        else if(collision.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("dazhongle,yinggaibian");
            EnemyStateManager psm=collision.gameObject.GetComponent<EnemyStateManager>();
            NetworkObject networkObject=collision.gameObject.GetComponent<NetworkObject>();
            psm.DamageEffect(damage.Value);
            GameObject.Find("ScoreInfo").GetComponent<ScoreManager>().ModifyByIndex(sourcePlayerId.Value,damage.Value);
            if(IsServer)
            {
                if (GetComponent<NetworkObject>().IsSpawned) cuihui();
            }
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(!IsServer) return;
        if(collision.gameObject.CompareTag("PlayerBody")||collision.gameObject.CompareTag("Bullet")) return;
        if (GetComponent<NetworkObject>().IsSpawned) cuihui();
    }
    //private void CreateBulletImpactEffect(Collision objectWeHit)
    //{
    //   ContactPoint contact = objectWeHit.contacts[0];
    //   GameObject hole = Instantiate(bulletImpactEffectPrefab,
    //      contact.point,
    //       Quaternion.LookRotation(contact.normal));
    //   hole.transform.SetParent(objectWeHit.gameObject.transform);
    //}

}