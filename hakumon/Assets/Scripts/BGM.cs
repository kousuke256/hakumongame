using UnityEngine;

public class BGM : MonoBehaviour
{
    //コンポーネント忘れない
    private AudioSource audioSource;
    public AudioClip Sample;
    //startのなかに
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    
        audioSource.PlayOneShot(Sample);
    }
    
}
