using UnityEngine;

public interface IDamagable
{
    int health { get; set; }
    public void TakeDamage(int damage);
    public void Die();
}
