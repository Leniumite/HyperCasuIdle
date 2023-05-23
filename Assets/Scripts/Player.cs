using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private float m_Timer = 1;
    private float m_TimerReset = 1;

    private void Update()
    {
        m_Timer -= Time.deltaTime;

        if (m_Timer <= 0)
        {
            GameManager.instance.DamageEnnemy();
            m_Timer = m_TimerReset/GameManager.instance.GetSpeed();
        }

        Turn();
    }

    public void Turn()
    {
        transform.Rotate(new Vector3(0, 0, GameManager.instance.GetSpeed()));
    }
}
