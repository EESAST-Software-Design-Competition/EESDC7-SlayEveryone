using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Weapon: NetworkBehaviour 
{
    public PlayerStateManager stateUI;
    public Transform bulletSpawn; // �����������ӵ����ɵ�λ��
    public GameObject bulletPrefab;
    public PlayerMovement playerMovement;
    private float bulletSpeed = 25f; // �ӵ����ٶ�
    private float[] bulletSpeedArr = { 25f, 40f, 20f };
    private float[] SpreadIntensityArr= { 30, 0, 80 };//Spread
    private int[] BulletsRemainedArr = { 30, 9, 5 }; //微商堂的子弹
    private int[] CurrentBulletsInAmmoArr = { 30, 9, 5 }; //现有的
    public int[] BulletsPerAmmoArr = { 30, 9, 5 };// 弹夹子弹数
    public float[] ShootingDelayArr = { 0.15f, 1.0f, 1.2f };
    public float[] damageArr = { 4f, 15f, 3.75f };
    private float SpreadIntensity = 30;
    private int BulletsRemained = 30; //微商堂的子弹
    private int CurrentBulletsInAmmo = 30; //现有的
    public int BulletsPerAmmo = 30;// 弹夹子弹数
    public float damage = 4f;
    public MusicController musicController;
    public int bulletsRemained{
        get{return BulletsRemained;}
        set
        {
            stateUI.bulletsRemained=BulletsRemained=value;
        }
    }
    public int bulletsPerAmmo{
        get{return BulletsPerAmmo;}
        set
        {
            stateUI.bulletsPerAmmo=BulletsPerAmmo=value;
        }
    }
    public int currentBulletsInAmmo{
        get{return CurrentBulletsInAmmo;}
        set
        {
            stateUI.currentBulletsInAmmo=CurrentBulletsInAmmo=value;
        }
    }
    public bool isReloading = false;
    public float reloadingTime = 3f;
    bool readyToShoot = true;

    public enum ShootingMode
    {
        Auto=0,Single=1,Burst=2
    }
    private ShootingMode _shootingMode=ShootingMode.Auto;
    public ShootingMode shootingMode
    {
        get { return _shootingMode;}
        set
        {
            CurrentBulletsInAmmoArr[(int)(_shootingMode)] = currentBulletsInAmmo;
            BulletsRemainedArr[(int)_shootingMode] = bulletsRemained;
            _shootingMode = value;
            bulletsRemained = BulletsRemainedArr[(int)(_shootingMode)];
            bulletsPerAmmo = BulletsPerAmmoArr[(int)(_shootingMode)];
            currentBulletsInAmmo = CurrentBulletsInAmmoArr[(int)(_shootingMode)];
            SpreadIntensity = SpreadIntensityArr[(int)(_shootingMode)];
            bulletSpeed = bulletSpeedArr[(int)(_shootingMode)];
            shootingDelay = ShootingDelayArr[(int)(_shootingMode)];
            damage = damageArr[(int)(_shootingMode)];
        }
    }
    public float shootingDelay = 0.2f;
    public int bulletsPerBurst =7;
    // Update is called once per frame
    void Update()
    {
        if (isReloading||!readyToShoot||!IsOwner)//����Ƿ������ϵ�
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.R)&&currentBulletsInAmmo<bulletsPerAmmo)
        {
            if (bulletsRemained > bulletsPerAmmo - currentBulletsInAmmo)
            {
                bulletsRemained -= (bulletsPerAmmo - currentBulletsInAmmo);
                currentBulletsInAmmo = bulletsPerAmmo;
            }
            else if(bulletsRemained>0)
            {
                currentBulletsInAmmo += bulletsRemained;
                bulletsRemained = 0;
            }
            isReloading = true;
            StartCoroutine(HaveReloaded());
        }
        
        if(shootingMode == ShootingMode.Single)// ������������ʱ�����ӵ�
        {
            if (Input.GetButtonDown("Fire1") && currentBulletsInAmmo > 0)
            {
                if (playerMovement.checkdefend == true)
                {
                    Debug.Log("noHello");
                    musicController.PlayerAutoModeAttackMusic();
                    ShootForSingleAndAuto();
                    playerMovement.checkdefend = false;
                    Invoke("AgainForSingle", 0.1f);
                    Invoke("ResetCheckDefend", 5f);
                }
                else
                {
                    musicController.PlayAttackMusic();
                    ShootForSingleAndAuto();
                }
            }
        }else if(shootingMode == ShootingMode.Auto)
        {
            if (Input.GetButton("Fire1")&& currentBulletsInAmmo > 0)
            {
                    musicController.PlayerAutoModeAttackMusic();
                    ShootForSingleAndAuto();
            }
        }
        else if(shootingMode == ShootingMode.Burst)
        {
            if (Input.GetButtonDown("Fire1") && currentBulletsInAmmo > 0)
            {
                if(playerMovement.checkdefend == true)
                {
                    Debug.Log("noHello");
                    musicController.PlayAttackMusic();
                    ShootForBurst();
                    playerMovement.checkdefend = false;
                    Invoke("AgainForBurst", 0.15f);
                    Invoke("ResetCheckDefend", 7f);
                }
                else
                {
                    musicController.PlayAttackMusic();
                    ShootForBurst();
                }
            }
        }
    }
    void AgainForSingle()
    {
        currentBulletsInAmmo ++;
        musicController.PlayerAutoModeAttackMusic();
        ShootForSingleAndAuto();
        Invoke("AgainAndAgainForSingle",0.1f);
    }
    void AgainAndAgainForSingle(){
        musicController.PlayerAutoModeAttackMusic();
        ShootForSingleAndAuto();
    }
    void AgainForBurst()
    {
        currentBulletsInAmmo++;
        musicController.PlayAttackMusic();
        ShootForBurst();
        Invoke("AgainAndAgainForBurst",0.15f);
    }
    void AgainAndAgainForBurst(){
        musicController.PlayAttackMusic();
        ShootForBurst();
    }
    void ResetCheckDefend()
    {
        playerMovement.guncheckreadydefend = true;

    }
    void ShootForSingleAndAuto()
    {
        currentBulletsInAmmo--;
        Debug.Log(currentBulletsInAmmo);
        // ��bulletSpawnλ������bulletPrefabʵ��
        //GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, bulletSpawn.rotation);
        ShootForSingleServerRpc(bulletSpawn.position,bulletSpawn.rotation,bulletSpawn.forward);

        // ��ȡ�ӵ���Rigidbody������������ʼ�ٶ�
        // Rigidbody rb = bullet.GetComponent<Rigidbody>();
        // if (rb != null)
        // {
        //     float x = UnityEngine.Random.Range(-spreadIntensity / 100, spreadIntensity / 100);
        //     //float y = UnityEngine.Random.Range(-spreadIntensity / 100, spreadIntensity / 100);
        //     rb.velocity = (bulletSpawn.forward + new Vector3(x, 0, 0))* bulletSpeed;  // �ӵ�����������ǰ������
        // }
        readyToShoot = false;
        Invoke("ResetShoot", shootingDelay);
    }
    void ShootForBurst()
    {
        currentBulletsInAmmo--;
        
        
        ShootForBurstServerRpc(bulletSpawn.position,bulletSpawn.rotation,bulletSpawn.forward);
        
        readyToShoot = false;
        Invoke("ResetShoot", shootingDelay);
    }
    
    private IEnumerator HaveReloaded()
    {
        yield return new WaitForSeconds(reloadingTime);
        isReloading = false;
    }
    private void ResetShoot()
    {
        readyToShoot=true;
    }
    [ServerRpc]
    private void ShootForSingleServerRpc(Vector3 pos,Quaternion rot,Vector3 dir)
    {
        GameObject bullet = Instantiate(bulletPrefab, pos,rot);
        bullet.GetComponent<NetworkObject>().Spawn();
        bullet.GetComponent<Bullet>().SetSourceID((int)OwnerClientId);
        bullet.GetComponent<Bullet>().SetSourcePlayerID(stateUI.playerId);
        bullet.GetComponent<Bullet>().SetDamage(damage);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        float x = UnityEngine.Random.Range(-SpreadIntensity / 100, SpreadIntensity / 100);
        //float y = UnityEngine.Random.Range(-spreadIntensity / 100, spreadIntensity / 100);
        rb.velocity = (dir + new Vector3(x, 0, 0)).normalized * bulletSpeed;
    }
    GameObject[] bullets;
    [ServerRpc]
    private void ShootForBurstServerRpc(Vector3 pos,Quaternion rot,Vector3 dir)
    {
        int seed = System.Environment.TickCount;
        Vector3 perpendicularVectorCW = new Vector3(dir.z, 0, -dir.x);
        //GameObject[] bullets=new GameObject[bulletsPerBurst];
        bullets = new GameObject[bulletsPerBurst];
        for (int i = 0; i < bulletsPerBurst; i++)
        {
        //int i = 0;
            seed += 1;
            bullets[i] = Instantiate(bulletPrefab, pos, rot);
            bullets[i].GetComponent<Bullet>().SetSourceID((int)OwnerClientId);
            bullets[i].GetComponent<Bullet>().SetDamage(damage);
            bullets[i].GetComponent<Bullet>().SetSourcePlayerID(stateUI.playerId);
            bullets[i].GetComponent<NetworkObject>().Spawn();
            Debug.Log("Hello");
            Rigidbody rb = bullets[i].GetComponent<Rigidbody>();
            UnityEngine.Random.InitState(seed);
            float x = UnityEngine.Random.Range(-SpreadIntensity / 100, SpreadIntensity / 100);
            //x = 0f;
            //float y = UnityEngine.Random.Range(-spreadIntensity / 100, spreadIntensity / 100);
            rb.velocity = (dir+x*perpendicularVectorCW.normalized).normalized * bulletSpeed;
        }
    }
    // public bool Shoot()
    // {
    //     if (readyToShoot)
    //     {
    //         ShootServerRpc(bulletSpawn.position,bulletSpawn.rotation,direction);
    //         readyToShoot = false;
    //         Invoke("ResetShoot", shootingDelay);
    //         return true;
    //     }
    //     else { return false; }
    // }
}

