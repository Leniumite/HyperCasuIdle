using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.ComponentModel;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("General Variables")]
    private bool b_IsEngagedInCombat = false;
    private int m_Money = 0, m_MoneyToUpgradeSpeed = 100, m_MoneyToUpgradeArmor = 100, m_MoneyToUpgradeAttack = 100, m_LvlSpeed = 1, m_LvlArmor = 1, m_LvlAttack = 1;
    private int m_StageNumber = 1, m_EnnemiesKilled = 0;
    private float m_Speed = 1, m_Attack, m_Armor = 1;

    [Header("Upgrades")]
    [SerializeField] private float m_SpeedUpgrade;

    [SerializeField] private TextMeshProUGUI Txt_Speed;
    [SerializeField] private TextMeshProUGUI Txt_Armor;
    [SerializeField] private TextMeshProUGUI Txt_Attack;

    [Header("Ennemies")]
    [SerializeField] private GameObject m_EnnemyPrefab;
    private GameObject m_ActualEnnemy;
    [SerializeField] private Transform m_EnnemyPrefabPosition;

    public GameObject m_Player;

    //Singleton
    private void Awake()
    {
        if (instance != null)
            DestroyImmediate(instance);
        else
            instance = this;
    }

    //Just to set player ref
    private void Start()
    {
        m_Attack = 1;
        m_Player = GameObject.Find("MainToupie");
    }

    //Spawn ennemies one by one
    private void Update()
    {
        if (b_IsEngagedInCombat)
            return;

        SpawnEnnemy();
    }

    //Spawn ennemies with a simple prefab and set all the variables depending the stage we are in
    private void SpawnEnnemy()
    {
        GameObject ennemy = Instantiate(m_EnnemyPrefab, m_EnnemyPrefabPosition.position, Quaternion.identity);
        m_ActualEnnemy = ennemy;
        Ennemy comp = ennemy.GetComponent<Ennemy>();
        comp.SetHealth(10 * (m_StageNumber * m_StageNumber));
        comp.SetArmor(Mathf.FloorToInt(m_StageNumber * 1.2f));
        comp.SetRewards(Random.Range(2,10) * m_StageNumber);

        b_IsEngagedInCombat = true;
    }

    //Update the specified button
    private void UpdateButton(TextMeshProUGUI text, string contenu)
    {
         text.text = contenu;
    }

    public void EnnemyDeath(Ennemy ennemy)
    {
        Debug.Log("Dead");
        m_Money += ennemy.m_Rewards;
        Destroy(ennemy.gameObject);
        m_EnnemiesKilled += 1;
        m_StageNumber += 1;

        b_IsEngagedInCombat = false;
    }

    //Called every seconds to hit ennemy
    public void DamageEnnemy()
    {
        Debug.Log(m_Attack);
        
        m_ActualEnnemy.GetComponent<Ennemy>().TakeDmg(m_Attack);
    }

    public void GetRewards()
    {

    }

    public void UpgradeSpeed()
    {
        m_Speed += m_SpeedUpgrade;
        m_LvlSpeed++;

        UpdateButton(Txt_Speed, m_LvlSpeed.ToString() + "\nSpeed\n" + m_MoneyToUpgradeSpeed.ToString());
    }

    public void UpgradeArmor()
    {
        m_Armor += Mathf.CeilToInt(m_Armor / 10);
        m_LvlArmor++;

        UpdateButton(Txt_Armor, m_LvlArmor.ToString() + "\nArmor\n" + m_MoneyToUpgradeArmor.ToString());
    }

    public void UpgradeAttack()
    {
        m_Attack += Mathf.CeilToInt(m_Attack / 10);
        m_LvlAttack++;

        UpdateButton(Txt_Attack, m_LvlAttack.ToString() + "\nAttack\n" + m_MoneyToUpgradeAttack.ToString());
    }

    
}
