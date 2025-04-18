using UnityEngine;
using System.Collections;

public class TrapDamage : MonoBehaviour
{
    [Header("Damage Settings")]
    [Range(0, 5000)] [SerializeField] private int damageAmount;
    [Range(0, 10)] [SerializeField] private float damageInterval;
    [SerializeField] private LayerMask targetLayers;

    [Header("Effects")]
    [SerializeField] private ParticleSystem hitEffect;
    [SerializeField] private AudioClip hitSound;

    private bool isDamaging;
    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        enabled = false;
    }

    private void OnTriggerStay(Collider other)
    {
        if (!enabled || isDamaging) return;

        if (IsValidTarget(other))
        {
            StartCoroutine(DamageOverTime(other.GetComponent<IDamage>()));
        }
    }

    private bool IsValidTarget(Collider other)
    {
        // Check if collider is on target layers
        return ((1 << other.gameObject.layer) & targetLayers) != 0;
    }

    IEnumerator DamageOverTime(IDamage target)
    {
        if (target == null) yield break;

        isDamaging = true;
        target.takeDamage(damageAmount);
        PlayHitEffects();
        yield return new WaitForSeconds(damageInterval);
        isDamaging = false;
    }

    private void TryDamage(IDamage target)
    {
        if (target != null)
        {
            target.takeDamage(damageAmount);
            PlayHitEffects();
        }
    }

    private void PlayHitEffects()
    {
        if (hitEffect != null)
            hitEffect.Play();

        if (hitSound != null && audioSource != null)
            audioSource.PlayOneShot(hitSound);
    }

    public void SetActive(bool active)
    {
        enabled = active;
        GetComponent<Collider>().enabled = active;
    }
}