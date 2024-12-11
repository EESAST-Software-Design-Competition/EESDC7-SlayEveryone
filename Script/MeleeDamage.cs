using UnityEngine;
using System.Collections;
using UnityEngine.TextCore.Text;
using System.Collections.Generic;
using Unity.Netcode;

public class MeleeWeapon : NetworkBehaviour
{
    public GameObject weaponOwner;
    public float energyRPA = 1f;
    public bool isAttacking = false;
    //protected bool isReadyToAttack = true;

    public float attackPeriod = 1f;//攻击持续时间
    //public float attackGap = 1f;//两次攻击最短间隔
    public float damage = 15.0f;
    private Dictionary<Collider, Coroutine> damageCoroutines = new Dictionary<Collider, Coroutine>();

    public void AttackController()
    {
        isAttacking=true;
            StartCoroutine(Attack());
    }

    IEnumerator Attack()
    {
        yield return new WaitForSeconds(attackPeriod); // ��������1���ʱ��
        StopAllDamageCoroutines(); // ֹͣ���е��˺�Э��
    }
    void OnTriggerStay(Collider other)
    {
        // if (IsOwner) return;//In case we hurt ourselves.
        // Debug.Log("zhuangshangqujiujkaigao");
        // if(!other.CompareTag("Player")&&!other.CompareTag("Enemy")) return;
        // if(other.CompareTag("Player")&&other.GetComponent<NetworkObject>().IsOwner) return;
        // Debug.Log("kenengdaziji");
        if(other.gameObject==weaponOwner) return;
        // Debug.Log("dadaole");
        if (isAttacking)
        // if(other.CompareTag("Player"))
        {
            // Debug.Log("zhong");
            // Debug.Log("dazhongle");
            if (!damageCoroutines.ContainsKey(other))
            {
                Debug.Log("kuaiyaokaikanle");
                Coroutine damageCoroutine = StartCoroutine(DamageCharacter(other, 0.3f));//ÿ0.3�����һ���˺�
                damageCoroutines[other] = damageCoroutine; // �����Э�����ӵ��ֵ���
            }
        }
    }
    IEnumerator DamageCharacter(Collider character, float delay)
    {
        int index=weaponOwner.GetComponent<PlayerStateManager>().playerId;
        // if (!damageCoroutines.ContainsKey(character))
        // yield return new WaitForSeconds(delay);
        while (isAttacking && character)
        {
            if(character.gameObject.CompareTag("Player"))
            {
                // Debug.Log("kaikou");
                PlayerStateManager targetCharacter = character.gameObject.GetComponent<PlayerStateManager>();
                if (targetCharacter.currentHP <= 0)
                {
                    StopCoroutine(damageCoroutines[character]);
                    damageCoroutines.Remove(character);
                    break;
                }
                targetCharacter.DamageEffect(damage);
                GameObject.Find("ScoreInfo").GetComponent<ScoreManager>().ModifyByIndex(index,damage);
            }
            else if(character.gameObject.CompareTag("Enemy"))
            {
                EnemyStateManager targetCharacter = character.gameObject.GetComponent<EnemyStateManager>();
                if (targetCharacter.currentHP <= 0)
                {
                    StopCoroutine(damageCoroutines[character]);
                    damageCoroutines.Remove(character);
                    break;
                }
                targetCharacter.DamageEffect(damage);
                GameObject.Find("ScoreInfo").GetComponent<ScoreManager>().ModifyByIndex(index,damage);
            }
             //core code

            yield return new WaitForSeconds(delay);
        }
        damageCoroutines.Remove(character); // ���ֵ����Ƴ���Э��
        
        
    }

    void OnTriggerExit(Collider other)
    {
       if (other.CompareTag("Player"))
        {
            StopDamageCoroutine(other);
            
        }
        if (damageCoroutines.Count == 0)
            {
                //isAttacking = false;
            }
    }

    void StopAllDamageCoroutines()
    {
        foreach (KeyValuePair<Collider, Coroutine> entry in damageCoroutines)
        {
            StopCoroutine(entry.Value);
        }
        damageCoroutines.Clear(); // ����ֵ�
        isAttacking = false; // ���ù���״̬
    }

    // ������������ֹͣ�ض�������˺�Э��
    void StopDamageCoroutine(Collider target)
    {
        if (damageCoroutines.TryGetValue(target, out Coroutine damageCoroutine))
        {
            StopCoroutine(damageCoroutine);
            damageCoroutines.Remove(target);
        }
    }
}
