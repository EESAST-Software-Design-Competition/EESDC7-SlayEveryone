using UnityEngine;

public class BulletController : MonoBehaviour
{
    public GameObject bulletPrefab; 
    public Transform bulletSpawnPoint; 
    public float bulletSpeed = 10f; 
    public GameObject bulletVFXPrefab; 

    void Update()
    {
        // ¼ì²âÊó±ê×ó¼üµã»÷
        if (Input.GetMouseButtonDown(0))
        {
            ShootBullet();
        }
    }

    void ShootBullet()
    {
        
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, Quaternion.identity);

        Vector3 shootDirection = transform.forward;

        Rigidbody bulletRigidbody = bullet.GetComponent<Rigidbody>();
        bulletRigidbody.velocity = shootDirection * bulletSpeed;

        if (bulletVFXPrefab != null)
        {
            
            GameObject bulletVFX = Instantiate(bulletVFXPrefab, bulletSpawnPoint.position, Quaternion.identity);
            
            bulletVFX.transform.parent = bullet.transform;
            
            Destroy(bulletVFX, 2f); 
        }
    }
}