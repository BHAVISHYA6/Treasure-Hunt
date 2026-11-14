using UnityEngine;

public class ClueScroll : MonoBehaviour, IInteractable
{
    [Header("Clue Details")]
    public string clueTitle = "Captain's Logbook";
    [TextArea(4, 10)]
    public string clueContent = "Three torn fragments of the secret map lie hidden across this island. Find them, and the ancient Sea Shrine will yield the Bronze Skeleton Key!";
    public string promptAction = "Read";

    [Header("Quest Progression")]
    public string updateObjectiveOnRead = "";
    public int bonusScore = 200;
    private bool hasBeenRead = false;

    [Header("Visual Effects")]
    public Transform rotatingVisual;
    public float floatSpeed = 2f;
    public float floatHeight = 0.15f;
    private Vector3 initialPos;

    private void Start()
    {
        initialPos = transform.position;
    }

    private void Update()
    {
        if (rotatingVisual != null)
        {
            rotatingVisual.Rotate(Vector3.up * 45f * Time.deltaTime, Space.World);
            float newY = initialPos.y + Mathf.Sin(Time.time * floatSpeed) * floatHeight;
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        }
    }

    public string GetInteractionPrompt() => $"Press [E] to {promptAction} {clueTitle}";

    public bool CanInteract(PlayerController player) => true;

    public void Interact(PlayerController player)
    {
        SoundManager.Instance?.PlayInteractSound();
        if (GameHUD.Instance != null)
        {
            GameHUD.Instance.ShowClueModal(clueTitle, clueContent);
        }

        if (!hasBeenRead)
        {
            hasBeenRead = true;
            if (GameManager.Instance != null)
            {
                if (!string.IsNullOrEmpty(updateObjectiveOnRead))
                {
                    GameManager.Instance.SetObjective(updateObjectiveOnRead);
                }
                GameManager.Instance.totalScore += bonusScore;
            }
        }
    }
}
