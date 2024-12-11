using UnityEngine;

public class MusicController : MonoBehaviour
{
    public AudioClip attackMusic; // ��������
    public AudioClip moveMusic; // �ƶ�����
    public AudioClip getAttackMusic; // �ܵ���������
    public AudioClip getObjectMusic; // �����Ʒ����

    public AudioClip autoModeAttack;

    private AudioSource audioSource; 

    void Start()
    {
        Invoke("RealStart", 0.05f);
    }
    void RealStart()
    {
        audioSource = GetComponent<AudioSource>();
    }
    public void PlayAttackMusic()
    {
        PlayMusic(attackMusic);
    }
    public void PlayerAutoModeAttackMusic()
    {
        audioSource.clip = autoModeAttack;
        audioSource.time = 0.1f; // ���ò��ŵĿ�ʼʱ��
        audioSource.Play();
    }
    public void WizardAttackMusic(){
        audioSource.clip=attackMusic;
        audioSource.time=0f;
        audioSource.Play();
        Invoke("StopClip",0.5f);
    }

    public void PlayMoveMusic()
    {
        PlayMusic(moveMusic);
    }

    public void PlayGetAttackMusic()
    {
        audioSource.clip = getAttackMusic;
        if (audioSource.clip != null)
        {
            audioSource.time = 1.1f; // ���ò��ŵĿ�ʼʱ��
            audioSource.pitch = 1.5f;
            audioSource.Play();
            Invoke("StopClip", 0.6f); // ��clipLength���ֹͣ����
        }
    }

    public void PlayGetObjectMusic()
    {
        PlayMusic(getObjectMusic);
    }

    private void PlayMusic(AudioClip music)
    {
        if (music != null)
        {
            audioSource.clip = music;
            audioSource.Play();
        }
    }
    
    private void StopClip()
    {
        audioSource.Stop();
        audioSource.pitch = 1;
    }
}