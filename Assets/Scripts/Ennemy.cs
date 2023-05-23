using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ennemy : MonoBehaviour
{
    private float m_AttackCooldown;
    private float m_Cooldown;

    public bool b_Boss;
    public float m_MaxHealth;
    public float m_Health;
    public int m_Armor;
    public int m_Rewards;

    private void Update()
    {
        if (m_Health <= 0)
            GameManager.instance.EnnemyDeath(this);

        Turn();
    }

    public void Turn()
    {
        transform.Rotate(new Vector3(0, 0, 0.8f));
    }

    public void TakeDmg(float dmg)
    {
        m_Health -= dmg;
    }

    public void SetHealth(int health)
    {
        m_MaxHealth = health;
        m_Health = health;
    }

    public void SetArmor(int armor)
    {
        m_Armor = armor;
    }

    public void SetRewards(int rewards)
    {
        m_Rewards = rewards;
    }
}
