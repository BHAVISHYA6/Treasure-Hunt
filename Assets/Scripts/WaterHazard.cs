using UnityEngine;

public class WaterHazard : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player == null)
        {
            player = other.GetComponentInParent<PlayerController>();
        }
        if (player != null)
        {
            player.OnFallIntoWater(transform.position);
        }
    }
}

