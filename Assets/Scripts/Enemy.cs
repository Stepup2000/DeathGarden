using UnityEngine;

public class Enemy : IDamagable
{
    public int health { get; set; } = 100;
    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        // Implement death logic here
        Debug.Log("Enemy died.");
    }
}
