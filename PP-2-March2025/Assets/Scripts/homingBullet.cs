using UnityEngine;
using System;
using System.Collections;


public class homingBullet : MonoBehaviour
{

    [Range(1,1000)][SerializeField] float rotSpeed;
    [Range(1, 5)][SerializeField] float speed;
    
    public Transform target;
    // Update is called once per frame
    void Update()
    { 
        if (target == null)
        {
            Debug.Log("No target found");
            return;
        }

        Vector3 dir = (target.position - transform.position).normalized;

        Quaternion lookRot = Quaternion.LookRotation(dir);

        transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime);

        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }
}

