using UnityEngine;

public class ScenePortal : MonoBehaviour, IInteractable
{
    [Header("Destination")]
    public string targetSceneName = "Scene2_AncientTemple";
    public string portalName = "Ancient Sea Gate";

    [Header("Requirements")]
    public bool requiresKey = true;
    public KeyPickup.KeyType requiredKey = KeyPickup.KeyType.Bronze;

    [Header("Visual Effects")]
    public Transform vortexVisual;
    public Light portalGlow;
    public ParticleSystem portalParticles;

    private bool isUnlocked = false;

    private void Start()
    {
        CheckUnlockStatus();
    }

    private void Update()
    {
        CheckUnlockStatus();

        if (vortexVisual != null && isUnlocked)
        {
            vortexVisual.Rotate(Vector3.forward * 90f * Time.deltaTime, Space.Self);
        }
    }

    private void CheckUnlockStatus()
    {
        if (isUnlocked) return;

        bool ready = true;
        if (requiresKey && GameManager.Instance != null)
        {
            switch (requiredKey)
            {
                case KeyPickup.KeyType.Bronze: ready = GameManager.Instance.hasBronzeKey; break;
                case KeyPickup.KeyType.Silver: ready = GameManager.Instance.hasSilverKey; break;
                case KeyPickup.KeyType.Gold: ready = GameManager.Instance.hasGoldKey; break;
            }
        }

        if (ready)
        {
            isUnlocked = true;
            if (portalGlow != null) portalGlow.enabled = true;
            if (portalParticles != null && !portalParticles.isPlaying) portalParticles.Play();
        }
        else
        {
            if (portalGlow != null) portalGlow.enabled = false;
            if (portalParticles != null && portalParticles.isPlaying) portalParticles.Stop();
        }
    }

    public string GetInteractionPrompt()
    {
        if (!isUnlocked)
        {
            return $"{portalName} is sealed! (Requires {requiredKey} Key)";
        }
        return $"Press [E] to Step through {portalName}";
    }

    public bool CanInteract(PlayerController player) => isUnlocked;

    public void Interact(PlayerController player)
    {
        if (!isUnlocked)
        {
            SoundManager.Instance?.PlayTrapSound();
            GameManager.Instance?.Notify($"Portal sealed! Find the {requiredKey} Key first.");
            return;
        }

        SoundManager.Instance?.PlayPortalSound();
        GameManager.Instance?.Notify($"Traveling through {portalName}...");

        if (string.IsNullOrEmpty(targetSceneName))
        {
            GameManager.Instance?.LoadNextLevel();
        }
        else
        {
            GameManager.Instance?.LoadLevel(targetSceneName);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        var player = other.GetComponent<PlayerController>();
        if (player != null && isUnlocked)
        {
            Interact(player);
        }
    }
}
