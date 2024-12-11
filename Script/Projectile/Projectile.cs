using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]float moveSpeed=10f;
    [SerializeField]Vector3 moveDirection;

    void OnEnable()
    {
        StartCoroutine(Movedirectly());
    }
    IEnumerator Movedirectly()
    {
        while(gameObject.activeSelf)
        {
            transform.Translate(moveDirection*moveSpeed*Time.deltaTime);

            yield return null;
        }
    }
}
