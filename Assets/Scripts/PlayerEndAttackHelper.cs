using UnityEngine;

public class PlayerEndAttackHelper : MonoBehaviour
{
    [SerializeField] ThirdPersonController playerController;

    public void EndAttack()
    {
        if (playerController != null) playerController.EndAttack();
    }
}
