using UnityEngine;
using System.Collections;
using UnityEngine.TextCore.Text;
using System.Collections.Generic;
using Unity.Netcode;

public class EnemyMeleeWeapon : NetworkBehaviour
{
    public GameObject weaponOwner;
    public float energyRPA = 1f;
    public bool isAttacking = false;
    private Dictionary<Collider,bool> done=new Dictionary<Collider, bool>();
    //protected bool isReadyToAttack = true;

    public float attackPeriod = 3f;//攻击持续时间
    //public float attackGap = 1f;//两次攻击最短间隔
    public float damage = 15.0f;
    private Dictionary<Collider, Coroutine> damageCoroutines = new Dictionary<Collider, Coroutine>();

    public void AttackController()
    {
        isAttacking=true;
        done.Clear();
        StartCoroutine(Attack());
    }

    IEnumerator Attack()
    {
        yield return new WaitForSeconds(attackPeriod); // ��������1���ʱ��
        StopAllDamageCoroutines(); // ֹͣ���е��˺�Э��
    }
    private void OnTriggerEnter(Collider collision)
    {
        if(collision.gameObject==weaponOwner) return;
        if(!IsServer) return;
        if (collision.gameObject.CompareTag("Player"))
        {
            if(done.ContainsKey(collision)) return;
            done[collision]=true;
            PlayerStateManager psm=collision.gameObject.GetComponent<PlayerStateManager>();
            //NetworkObject networkObject=collision.gameObject.GetComponent<NetworkObject>();
            psm.DamageEffect(damage);
        }
        
    }
    // void OnTriggerStay(Collider other)
    // {
    //     // if (IsOwner) return;//In case we hurt ourselves.
    //     // Debug.Log("zhuangshangqujiujkaigao");
    //     // if(!other.CompareTag("Player")&&!other.CompareTag("Enemy")) return;
    //     // if(other.CompareTag("Player")&&other.GetComponent<NetworkObject>().IsOwner) return;
    //     // Debug.Log("kenengdaziji");
    //     if(other.gameObject==weaponOwner) return;
    //     // Debug.Log("dadaole");
    //     if (isAttacking)
    //     // if(other.CompareTag("Player"))
    //     {
    //         // Debug.Log("zhong");
    //         // Debug.Log("dazhongle");
    //         if (!damageCoroutines.ContainsKey(other))
    //         {
    //             Debug.Log("kuaiyaokaikanle");
    //             Coroutine damageCoroutine = StartCoroutine(DamageCharacter(other, 0.05f));//ÿ0.3�����һ���˺�
    //             damageCoroutines[other] = damageCoroutine; // �����Э�����ӵ��ֵ���
    //         }
    //     }
    // }
    // IEnumerator DamageCharacter(Collider character, float delay)
    // {
    //     Debug.Log("dengxiakaikan");
    //     if (!damageCoroutines.ContainsKey(character))
    //     yield return new WaitForSeconds(delay);
    //     while (isAttacking && character)
    //     {
    //         Debug.Log("mashangkaikou");
    //         if(character.gameObject.CompareTag("Player"))
    //         {
    //             Debug.Log("kaikou");
    //             PlayerStateManager targetCharacter = character.gameObject.GetComponent<PlayerStateManager>();
    //             if (targetCharacter.currentHP <= 0)
    //             {
    //                 StopCoroutine(damageCoroutines[character]);
    //                 damageCoroutines.Remove(character);
    //                 break;
    //             }
    //             targetCharacter.DamageEffect(damage);
    //         }
    //         else if(character.gameObject.CompareTag("Enemy"))
    //         {
    //             EnemyStateManager targetCharacter = character.gameObject.GetComponent<EnemyStateManager>();
    //             if (targetCharacter.currentHP <= 0)
    //             {
    //                 StopCoroutine(damageCoroutines[character]);
    //                 damageCoroutines.Remove(character);
    //                 break;
    //             }
    //             targetCharacter.DamageEffect(damage);
    //         }
    //          //core code

    //         yield return new WaitForSeconds(delay);
    //     }
    //     damageCoroutines.Remove(character); // ���ֵ����Ƴ���Э��
        
        
    // }

    

    void StopAllDamageCoroutines()
    {
        
        isAttacking = false; // ���ù���״̬
    }

    // ������������ֹͣ�ض�������˺�Э��
    
}
