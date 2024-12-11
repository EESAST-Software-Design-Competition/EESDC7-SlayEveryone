using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.Netcode;
using Unity.VisualScripting;

public class PlayerMovement : NetworkBehaviour
{
    public Transform body;
    public float speed = 120f;
    public float exhaustingRate = 1f;
    //private float gravity = -9.81f * 2f;
    private float fixedYPosition;
    public Rigidbody rb;
    bool isMoving;
    float stopTime=4f;
    private Animator anim;
    private NavMeshAgent agent;
    public RawNumber rec;
    public bool CustomIsOwner = false;
    [SerializeField] private bool checkmove;
    [SerializeField] private bool checkshift;
    [SerializeField] public  bool checkdefend;
    [SerializeField] private bool checkreload = false;
    public bool checkattack;
    [SerializeField] public bool guncheckreadydefend;
    private int combocheck;
    private float lastProtection;

    Vector3 velocity;
    private Vector3 lastPosition = new Vector3(0f, 0f, 0f);
    public MagicWeapon magicWeapon;//Used for magicweapon
    public ItemManager itemManager;
    public Weapon weapon;//Used for slinger weapon
    public MeleeWeapon meleeWeapon;
    public PlayerStateManager playerStateManager;
    public MusicController musicController;
    public WizardParticleSystemP wizardParticleSystemP1;
    public WizardParticleSystemP wizardParticleSystemP2;

    private float wizardDamage;
    private float wizardMagicRequired;
    void Start()
    {
        itemManager = GetComponent<ItemManager>();
        playerStateManager = GetComponent<PlayerStateManager>();
        lastProtection = playerStateManager.public_protection;
        fixedYPosition = transform.position.y;
        anim = GetComponentInChildren<Animator>();
        if(playerStateManager.identity==PlayerStateManager.Identity.Wizard)
        {
            wizardDamage = magicWeapon.damage;
            wizardMagicRequired = magicWeapon.magicRequiredPerFire;
            ParticleSystem[] particleSystems = GetComponentsInChildren<ParticleSystem>();
            foreach (ParticleSystem ps in particleSystems)
            {
                ps.Stop();
            }
        }
        else if (playerStateManager.identity == PlayerStateManager.Identity.Gunslinger)
        {
            guncheckreadydefend = true;
        }
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
    public bool meichong = true;
    private float consumeofchong=1f;
    // Update is called once per frame
    void Update()
    {
        // if(getOwner==null||networkObject==null||gameIDManager==null) return;
        // if(!CustomIsOwner) return;
        if (!IsOwner) return;
        GetComponent<Rigidbody>().isKinematic=true;
        GetComponent<Rigidbody>().isKinematic=false;
        checkmove = false;
        checkshift = false;
        checkattack = false;
        //checkdefend = false;
        float x = 0f, z = 0f, movespeed = speed;isMoving=true;
        if (Input.GetKey(KeyCode.A))
        {
            x += -1f;
            checkmove = true;
        }
        if (Input.GetKey(KeyCode.D))
        {
            x += 1f;
            checkmove = true;
        }
        if (Input.GetKey(KeyCode.W))
        {
            z += 1f;
            checkmove = true;
        }
        if (Input.GetKey(KeyCode.S))
        {
            z += -1f;
            checkmove = true;
        }
        if (Input.GetKey(KeyCode.LeftShift) && playerStateManager.currentEnergy > 0f&&checkmove)
        {
            movespeed *= 1.5f;
            playerStateManager.currentEnergy -= exhaustingRate * Time.deltaTime;
            checkshift = true;
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
                switch (playerStateManager.identity)
                {
                    case PlayerStateManager.Identity.Wizard:
                        if (magicWeapon.readyToShoot)
                        {
                            checkattack = true;
                            magicWeapon.Shoot();
                        }
                        break;
                    case PlayerStateManager.Identity.Berserker:
                        if (playerStateManager.currentEnergy >= meleeWeapon.energyRPA&& !meleeWeapon.isAttacking)
                        {
                        if (!checkdefend)
                        {
                            playerStateManager.currentEnergy -= meleeWeapon.energyRPA * exhaustingRate;
                            checkattack = true;
                            meleeWeapon.AttackController();
                            musicController.PlayAttackMusic();
                        }
                        }
                        break;
                    case PlayerStateManager.Identity.Gunslinger:
                        checkattack = true;
                        break;
            }
        }
        if(playerStateManager.identity==PlayerStateManager.Identity.Berserker&&!checkdefend)
        {
            playerStateManager.protection = lastProtection;
        }
        if (Input.GetKey(KeyCode.Mouse1))
        {
            switch (playerStateManager.identity)
            {
                case PlayerStateManager.Identity.Wizard:
                    if (playerStateManager.currentMana > 0.1f&&magicWeapon.damage<60f)
                    {
                        checkdefend = true;
                        wizardParticleSystemP1.SwitchOn();
                        playerStateManager.currentMana-=10*Time.deltaTime/2.5f;
                        magicWeapon.damage += magicWeapon.damage*Time.deltaTime/2.5f;
                        magicWeapon.magicRequiredPerFire += 0.5f * magicWeapon.magicRequiredPerFire * Time.deltaTime / 2.5f;
                    }
                    else if (playerStateManager.currentMana > 0.1f && magicWeapon.damage >= 60f)
                    {
                        wizardParticleSystemP2.SwitchOn();
                        playerStateManager.currentMana -= 1f * Time.deltaTime;
                    }
                    else
                    {
                        magicWeapon.damage = wizardDamage;
                        magicWeapon.magicRequiredPerFire = wizardMagicRequired;
                        wizardParticleSystemP1.SwitchOff();
                        wizardParticleSystemP2.SwitchOff();
                        checkdefend = false;
                    }
                    break;
                case PlayerStateManager.Identity.Berserker:
                    if (playerStateManager.currentEnergy > 0f)
                    {
                        checkdefend = true;
                        playerStateManager.currentEnergy -= 0.3f*exhaustingRate * Time.deltaTime;
                        playerStateManager.protection = 0.95f;
                        movespeed /= 2f;
                    }
                    // else if(!checkdefend)
                    // {
                    //     playerStateManager.protection = lastProtection;
                    // }
                    break;
                case PlayerStateManager.Identity.Gunslinger:
                    if(guncheckreadydefend)
                    {
                        if (weapon.currentBulletsInAmmo >= 2)
                        {
                            Debug.Log("Hello");
                            checkdefend= true;
                            guncheckreadydefend = false;
                        }
                    }
                    break;
            }
        }
        else
        {
            switch (playerStateManager.identity)
            {
                case PlayerStateManager.Identity.Wizard:
                    magicWeapon.damage = wizardDamage;
                    magicWeapon.magicRequiredPerFire = wizardMagicRequired;
                    wizardParticleSystemP1.SwitchOff();
                    wizardParticleSystemP2.SwitchOff();
                    checkdefend = false;
                    break;
                case PlayerStateManager.Identity.Berserker:
                    checkdefend = false;
                    break;
                case PlayerStateManager.Identity.Gunslinger:
                    break;
            }
        }
        if (!checkmove && !checkdefend&&!checkattack&&meichong)
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
        if (playerStateManager.currentEnergy <= playerStateManager.maxEnergy && !checkmove &&!checkdefend&&!isMoving&&meichong)
        {
            playerStateManager.currentEnergy += exhaustingRate * Time.deltaTime;
        }
        Vector3 moveDirection = new Vector3(x, 0, z);
        if (meichong)
        {
            if (moveDirection != Vector3.zero)
            {
                velocity = Vector3.Normalize(moveDirection) * movespeed * Time.deltaTime;
            }
            else
            {
                velocity = Vector3.zero;
            }
        }
        //velocity.y += gravity * Time.deltaTime;

        if (body != null)
        {
            RotateToMouse();
        }

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

        if (weapon != null) checkreload = weapon.isReloading;

        anim.SetBool("checkmove", checkmove);
        anim.SetBool("checkshift", checkshift);
        anim.SetBool("checkattack", checkattack);
        anim.SetBool("checkdefend", checkdefend);
        anim.SetBool("checkreload", checkreload);
        anim.SetInteger("combocheck", combocheck);

        // Debug.Log(isMoving);
        // Debug.Log(stopTime);
        if(Input.GetKeyDown(KeyCode.E)) {
            switch(playerStateManager.identity)
            {
                case PlayerStateManager.Identity.Gunslinger:
                    if (weapon != null)
                    {
                        weapon.shootingMode = (Weapon.ShootingMode)((int)(weapon.shootingMode + 1) % 3);
                    }
                    break;
                case PlayerStateManager.Identity.Berserker:
                    if (!meichong) break;
                    if (playerStateManager.currentEnergy > consumeofchong*exhaustingRate)
                    {
                        playerStateManager.currentEnergy -= consumeofchong*exhaustingRate;
                    }
                    else break;
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    Plane groundPlane = new Plane(Vector3.up, body.position);
                    float rayDistance;

                    if (groundPlane.Raycast(ray, out rayDistance))
                    {
                        Vector3 point = ray.GetPoint(rayDistance);
                        Vector3 direction = (point - body.position);
                        direction.y = 0; 
                        direction=Vector3.Normalize(direction);
                        direction = impulsespeed * direction;
                        velocity = direction;
                        meichong = false;
                        StartCoroutine(chongwanle());
                    }
                    break;
                case PlayerStateManager.Identity.Wizard:
                    magicWeapon.WizardBallGenerate();
                    break;
            }

        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (playerStateManager.identity == PlayerStateManager.Identity.Berserker && !meichong)
        {
            meichong = true;
            rb.MovePosition(rb.position - 10*velocity * Time.fixedDeltaTime);
            velocity = Vector3.zero;
        }
    }

    private IEnumerator chongwanle()
    {
        yield return new WaitForSeconds(timeofchong);
        meichong = true;

    }
    private float timeofchong = 0.25f;
    private float impulsespeed = 0.85f;
   
    public void AttackOver()
    {
        checkattack = false;
    }


    void RotateToMouse()
    {
        // ... (Ray, Plane, distance, point �����������ͳ�ʼ��)

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
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
            itemManager.direction = direction;
        }
    }
    //IEnumerator BerserkerResetProtection(float lastProtection,float delay)
    //{
    //    yield return new WaitForSeconds(delay);
    //    playerStateManager.protection = lastProtection;
    //}
}
