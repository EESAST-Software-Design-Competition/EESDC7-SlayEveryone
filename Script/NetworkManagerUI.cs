using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class NetworkManagerUI : MonoBehaviour
{
    public ChangeScene sceneManager;
    [SerializeField] private Button hostBtn;
    [SerializeField] private Button clientBtn;
    private void Awake()
    {
        hostBtn.onClick.AddListener(()=>{
            sceneManager.SetID("Host");
            SceneManager.LoadScene("GameScene");
        });
        clientBtn.onClick.AddListener(()=>{
            sceneManager.SetID("Client");
            SceneManager.LoadScene("GameScene");
        });
    }
}
