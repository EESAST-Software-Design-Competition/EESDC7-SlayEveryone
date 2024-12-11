using System;
using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEditor;

public class GrenadeBurst : NetworkBehaviour
{
    public float delay = 3f; // ��ը�ӳ�
    public float blastRadius = 5f; // ��ը��Χ
    public float damage = 50; // ��ը�˺�

    private float countdown; // ����ʱ
    private NetworkVariable<bool> hasExploded=new NetworkVariable<bool>(false);
    public float force;
    new public Rigidbody rigidbody;
    public GameObject hitvfx;
    Cinemachine.CinemachineImpulseSource source;

    public void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.centerOfMass = transform.position;
    }
    void Start()
    {
        countdown = delay;
        gameObject.GetComponent<Collider>().enabled = false;
    }

    void Update()
    {
        countdown -= Time.deltaTime;
        if (countdown <= 0f && !hasExploded.Value)
        {
            ExplodeServerRpc();
        }
    }
    [ServerRpc(RequireOwnership = false)]
    void ExplodeServerRpc()
    {
        hasExploded.Value = true;
        // ��ʾ��ըЧ������ѧ���ͣ�
        //Instantiate(explosionEffect, transform.position, transform.rotation);
        //PoolManager.Release(hitVFX, collision.GetContact(0).point.Quaternion.LookRotation(collision.GetContact(0).normal * -1f));
        Debug.Log("boom");
        //gizmos
        GameObject effectNode = Instantiate(hitvfx, null);
        effectNode.GetComponent<NetworkObject>().Spawn();
        effectNode.transform.position = this.transform.position;
        // ��ȡ��ը��Χ�ڵ�����3D��ײ��
        Collider[] colliders = Physics.OverlapSphere(transform.position, blastRadius);
        foreach (Collider nearbyObject in colliders)
        {
            // ���������ֵ������۳�����ֵ
            PlayerStateManager playerStateManager = nearbyObject.GetComponent<PlayerStateManager>();
            EnemyStateManager enemyStateManager=nearbyObject.GetComponent<EnemyStateManager>();
            if (playerStateManager != null)
            {
                playerStateManager.DamageEffect(damage);
            }
            else if(enemyStateManager!=null){
                enemyStateManager.DamageEffect(1.5f*damage);
            }
        }
        if (IsServer)
        {
            GetComponent<NetworkObject>().Despawn();
        }
    }

    // public void OnCollisionEnter(Collision collision)
    // {
    //     if(collision.gameObject.name!="Player")
    //     {
    //         PoolManager.Release(hitVFX, collision.GetContact(0).point.Quaternion.LookRotation(collision.GetContact(0).normal * -1f));
    //         rigidbody.isKinematic = true;
    //         StartCoroutine(countdown());

    //     }
    // }

    // �����ڱ༭������ʾ��ը��Χ
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, blastRadius);
    }

    IEnumerator Countdown()
    {
        yield return new WaitForSeconds(3);

    }
    // private void OnTriggerEnter(Collider ground)
    // {
    //     Debug.Log("boom");
    //     //gizmos
    //     GameObject effectNode = Instantiate(hitvfx, null);
    //     effectNode.transform.position = this.transform.position;
    // }

}