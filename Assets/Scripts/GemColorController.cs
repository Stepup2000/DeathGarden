using UnityEngine;

public class GemColorController : MonoBehaviour
{
    [Header("Gem Renderers")]
    [SerializeField] private GemColor[] gems;

    private GateController gateController;

    private void Awake()
    {
        gateController = GetComponentInParent<GateController>();
    }

    private void OnEnable()
    {
        if (gateController != null)
            gateController.OnPressedPlateCountChanged += HandleCountChanged;
    }

    private void OnDisable()
    {
        if (gateController != null)
            gateController.OnPressedPlateCountChanged -= HandleCountChanged;
    }

    private void Start()
    {
        UpdateGemColors(0);
    }

    private void HandleCountChanged(int count)
    {
        UpdateGemColors(count);
    }

    private void UpdateGemColors(int activeCount)
    {
        if (gems == null) return;

        for (int i = 0; i < gems.Length; i++)
        {
            if (gems[i] == null)
                continue;

            if (i < activeCount)
            {
                gems[i].SetGreen();
            }
            else
            {
                gems[i].SetRed();
            }
        }
    }
}