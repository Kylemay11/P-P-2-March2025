using UnityEngine;
using UnityEngine.Audio;

public class Settings : MonoBehaviour
{
    public AudioMixer audiomix;

    public void setAudio(float vol)
    {
        audiomix.SetFloat("volume", vol);
    }

    public void setQuality(int qual)
    {

    }
}
