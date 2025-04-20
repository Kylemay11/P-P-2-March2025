using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class audioForObjects : MonoBehaviour
{
    private float nextPlayTime = 0f;
    private float minTimeBetweenClips = 3f; // Minimum time in seconds between audio clip playbacks
    private float maxTimeBetweenClips = 10f; // Maximum time in seconds between audio clip playbacks

    [Header("--- Audio ---")]
    [SerializeField] AudioSource aud;
    [SerializeField] AudioClip[] audClips;
    [Range(0, 1)][SerializeField] float audClipsVol;
    // Update is called once per frame
    void Update()
    {

    if (Time.time >= nextPlayTime)
        {
            // Play a random audio clip from the audClips array at the specified volume
            aud.PlayOneShot(audClips[Random.Range(0, audClips.Length)], audClipsVol);

            // Calculate the next time to play an audio clip
            nextPlayTime = Time.time + Random.Range(minTimeBetweenClips, maxTimeBetweenClips);
        }
    }
}
