using UnityEngine;
[RequireComponent(typeof(AudioSource))]
public class EndrollBGM : MonoBehaviour
{
    [Range(0f, 1f)]
    public float volume = 0.4f;
    private void Start()
    {
        AudioSource source = GetComponent<AudioSource>();
        source.playOnAwake = false;
        source.spatialBlend = 0f;
        source.loop = true;
        source.volume = volume;
        source.clip = Resources.Load<AudioClip>("Space_EndCredits");
        if (source.clip == null)
        {
            Debug.LogWarning(
                "ResourcesにSpace_EndCredits.wavを入れてください！");
            return;
        }
        source.Play();
    }
}