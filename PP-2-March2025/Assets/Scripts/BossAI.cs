using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class BossAI : MonoBehaviour
{
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Animator anim;

    [SerializeField] int HP;
    [SerializeField] int faceTargetSpeed;
    [SerializeField] int animTranSpeed;

    [SerializeField] Transform shootPos1;
    [SerializeField] Transform shootPos2;
    [SerializeField] Transform shootPos3;

    [SerializeField] GameObject bullet;
    [SerializeField] float shootRate;

    float shootTimer;

    Vector3 playerDir;

    bool playerInRange;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameManager.instance.updateGameGoal(1);
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
        Quaternion rot = Quaternion.LookRotation(new Vector3(playerDir.x, playerDir.y, playerDir.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * faceTargetSpeed);
    }

    public void takeDamage(int amount)
    {
        HP -= amount;
        StartCoroutine(flashRed());

        agent.SetDestination(gameManager.instance.player.transform.position);
        
        if (HP <= 0)
        {
            gameManager.instance.updateGameGoal(-1);
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
        directionToPlayer.y = -Mathf.Abs(directionToPlayer.y);

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
}
