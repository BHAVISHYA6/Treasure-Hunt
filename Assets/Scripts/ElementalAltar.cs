using UnityEngine;

public class ElementalAltar : MonoBehaviour, IInteractable
{
    public enum ElementType { Fire, Earth, Water }

    [Header("Altar Configuration")]
    public ElementType elementType = ElementType.Fire;
    public string altarName = "Ruby Fire Altar";
    public Color elementColor = Color.red;

    [Header("Visual Indicators")]
    public Light beaconLight;
    public ParticleSystem beaconParticles;
    public Renderer crystalRenderer;

    public bool isActivated { get; private set; } = false;

    private void Start()
    {
        if (beaconLight != null) beaconLight.enabled = false;
        if (beaconParticles != null) beaconParticles.Stop();
    }

    public string GetInteractionPrompt()
    {
        if (isActivated) return $"{altarName} is active.";
        return $"Press [E] to Ignite {altarName}";
    }

    public bool CanInteract(PlayerController player) => !isActivated;

    public void Interact(PlayerController player)
    {
        if (isActivated) return;
        isActivated = true;

        SoundManager.Instance?.PlayAltarSound();

        if (beaconLight != null)
        {
            beaconLight.enabled = true;
            beaconLight.color = elementColor;
        }

        if (beaconParticles != null)
        {
            beaconParticles.Play();
        }

        if (crystalRenderer != null)
        {
            crystalRenderer.material.color = elementColor;
        }

        GameManager.Instance?.Notify($"{altarName} awakened!");

        // Notify TemplePuzzleController in the scene
        var puzzle = FindObjectOfType<TemplePuzzleController>();
        if (puzzle != null)
        {
            puzzle.OnAltarActivated(this);
        }
    }
}
