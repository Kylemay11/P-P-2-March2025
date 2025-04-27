using UnityEngine;
using UnityEngine.AI;

public class animatorScript : MonoBehaviour
{
    Animator animator;
    NavMeshAgent agent;
    enemyAI enemy;

    private const string IsWalking = "IsWalking";
    private const string Speed = "Speed";
    void start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        enemy = GetComponent<enemyAI>();
    }

    void Update()
    {
        Vector3 velocity = agent.velocity;
        float speed = velocity.magnitude;

        animator.SetFloat(Speed, speed);

        animator.SetBool(IsWalking, speed > 0.1f);
    }


    public void TriggerDeathAnimation()
    {
            animator.SetTrigger("Die");

            agent.isStopped = true;
            agent.enabled = false;
    }
}
