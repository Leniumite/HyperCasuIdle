using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private float m_Timer = 1;
    private float m_TimerReset = 1;
    public ParticleSystem upgradeParticle;
    
    private void Update()
    {
        m_Timer -= Time.deltaTime;

        if (GameManager.instance.b_canAttack) return;
        
        if (m_Timer <= 0)
        {
            GameManager.instance.DamageEnnemy();
            m_Timer = m_TimerReset / GameManager.instance.GetSpeed();
        }

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Mouse0))
#else
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
#endif
            GameManager.instance.DamageEnnemy();
    }

    public void LateUpdate()
    {
        Turn();
    }

    public void Turn()
    {
        transform.Rotate(new Vector3(0, 0, GameManager.instance.GetSpeed()));
    }
}
