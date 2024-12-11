using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class IntoGame : MonoBehaviour
{
    private Button cur;
    private void Awake()
    {
        cur=GetComponent<Button>();
        cur.onClick.AddListener(()=>{
            Destroy(GameObject.Find("Canvas"));
            GameObject.Find("NetUI").GetComponent<Canvas>().enabled=true;
        });
    }
}
