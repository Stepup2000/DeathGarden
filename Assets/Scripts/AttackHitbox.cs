using System.Collections.Generic;
using UnityEngine;

public class WeaponHitbox : MonoBehaviour
{
    [SerializeField] private int damage = 10;

    private HashSet<IDamagable> hitEnemies = new();

    private void OnEnable()
    {
        hitEnemies.Clear();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out IDamagable enemy))
        {
            if (hitEnemies.Add(enemy))
            {
                enemy.TakeDamage(damage);
            }
        }
    }
}