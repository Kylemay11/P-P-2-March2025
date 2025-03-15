using UnityEngine;

public class zombieBile : MonoBehaviour
{
    [SerializeField] private float destroyDelay;
    [SerializeField] Damage damageScript;

    private Vector3 startPos, targetPos;
    private float timeOfFlight;
    private float launchTime;
    private float arcHeight;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        damageScript = GetComponent<Damage>();
    }

    // Update is called once per frame
    void Update()
    {
        float elapsedTime = Time.time - launchTime;
        float normalizedTime = elapsedTime / timeOfFlight;

        if (normalizedTime > 1f)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 currentPos = Vector3.Lerp(startPos, targetPos, normalizedTime);

        // Add arc to the trajectory
        currentPos.y += arcHeight * Mathf.Sin(normalizedTime * Mathf.PI);

        transform.position = currentPos;
    }

    public void Launch(Vector3 target, float speed, float height)
    {
        startPos = transform.position;
        targetPos = target;
        arcHeight = height;

        float distance = Vector3.Distance(startPos, targetPos);
        timeOfFlight = distance / speed;
        launchTime = Time.time;

        if (damageScript != null)
        {
            // Set the speed in the Damage script
            damageScript.GetComponent<Rigidbody>().linearVelocity = transform.forward * speed;
        }
    }
}
