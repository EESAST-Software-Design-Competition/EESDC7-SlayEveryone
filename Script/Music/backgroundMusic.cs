using UnityEngine;

public class BackgroundMusicController : MonoBehaviour
{
    public AudioClip backgroundMusic;

    private AudioSource audioSource; 

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        audioSource.loop = true;

        if (backgroundMusic != null)
        {
            audioSource.clip = backgroundMusic;
            audioSource.Play();
        }
    }
}