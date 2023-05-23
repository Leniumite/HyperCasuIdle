using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private float m_Timer = 1;

    private void Update()
    {
        m_Timer -= Time.deltaTime;

        if (m_Timer <= 0)
        {
            GameManager.instance.DamageEnnemy();
            m_Timer = 1;
        }
    }
}
