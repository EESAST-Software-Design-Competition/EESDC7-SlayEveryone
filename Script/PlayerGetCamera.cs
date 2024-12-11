using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class PlayerGetCamera : NetworkBehaviour
{
    CameraFollow mainCameraFollow;
    private void Awake()
    {
        Invoke("nothing",0.15f);
    }
    private void nothing()
    {
        // if(!GetComponent<PlayerMovement>().CustomIsOwner) return;
        if(!IsOwner) return;
        mainCameraFollow=GameObject.Find("Main Camera").GetComponent<CameraFollow>();
        mainCameraFollow.targetPlayer=transform;
    }
}
