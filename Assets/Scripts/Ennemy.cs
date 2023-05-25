using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Ennemy : MonoBehaviour
{
    private float m_AttackCooldown;
    private float m_Cooldown;

    public ParticleSystem hitParticle;
    public bool b_Boss;
    public float m_MaxHealth;
    public float m_Health;
    public int m_Rewards;

    private void Start()
    {
        var test = GetComponent<MeshRenderer>();
        test.material.color = Random.ColorHSV();
    }

    private void LateUpdate()
    {
        Turn();
    }

    public void Turn()
    {
        transform.Rotate(new Vector3(0, 0, 0.8f));
    }

    public void TakeDmg(float dmg)
    {
        hitParticle.gameObject.SetActive(true);
        hitParticle.Play();
        GameManager.instance.m_EnnemyHit.Play();

        m_Health -= dmg;

        if (m_Health <= 0)
        {
            m_Health = 0;
            GameManager.instance.EnnemyDeath(this);
        }
    }

    public void SetHealth(int health)
    {
        if (b_Boss)
        {
            m_MaxHealth = health * 50;
            m_Health = health * 50;
        }
        else
        {
            m_MaxHealth = health;
            m_Health = health;
        }
    }

    public void SetRewards(int rewards)
    {
        if(b_Boss)
            m_Rewards = rewards * 8;
        else
            m_Rewards = rewards;
    }
}
