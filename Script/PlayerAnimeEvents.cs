using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimeEvents : MonoBehaviour
{
    // Start is called before the first frame update

    private PlayerMovement player;
    void Start()
    {
        player = GetComponentInParent<PlayerMovement>();
        
        
    }

    private void AnimationTrigger(){
        player.AttackOver();
    }
}
