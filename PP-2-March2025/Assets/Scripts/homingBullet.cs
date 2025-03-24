using UnityEngine;
using System;
using System.Collections;


public class homingBullet : MonoBehaviour
{
    [SerializeField] float homingSpeed;
    [SerializeField] float velocity;
    public float angleBetween;
    public Transform target;
    // Update is called once per frame
    void Update()
    {
        // StartCoroutine(bullet());
        // velocity = transform.forward.z;
        target = gameManager.instance.player.transform;
        float step = velocity * Time.deltaTime;

        // Vector3 playDir = gameManager.instance.player.transform.position;
        //Quaternion rot = Quaternion.LookRotation(new Vector3(playDir.x, transform.position.y,playDir.z));

        // transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * homingSpeed);

        transform.rotation = Quaternion.RotateTowards(transform.rotation,target.rotation , step);

        // transform.position += transform.forward * velocity * Time.deltaTime;
        
    }

    IEnumerator bullet()
    {
        
        
        yield return new WaitForSeconds(0.1f);
    }
}
