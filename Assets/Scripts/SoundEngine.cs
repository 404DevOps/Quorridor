using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEngine : MonoBehaviour
{
    public static SoundEngine Instance;

    public AudioClip Bleep;
    public AudioClip Placed;
    public AudioClip Hoover;
    public AudioClip Clicked;
    public AudioClip Rotate;

    private AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else 
        {
            Destroy(this.gameObject);
        }
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayBleep()
    {
        audioSource.PlayOneShot(Bleep);
    }
    public void PlayHover()
    {
        audioSource.PlayOneShot(Hoover);
    }
    public void PlayPlaced()
    {
        audioSource.PlayOneShot(Placed);
    }
    public void PlayClicked()
    {
        audioSource.PlayOneShot(Clicked);
    }
    public void PlayRotate()
    {
        audioSource.PlayOneShot(Rotate);
    }
}
