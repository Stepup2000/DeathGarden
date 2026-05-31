using System;
using UnityEngine;

public class GateController : MonoBehaviour
{
    [Header("Gate")]
    [SerializeField] private string openTrigger = "Open";

    [Header("Plates")]
    [SerializeField] private PressurePlate[] plates;

    public event Action<int> OnPressedPlateCountChanged;

    private Animator animator;
    private Collider gateCollider;
    private bool isOpen;

    private void OnEnable()
    {
        if (plates == null) return;

        foreach (var plate in plates)
        {
            if (plate == null) continue;
            plate.OnPressChanged += HandlePlateChanged;
        }

        animator = GetComponent<Animator>();
        gateCollider = GetComponent<Collider>();
    }

    private void OnDisable()
    {
        if (plates == null) return;

        foreach (var plate in plates)
        {
            if (plate == null) continue;
            plate.OnPressChanged -= HandlePlateChanged;
        }
    }

    private void HandlePlateChanged(IPresser presser, bool pressed)
    {
        int pressedCount = GetPressedPlateCount();
        OnPressedPlateCountChanged?.Invoke(pressedCount);

        CheckState();
    }

    private int GetPressedPlateCount()
    {
        if (plates == null)
            return 0;

        int count = 0;

        foreach (var plate in plates)
        {
            if (plate != null && plate.IsPressed)
                count++;
        }

        return count;
    }

    private void CheckState()
    {
        if (isOpen) return;
        if (plates == null || plates.Length == 0) return;

        foreach (var plate in plates)
        {
            if (plate == null || !plate.IsPressed)
                return;
        }

        OpenGate();
    }

    private void OpenGate()
    {
        isOpen = true;

        if (gateCollider != null)
            gateCollider.enabled = false;

        if (animator != null)
            animator.SetTrigger(openTrigger);

        Debug.Log("Gate opened");
    }
}