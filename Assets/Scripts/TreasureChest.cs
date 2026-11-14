using System.Collections;
using UnityEngine;

public class TreasureChest : MonoBehaviour, IInteractable
{
    [Header("Chest Configuration")]
    public string chestName = "Ancient Treasure Chest";
    public bool requiresKey = false;
    public KeyPickup.KeyType requiredKeyType = KeyPickup.KeyType.Bronze;
    public int coinsReward = 15;
    public bool isGrandVaultChest = false;

    [Header("Lid Transform")]
    public Transform chestLid;
    public Vector3 openRotation = new Vector3(-95f, 0f, 0f);
    public float openSpeed = 2.5f;

    [Header("Visual Effects")]
    public Light interiorGlow;
    public ParticleSystem treasureParticles;

    private bool isOpen = false;
    private bool isOpening = false;

    private void Start()
    {
        if (interiorGlow != null)
            interiorGlow.intensity = 0f;
    }

    public string GetInteractionPrompt()
    {
        if (isOpen) return "";
        if (requiresKey)
        {
            bool hasKey = HasRequiredKey();
            if (!hasKey)
                return $"Locked! (Requires {requiredKeyType} Key)";
            return $"Press [E] to Unlock {chestName} with {requiredKeyType} Key";
        }
        return $"Press [E] to Open {chestName}";
    }

    public bool CanInteract(PlayerController player) => !isOpen && !isOpening;

    public void Interact(PlayerController player)
    {
        if (isOpen || isOpening) return;

        if (requiresKey && !HasRequiredKey())
        {
            SoundManager.Instance?.PlayTrapSound();
            GameManager.Instance?.Notify($"Locked! You need the {requiredKeyType} Key to open this!");
            return;
        }

        StartCoroutine(OpenChestRoutine(player));
    }

    private bool HasRequiredKey()
    {
        if (GameManager.Instance == null) return true;
        switch (requiredKeyType)
        {
            case KeyPickup.KeyType.Bronze: return GameManager.Instance.hasBronzeKey;
            case KeyPickup.KeyType.Silver: return GameManager.Instance.hasSilverKey;
            case KeyPickup.KeyType.Gold: return GameManager.Instance.hasGoldKey;
            default: return true;
        }
    }

    private IEnumerator OpenChestRoutine(PlayerController player)
    {
        isOpening = true;
        SoundManager.Instance?.PlayChestSound();

        // Animate lid opening smoothly
        if (chestLid != null)
        {
            Quaternion startRot = chestLid.localRotation;
            Quaternion targetRot = Quaternion.Euler(openRotation);
            float elapsed = 0f;
            float duration = 0.8f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);
                chestLid.localRotation = Quaternion.Slerp(startRot, targetRot, t);

                if (interiorGlow != null)
                {
                    interiorGlow.intensity = Mathf.Lerp(0f, 3.5f, t);
                }
                yield return null;
            }
            chestLid.localRotation = targetRot;
        }

        isOpen = true;
        isOpening = false;

        // Play particle effects
        if (treasureParticles != null)
        {
            treasureParticles.Play();
        }

        // Award reward
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddCoins(coinsReward);
            GameManager.Instance.Notify($"Chest opened! Found {coinsReward} Gold Coins!");
        }

        // Grand Vault final chest trigger
        if (isGrandVaultChest)
        {
            if (player != null)
            {
                player.TriggerVictoryAnimation();
            }
            if (GameManager.Instance != null)
            {
                GameManager.Instance.FinishGame();
            }
        }
    }
}
