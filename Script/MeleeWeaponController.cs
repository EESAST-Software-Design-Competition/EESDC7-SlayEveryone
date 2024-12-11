using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeaponController : MonoBehaviour
{
    protected bool isAttacking = false;
    protected bool isReadyToAttack = true;

    public float attackPeriod = 1f;//攻击持续时间
    public float attackGap = 2f;//两次攻击最短间隔
   
}
