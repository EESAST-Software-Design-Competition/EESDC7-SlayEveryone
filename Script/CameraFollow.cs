using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class CameraFollow : MonoBehaviour
{
    public Transform targetPlayer; // 角色的 Transform 组件
    public bool shake= false;
    private float totalshaketime=0.35f;
    private float amplitude=0.14f;
    private float co=0f;
    void Update()
    {
        // if(!IsOwner) return;
        if(shake==true)
        {
            shake=false;
            co=1f;
            StartCoroutine(ShakingCamera());
        }
        if (targetPlayer != null)
        {
            float dx=Random.Range(-co*amplitude,co*amplitude);
            float dz=Random.Range(-co*amplitude,co*amplitude);
            // 获取目标角色位置
            Vector3 targetPos = targetPlayer.position;
            // 设置摄像机位置为目标位置，并保持相对位置
            transform.position = new Vector3(targetPos.x+dx, targetPos.y+16f, targetPos.z-1.8f+dz);
        }
    }
    IEnumerator ShakingCamera()
    {
        yield return new WaitForSeconds(totalshaketime);
        co=0;
    }
}
