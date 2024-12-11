using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.Netcode;
using Unity.VisualScripting;

public class EnemyMovement : NetworkBehaviour
{
    public Transform body;
    public float speed = 120f;
    public float exhaustingRate = 1f;
    private float fixedYPosition;
    public Rigidbody rb;
    bool isMoving;
    float stopTime=4f;
    private Animator anim;
    private NavMeshAgent agent;
    public int enemyid;
    public RawNumber rec;
    public bool CustomIsOwner = false;
    [SerializeField] private bool checkmove;
    [SerializeField] private bool checkshift;
    [SerializeField] private bool checkdefend;
    [SerializeField] private bool checkreload = false;
    public bool checkattack;
    private int combocheck;
    private float lastProtection;

    Vector3 velocity;
    private Vector3 lastPosition = new Vector3(0f, 0f, 0f);
    public MagicWeapon magicWeapon;//Used for magicweapon
    public Weapon weapon;//Used for slinger weapon
    public EnemyMeleeWeapon meleeWeapon;
    public EnemyStateManager enemyStateManager;
    public MusicController musicController;
    void Start()
    {
        if(enemyid==2){
        // 获取该 GameObject 及其所有子对象（无论它们是否被禁用）的所有渲染器
        Renderer[] renderers = GetComponentsInChildren<Renderer>(true);

        // 遍历所有渲染器并将它们禁用
        foreach (Renderer r in renderers)
        {
            r.enabled = false;
        }
        }
        enemyStateManager = GetComponent<EnemyStateManager>();
        lastProtection = enemyStateManager.public_protection;
        fixedYPosition = transform.position.y;
        anim = GetComponentInChildren<Animator>();
    }
    [ServerRpc(RequireOwnership = false)]
    private void IllegalRecycleServerRpc()
    {
        GetComponent<NetworkObject>().Despawn();
    }
    private void Awake()
    {
        // Invoke("OwnershipTransform",0.1f);
        // StartCoroutine(WaitForCorrectlySet());
        // Invoke("RealAwake",0.05f);
    }
    // IEnumerator WaitForCorrectlySet()
    // {
    // while (OwnerPlayerID==0)
    // {
    // yield return null;
    // }
    // }
    private void RealAwake()
    {
        // Debug.Log(OwnerPlayerID);
        // NetworkObject[] networkObjects = FindObjectsOfType<NetworkObject>();
        // foreach (NetworkObject networkObject in networkObjects)
        // {
        //     Debug.Log(networkObject.NetworkObjectId);
        //     if (networkObject.NetworkObjectId==OwnerPlayerID&&networkObject.IsLocalPlayer)
        //     {
        //         CustomIsOwner=true;
        //         break;
        //     }
        // }
    }

    // Update is called once per frame
    private float attacklimit = 2f;
    private EnemyMovement enemy;
    private Transform Findnearest(Transform cur)
    {
        // Debug.Log("jinlai");
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        //Debug.Log(players.Length);
        if (players.Length == 0) return cur;
        float xiao = Vector3.Distance(players[0].transform.position, cur.position);
        Transform des = players[0].transform;
        foreach (GameObject player in players)
        {
            float dis = Vector3.Distance(player.transform.position, cur.position);
            if (dis < xiao)
            {
                xiao = dis;
                des = player.transform;
            }
        }
        return des;
    }
    public NavMeshAgent navagent;
    private float baozhalimit=2f;
    [ClientRpc]
    private void baozhaClientRpc()
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>(false);
        foreach (Renderer r in renderers)
        {
            r.enabled = true;
        }
        GrenadeBurst grenadeBurst = GetComponent<GrenadeBurst>();
        grenadeBurst.enabled = true;
    }
    private bool zhale=false;
    void Update()
    {
        // if(getOwner==null||networkObject==null||gameIDManager==null) return;
        // if(!CustomIsOwner) return;
        if (!IsOwner) return;
        GetComponent<Rigidbody>().isKinematic=true;
        GetComponent<Rigidbody>().isKinematic=false;
        NavMeshAgent navagent = GetComponent<NavMeshAgent>();
        // if(navagent==null||enemy==null) return;
        if (navagent == null) return;
        // Debug.Log("zaizhao");
        Transform lai = Findnearest(navagent.transform);
        // Debug.Log(lai);
        if(enemyid==2||!meleeWeapon.isAttacking)
        {//后摇
            navagent.destination = lai.position;
        }
        float dis = Vector3.Distance(transform.position, lai.position);
        // Debug.Log(dis);
        if(enemyid==2&&dis<baozhalimit&&!zhale)
        {
            Debug.Log("kanaqingchu");
            Debug.Log(dis);
            navagent.speed/=2f;
            baozhaClientRpc();
            zhale=true;
        }
        checkmove = false;
        checkattack = false;
        //checkdefend = false;
        float x = 0f, z = 0f, movespeed = speed;isMoving=true;
        if(enemyStateManager.identity==EnemyStateManager.Identity.Berserker&&!checkdefend)
        {
            enemyStateManager.protection = lastProtection;
        }
        checkdefend=false;
        if (!checkmove && !checkdefend&&!checkattack)
        {
            if (stopTime < 0f)
            {
                isMoving = false;//Used for energy restoring
            }
            else
            {
                stopTime -= Time.deltaTime;
            }
        }
        else
        {
            stopTime = 1.5f;
        }
        if (enemyStateManager.currentEnergy <= enemyStateManager.maxEnergy && !checkmove &&!checkdefend&&!isMoving)
        {
            enemyStateManager.currentEnergy += exhaustingRate * Time.deltaTime;
        }
        Vector3 moveDirection = new Vector3(x, 0, z);
        if (moveDirection != Vector3.zero)
        {
            velocity = Vector3.Normalize(moveDirection) * movespeed * Time.deltaTime;
        }
        else
        {
            velocity = Vector3.zero;
        }
        //velocity.y += gravity * Time.deltaTime;

        rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);
        //if (lastPosition != gameObject.transform.position)
        //{
        //    isMoving = true;
        //}
        //else
        //{
        //    stopTime = 1f;
        //    isMoving = false;
        //}

        //lastPosition = gameObject.transform.position;
        if (dis < attacklimit)
        {
            switch (enemyStateManager.identity)
            {
                case EnemyStateManager.Identity.Wizard:
                    if (magicWeapon.readyToShoot)
                    {
                        checkattack = true;
                        magicWeapon.Shoot();
                    }
                    break;
                case EnemyStateManager.Identity.Berserker:
                    if (!meleeWeapon.isAttacking)
                    {
                        // Debug.Log("kaikan");
                        checkattack = true;
                        meleeWeapon.AttackController();
                        musicController.PlayAttackMusic();
                    }
                    break;
                case EnemyStateManager.Identity.Gunslinger:
                    checkattack = true;
                    break;
            }
        }
        if(navagent.velocity.magnitude>0){
            checkmove=true;
        }

        if (weapon != null) checkreload = weapon.isReloading;

        anim.SetBool("checkmove", checkmove);
        anim.SetBool("checkattack", checkattack);
    }
    public void Attack()
    {
        switch (enemyStateManager.identity)
            {
                case EnemyStateManager.Identity.Wizard:
                    if (magicWeapon.readyToShoot)
                    {
                        checkattack = true;
                        magicWeapon.Shoot();
                    }
                    break;
                case EnemyStateManager.Identity.Berserker:
                if(!meleeWeapon.isAttacking)
                    {
                    Debug.Log("kaikan");
                        checkattack = true;
                        meleeWeapon.AttackController();
                        musicController.PlayAttackMusic();
                    }
                    break;
                case EnemyStateManager.Identity.Gunslinger:
                    checkattack = true;
                    break;
            }
    }
   
    public void AttackOver()
    {
        checkattack = false;
    }


    public void RotateToTarget(Transform cur)
    {
        // ... (Ray, Plane, distance, point �����������ͳ�ʼ��)

        Ray ray = Camera.main.ScreenPointToRay(cur.position);
        Plane groundPlane = new Plane(Vector3.up, body.position);
        float rayDistance;

        // Raycast ����������λ��
        if (groundPlane.Raycast(ray, out rayDistance))
        {
            Vector3 point = ray.GetPoint(rayDistance);
            Vector3 direction = (point - body.position).normalized;
            direction.y = 0; // ȷ�� y ������0��������תֻ������Y��

            // ���峯�����ķ���
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            body.rotation = Quaternion.Slerp(body.rotation, lookRotation, Time.deltaTime * 10); // 10Ϊ��ת�ٶȣ����Ը��������Ϸ��Ҫ���е���
            if (magicWeapon != null) magicWeapon.direction = direction;
        }
    }
    //IEnumerator BerserkerResetProtection(float lastProtection,float delay)
    //{
    //    yield return new WaitForSeconds(delay);
    //    enemyStateManager.protection = lastProtection;
    //}
}
