using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ennemy : MonoBehaviour
{
    private float m_AttackCooldown;
    private float m_Cooldown;

    public bool b_Boss;
    public int m_Attack;
    public int m_Armor;
    public int m_Rewards;

    private void Update()
    {
        if (b_Boss)
        {
            m_Cooldown -= Time.deltaTime;

            if (m_Cooldown <= 0)
            {
                Attack(m_Attack);
                m_Cooldown = m_AttackCooldown;
            }
        }
    }

    private void Attack(int dmg)
    {

    }

    public void TakeDmg(int dmg)
    { 

    }

    public void SetAttack(int attack)
    {
        m_Attack = attack;
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
