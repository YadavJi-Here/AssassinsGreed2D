using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource source;

    public void PlayAudio()
    {
        source.Play();
    }
}
