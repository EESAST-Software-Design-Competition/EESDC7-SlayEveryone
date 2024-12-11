using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDeactivate : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] bool destroyGameObject;
    [SerializeField] float lifetiem = 3f;
    WaitForSeconds waitlifetime;
    void Awake()
    {
        waitlifetime = new WaitForSeconds(lifetiem);
    }
    void OnEnable()
    {
        StartCoroutine(DeactivateCoroutine());
    }
    IEnumerator DeactivateCoroutine()
    {
        yield return waitlifetime;
        if(destroyGameObject)
        {
            Destroy(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
