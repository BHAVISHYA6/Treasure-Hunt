using UnityEngine;

public class KeyPickup : MonoBehaviour, IInteractable
{
    public enum KeyType { Bronze, Silver, Gold }

    [Header("Key Configuration")]
    public KeyType keyType = KeyType.Bronze;
    public string keyName = "Bronze Skeleton Key";
    public bool requiresAllMapPieces = false;
    public bool hiddenUntilReady = false;

    [Header("Animation")]
    public float rotationSpeed = 60f;
    public float bobSpeed = 2f;
    public float bobDistance = 0.15f;

    private Vector3 startPos;
    private bool isCollected = false;

    private void Start()
    {
        startPos = transform.position;
    }

    private void Update()
    {
        if (isCollected) return;

        // Check visibility condition
        if (requiresAllMapPieces && hiddenUntilReady)
        {
            bool ready = GameManager.Instance != null && GameManager.Instance.mapPieces >= GameManager.TotalMapPieces;
            foreach (var r in GetComponentsInChildren<Renderer>())
            {
                r.enabled = ready;
            }
            var col = GetComponent<Collider>();
            if (col != null) col.enabled = ready;

            if (!ready) return;
        }

        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.World);
        float newY = startPos.y + Mathf.Sin(Time.time * bobSpeed) * bobDistance;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isCollected) return;
        var player = other.GetComponent<PlayerController>();
        if (player != null && CanInteract(player))
        {
            Interact(player);
        }
    }

    public string GetInteractionPrompt() => $"Press [E] to Take {keyName}";

    public bool CanInteract(PlayerController player)
    {
        if (isCollected) return false;
        if (requiresAllMapPieces && GameManager.Instance != null && GameManager.Instance.mapPieces < GameManager.TotalMapPieces)
            return false;
        return true;
    }

    public void Interact(PlayerController player)
    {
        if (isCollected) return;
        isCollected = true;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.CollectKey(keyType.ToString());
        }

        // Spawn visual flash
        GameObject flash = new GameObject("KeyPickupLight");
        flash.transform.position = transform.position;
        var light = flash.AddComponent<Light>();
        light.type = LightType.Point;
        light.color = keyType == KeyType.Gold ? Color.yellow : (keyType == KeyType.Silver ? Color.white : new Color(0.8f, 0.5f, 0.2f));
        light.range = 5f;
        light.intensity = 3f;
        Destroy(flash, 0.4f);

        Destroy(gameObject);
    }
}
