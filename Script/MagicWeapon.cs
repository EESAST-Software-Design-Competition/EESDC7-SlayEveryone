using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.VFX;

public class MagicWeapon : NetworkBehaviour
{
    public PlayerStateManager stateUI;
    public Transform bulletSpawn; 
    public GameObject bulletPrefab; 
    public float bulletSpeed = 300f; 
    public float spreadIntensity;//Spread
    private NetworkVariable<float> Damage = new NetworkVariable<float>(25f, default, NetworkVariableWritePermission.Owner);
    private NetworkVariable<float> MagicRequiredPerFire = new NetworkVariable<float>(7f, default, NetworkVariableWritePermission.Owner);
    public float damage
    {
        get { return Damage.Value; }
        set
        {
            Damage.Value = value;
        }
    }
    public float magicRequiredPerFire
    {
        get { return MagicRequiredPerFire.Value; }
        set
        {
            MagicRequiredPerFire.Value = value;
        }
    }

    public bool readyToShoot = true;

    public float shootingDelay = 10f;
    // Update is called once per frame
    public PlayerStateManager stateManager;
    public Vector3 direction;
    public MusicController musicController;

    //[ServerRpc]
    //private void ShootServerRpc(Vector3 dir)
    //{
    //    ShootClientRpc(dir);
    //    return;
    //}
    //[ClientRpc]
    //private void ShootClientRpc(Vector3 dir)
    //{
    //    Debug.Log("diaoyongle");
    //    GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, bulletSpawn.rotation);
    //    Rigidbody rb = bullet.GetComponent<Rigidbody>();
    //    float x = UnityEngine.Random.Range(-spreadIntensity / 100, spreadIntensity / 100);
    //    //float y = UnityEngine.Random.Range(-spreadIntensity / 100, spreadIntensity / 100);
    //    rb.velocity = (dir + new Vector3(x, 0, 0)) * bulletSpeed;
    //}
    [ServerRpc]
    private void ShootServerRpc(Vector3 pos,Quaternion rot,Vector3 dir)
    {
        GameObject bullet = Instantiate(bulletPrefab, pos,rot);
        bullet.GetComponent<NetworkObject>().Spawn();
        bullet.GetComponent<Bullet>().SetSourceID((int)OwnerClientId);
        bullet.GetComponent<Bullet>().SetSourcePlayerID(stateUI.playerId);
        if (!IsServer) Debug.Log("RpcCHUWENTRILE"); 
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        float x = UnityEngine.Random.Range(-spreadIntensity / 100, spreadIntensity / 100);
        float y = UnityEngine.Random.Range(-spreadIntensity / 100, spreadIntensity / 100);
        rb.velocity = (dir + new Vector3(x, 0, 0)) * bulletSpeed;
        bullet.GetComponent<Bullet>().SetDamage(damage);
       
    }
    public bool Shoot()
    {
        if (stateManager.currentMana > magicRequiredPerFire && readyToShoot)
        {
            stateManager.currentMana -= magicRequiredPerFire;
            ShootServerRpc(bulletSpawn.position,bulletSpawn.rotation,direction);

            musicController.WizardAttackMusic();
            readyToShoot = false;
            Invoke("ResetShoot", shootingDelay);
            return true;
        }
        else { return false; }
    }

    private void ResetShoot()
    {
        readyToShoot = true;
    }
    public bool WizardBallGenerate()
    {
        if (stateManager.currentMana > 35f && readyToShoot)
        {
            stateManager.currentMana -= 35f;
            WizardBallGenerateServerRpc(bulletSpawn.position,Quaternion.identity, direction);

            musicController.WizardAttackMusic();
            readyToShoot = false;
            Invoke("ResetShoot", shootingDelay);
            return true;
        }
        else { return false; }
    }

    public GameObject wizardBall;
    [ServerRpc]
    void WizardBallGenerateServerRpc(Vector3 pos, Quaternion rot, Vector3 dir)
    {
        GameObject bullet = Instantiate(wizardBall, pos, rot);
        bullet.GetComponent<NetworkObject>().Spawn();
        bullet.GetComponent<WizardBall>().SetSourceID((int)OwnerClientId);
        bullet.GetComponent<WizardBall>().SetSourcePlayerID(stateUI.playerId);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        rb.velocity = dir * 25f;

    }
}