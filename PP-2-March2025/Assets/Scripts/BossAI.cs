using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class BossAI : MonoBehaviour, IDamage, IZombie
{
    //public event System.Action OnZombieDeath;
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Animator anim;

    [SerializeField] int HP;
    [SerializeField] int faceTargetSpeed;
    [SerializeField] int animTranSpeed;

    [SerializeField] Transform shootPos1;
    [SerializeField] Transform shootPos2;
    [SerializeField] Transform shootPos3;
    [SerializeField] Transform shootPos4;
    [SerializeField] Transform shootPos5;
    [SerializeField] Transform shootPos6;


    [SerializeField] GameObject bullet;
    [SerializeField] float shootRate;

    float shootTimer;
    bool playerInRange;

    Vector3 playerDir;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        shootTimer += Time.deltaTime;
        
       
        playerDir = gameManager.instance.player.transform.position - transform.position;
        agent.SetDestination(gameManager.instance.player.transform.position);
        
        if (shootTimer >= shootRate)
        {
            shoot();
        }
        
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            faceTarget();
        }
        
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    void faceTarget()
    {
        Quaternion rot = Quaternion.LookRotation(new Vector3(playerDir.x,playerDir.y, playerDir.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * faceTargetSpeed);
    }

    public void takeDamage(int amount)
    {
        HP -= amount;
        StartCoroutine(flashRed());

        agent.SetDestination(gameManager.instance.player.transform.position);
        
        if (HP <= 0)
        {
            Destroy(gameObject);
        }
    }
    IEnumerator flashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = Color.white;
    }

    Vector3 getDownwardPlayerDir()
    {
        Vector3 directionToPlayer = gameManager.instance.player.transform.position 
            - transform.position;

        //force the y component to be negative
        directionToPlayer.y = -Mathf.Abs(directionToPlayer.y) - 2; // The -2 brings it down enough to hit player

        return directionToPlayer.normalized;
    }

    Vector3 getDownwardDir()
    {
        return Vector3.down;
    }

    void Phase1()
    {
        shootTimer = 0;

        Vector3 shootDirection = getDownwardPlayerDir();
        Quaternion shootRotation = Quaternion.LookRotation(shootDirection);

        Instantiate(bullet, shootPos1.position, shootRotation);
        Instantiate(bullet, shootPos2.position, shootRotation);
        Instantiate(bullet, shootPos3.position, shootRotation);
    }

    void Phase2()
    {
        shootTimer = 0;

        Vector3 shootDirection = getDownwardDir();
        Quaternion shootRotation = Quaternion.LookRotation(shootDirection);

        Instantiate(bullet, shootPos1.position, shootRotation);
        Instantiate(bullet, shootPos2.position, shootRotation);
        Instantiate(bullet, shootPos3.position, shootRotation); 
        Instantiate(bullet, shootPos4.position, shootRotation);
        Instantiate(bullet, shootPos5.position, shootRotation);
        Instantiate(bullet, shootPos6.position, shootRotation);
        
    }

    void shoot()
    {
       if(HP >= HP % 2)
        {
            Phase1();
        }
        else
        {
            Phase2();
        }
    }

    public void InitializeZombie(float health, float speed, float damage)
    {
        HP = Mathf.RoundToInt(health);
        agent.speed = speed;
        Debug.Log($"[InitBoss] Health: {health}, Speed: {speed}, Damage: {damage}");
    }
}
