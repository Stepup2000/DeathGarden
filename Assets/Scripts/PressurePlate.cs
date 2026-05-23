using System;
using System.Collections;
using UnityEngine;

public class PressurePlate : MonoBehaviour, IPressable
{
    public event Action<IPresser, bool> OnPressChanged;

    [SerializeField] private float releaseDelay = 2f;

    public bool IsPressed { get; private set; }

    private Coroutine releaseRoutine;

    private void OnTriggerEnter(Collider other)
    {
        Handle(other, true);
    }

    private void OnTriggerExit(Collider other)
    {
        Handle(other, false);
    }

    private void Handle(Collider other, bool entered)
    {
        if (!other.TryGetComponent(out MonoBehaviour mb)) return;
        if (mb is not IPresser presser) return;

        if (entered) Press(presser);
        else Release(presser);
    }

    public void Press(IPresser presser)
    {
        if (presser == null) return;

        // cancel delayed release
        if (releaseRoutine != null)
        {
            StopCoroutine(releaseRoutine);
            releaseRoutine = null;
        }

        if (!IsPressed)
        {
            IsPressed = true;
            OnPressChanged?.Invoke(presser, true);
        }
    }

    public void Release(IPresser presser)
    {
        if (presser == null) return;

        if (!IsPressed)
            return;

        if (releaseRoutine == null)
            releaseRoutine = StartCoroutine(ReleaseAfterDelay());
    }

    private IEnumerator ReleaseAfterDelay()
    {
        yield return new WaitForSeconds(releaseDelay);

        releaseRoutine = null;

        if (!IsPressed)
            yield break;

        IsPressed = false;
        OnPressChanged?.Invoke(null, false);
    }
}