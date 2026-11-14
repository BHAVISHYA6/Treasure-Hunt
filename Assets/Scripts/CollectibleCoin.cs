using UnityEngine;

public class CollectibleCoin : MonoBehaviour
{
    public enum CoinType { StandardCoin, RareGem, MapFragment }

    [Header("Collectible Properties")]
    public CoinType type = CoinType.StandardCoin;
    public int coinValue = 1;

    [Header("Motion Settings")]
    public float rotationSpeed = 100f;
    public float bobFrequency = 2.5f;
    public float bobAmplitude = 0.12f;
    public float magnetDistance = 3.0f;
    public float magnetSpeed = 8.0f;

    private Vector3 basePosition;
    private bool isCollected = false;
    private Transform playerTransform;

    private void Start()
    {
        basePosition = transform.position;
        var player = FindObjectOfType<PlayerController>();
        if (player != null) playerTransform = player.transform;
    }

    private void Update()
    {
        if (isCollected) return;

        // Rotation
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.World);

        // Bobbing
        float newY = basePosition.y + Mathf.Sin(Time.time * bobFrequency) * bobAmplitude;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);

        // Magnetic attraction to player when close
        if (playerTransform != null)
        {
            float dist = Vector3.Distance(transform.position, playerTransform.position + Vector3.up * 0.8f);
            if (dist < magnetDistance)
            {
                transform.position = Vector3.MoveTowards(transform.position, playerTransform.position + Vector3.up * 0.8f, magnetSpeed * Time.deltaTime);
                if (dist < 0.6f)
                {
                    Collect();
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isCollected) return;
        if (other.CompareTag("Player") || other.GetComponent<PlayerController>() != null)
        {
            Collect();
        }
    }

    private void Collect()
    {
        if (isCollected) return;
        isCollected = true;

        if (GameManager.Instance != null)
        {
            if (type == CoinType.MapFragment)
            {
                GameManager.Instance.AddMapPiece();
            }
            else
            {
                GameManager.Instance.AddCoins(coinValue);
            }
        }

        // Spawn visual burst particles if needed
        CreatePickupFlash();
        Destroy(gameObject);
    }

    private void CreatePickupFlash()
    {
        // Simple procedural sparkle or light flash
        GameObject flash = new GameObject("PickupFlash");
        flash.transform.position = transform.position;
        var light = flash.AddComponent<Light>();
        light.type = LightType.Point;
        light.color = type == CoinType.MapFragment ? Color.cyan : (type == CoinType.RareGem ? Color.magenta : Color.yellow);
        light.range = 3.5f;
        light.intensity = 2.5f;
        Destroy(flash, 0.25f);
    }
}
