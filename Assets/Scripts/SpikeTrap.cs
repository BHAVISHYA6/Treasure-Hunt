using System.Collections;
using UnityEngine;

public class SpikeTrap : MonoBehaviour
{
    [Header("Spike Movement")]
    public Transform spikeGroup;
    public float hiddenY = -0.45f;
    public float extendedY = 0.5f;

    [Header("Timing")]
    public float waitDuration = 2.0f;
    public float warningDuration = 0.6f;
    public float extendedDuration = 1.0f;
    public float startDelay = 0.0f;

    [Header("Visual Warning")]
    public Light warningLight;
    public BoxCollider solidSpikeCollider;

    private bool isActive = false;

    private void Start()
    {
        if (solidSpikeCollider != null)
            solidSpikeCollider.enabled = false;

        if (spikeGroup != null)
        {
            Vector3 pos = spikeGroup.localPosition;
            pos.y = hiddenY;
            spikeGroup.localPosition = pos;
        }

        if (warningLight != null)
            warningLight.enabled = false;

        StartCoroutine(TrapCycleRoutine());
    }

    private IEnumerator TrapCycleRoutine()
    {
        if (startDelay > 0f)
            yield return new WaitForSeconds(startDelay);

        while (true)
        {
            // 1. Hidden idle
            isActive = false;
            if (solidSpikeCollider != null) solidSpikeCollider.enabled = false;
            if (warningLight != null) warningLight.enabled = false;
            SetSpikeHeight(hiddenY);
            yield return new WaitForSeconds(waitDuration);

            // 2. Warning phase
            if (warningLight != null) warningLight.enabled = true;
            SetSpikeHeight(Mathf.Lerp(hiddenY, extendedY, 0.2f));
            yield return new WaitForSeconds(warningDuration);

            // 3. Extend spikes rapidly - now a deadly solid obstacle!
            isActive = true;
            if (solidSpikeCollider != null) solidSpikeCollider.enabled = true;
            SoundManager.Instance?.PlayTrapSound();
            float t = 0f;
            while (t < 0.1f)
            {
                t += Time.deltaTime;
                SetSpikeHeight(Mathf.Lerp(hiddenY, extendedY, t / 0.1f));
                yield return null;
            }
            SetSpikeHeight(extendedY);

            yield return new WaitForSeconds(extendedDuration);

            // 4. Retract smoothly
            t = 0f;
            while (t < 0.25f)
            {
                t += Time.deltaTime;
                SetSpikeHeight(Mathf.Lerp(extendedY, hiddenY, t / 0.25f));
                yield return null;
            }
            SetSpikeHeight(hiddenY);
            isActive = false;
            if (solidSpikeCollider != null) solidSpikeCollider.enabled = false;
        }
    }

    private void SetSpikeHeight(float y)
    {
        if (spikeGroup == null) return;
        Vector3 pos = spikeGroup.localPosition;
        pos.y = y;
        spikeGroup.localPosition = pos;
    }

    private void OnTriggerEnter(Collider other)
    {
        CheckDamage(other);
    }

    private void OnTriggerStay(Collider other)
    {
        CheckDamage(other);
    }

    private void CheckDamage(Collider other)
    {
        if (!isActive) return;
        var player = other.GetComponent<PlayerController>();
        if (player == null) player = other.GetComponentInParent<PlayerController>();
        if (player != null)
        {
            player.TakeDamage(1, transform.position);
        }
    }
}
