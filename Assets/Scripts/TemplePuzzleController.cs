using System.Collections.Generic;
using UnityEngine;

public class TemplePuzzleController : MonoBehaviour
{
    [Header("Altars to Activate")]
    public List<ElementalAltar> altars = new List<ElementalAltar>();

    [Header("Rewards & Gate")]
    public GameObject silverKeyObject;
    public GameObject bridgeOrBarrier;
    public Vector3 bridgeLowerOffset = new Vector3(0f, -4f, 0f);

    public bool isSolved { get; private set; } = false;

    private void Start()
    {
        if (silverKeyObject != null)
        {
            silverKeyObject.SetActive(false);
        }
    }

    public void OnAltarActivated(ElementalAltar altar)
    {
        if (isSolved) return;

        int activeCount = 0;
        foreach (var a in altars)
        {
            if (a != null && a.isActivated) activeCount++;
        }

        GameManager.Instance?.SetObjective($"Awaken the 3 Elemental Altars ({activeCount}/{altars.Count})");

        if (activeCount >= altars.Count)
        {
            SolvePuzzle();
        }
    }

    private void SolvePuzzle()
    {
        isSolved = true;
        SoundManager.Instance?.PlayKeySound();

        if (silverKeyObject != null)
        {
            silverKeyObject.SetActive(true);
        }

        if (bridgeOrBarrier != null)
        {
            bridgeOrBarrier.transform.position += bridgeLowerOffset;
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.Notify("Trial Complete! The Silver Key has manifested!");
            GameManager.Instance.SetObjective("Claim the Silver Key from the central dais to unlock the Portal!");
        }
    }
}
