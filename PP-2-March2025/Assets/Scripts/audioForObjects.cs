using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class audioForObjects : MonoBehaviour
{
    public AudioSource source;
    public AudioClip clip;
    public AudioClip gunShot;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            source.PlayOneShot(clip);
        }
    }
}
