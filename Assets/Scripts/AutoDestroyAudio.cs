using UnityEngine;

public class AutoDestroyAudio : MonoBehaviour
{
    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        Destroy(gameObject, audioSource.clip.length);
    }
}
