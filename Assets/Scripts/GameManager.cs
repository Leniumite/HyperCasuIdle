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
    private int m_Money = 0, m_MoneyToUpgradeSpeed = 10, m_MoneyToUpgradeArmor = 10, m_MoneyToUpgradeAttack = 10, m_LvlSpeed = 1, m_LvlArmor = 1, m_LvlAttack = 1;
    private int m_StageNumber = 1, m_EnnemiesKilled = 0;
    private float m_Speed = 0.5f, m_Attack, m_Armor = 1;
    [SerializeField] private GameObject m_EnvironnementPrefab;
    [SerializeField] private Transform m_ActualEnvironnement;

    [Header("Upgrades")]
    [SerializeField] private float m_SpeedUpgrade;

    [SerializeField] private TextMeshProUGUI Txt_Speed;
    [SerializeField] private TextMeshProUGUI Txt_Armor;
    [SerializeField] private TextMeshProUGUI Txt_Attack;
    [SerializeField] private TextMeshProUGUI Txt_MoneyText;

    [SerializeField] private Button Btn_Speed;
    [SerializeField] private Button Btn_Armor;
    [SerializeField] private Button Btn_Attack;

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
        m_Player = GameObject.Find("Player");
    }

    //Spawn ennemies one by one
    private void Update()
    {
        Btn_Speed.interactable = m_Money >= m_MoneyToUpgradeSpeed ? true : false;
        Btn_Armor.interactable = m_Money >= m_MoneyToUpgradeArmor ? true : false;
        Btn_Attack.interactable = m_Money >= m_MoneyToUpgradeAttack ? true : false;

        if (b_IsEngagedInCombat)
            return;

        SpawnEnnemy();
    }

    //Spawn ennemies with a simple prefab and set all the variables depending the stage we are in
    private void SpawnEnnemy()
    {
        GameObject ennemy = Instantiate(m_EnnemyPrefab, m_EnnemyPrefabPosition.position, m_EnnemyPrefabPosition.rotation);
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

    public void MoveEnvironnement()
    {
        float moveDist = 30;

        Vector3 newPos = new Vector3(m_ActualEnvironnement.position.x, m_ActualEnvironnement.position.y, m_ActualEnvironnement.position.z + 30);
        GameObject nextPart = Instantiate(m_EnvironnementPrefab, newPos, m_ActualEnvironnement.rotation);



        m_ActualEnvironnement = nextPart.transform;
        b_IsEngagedInCombat = false;
    }

    //All these deaths increments several counters to knoww where the player is in the progression
    public void EnnemyDeath(Ennemy ennemy)
    {
        Debug.Log("Dead");
        m_Money += ennemy.m_Rewards;
        UpdateMoney();
        Destroy(ennemy.gameObject);
        m_EnnemiesKilled += 1;

        MoveEnvironnement();
        //m_StageNumber += 1;
    }

    //Mo$t important
    public void UpdateMoney()
    {
        Txt_MoneyText.text = m_Money.ToString();
    }

    //Called every seconds to hit ennemy
    public void DamageEnnemy()
    {
        Debug.Log(m_Attack);
        
        m_ActualEnnemy.GetComponent<Ennemy>().TakeDmg(m_Attack);
    }

    public void UpgradeSpeed()
    {
        m_Speed += m_SpeedUpgrade;
        m_LvlSpeed++;
        m_Money -= m_MoneyToUpgradeSpeed;

        UpdateMoney();
        UpdateButton(Txt_Speed, m_LvlSpeed.ToString() + "\nSpeed\n" + m_MoneyToUpgradeSpeed.ToString());
    }

    public void UpgradeArmor()
    {
        m_Armor += Mathf.CeilToInt(m_Armor / 10);
        m_LvlArmor++;
        m_Money -= m_MoneyToUpgradeArmor;

        UpdateMoney();
        UpdateButton(Txt_Armor, m_LvlArmor.ToString() + "\nArmor\n" + m_MoneyToUpgradeArmor.ToString());
    }

    public void UpgradeAttack()
    {
        m_Attack += Mathf.CeilToInt(m_Attack / 10);
        m_LvlAttack++;
        m_Money -= m_MoneyToUpgradeAttack;

        UpdateMoney();
        UpdateButton(Txt_Attack, m_LvlAttack.ToString() + "\nAttack\n" + m_MoneyToUpgradeAttack.ToString());
    }

    public float GetSpeed()
    {
        return m_Speed;
    }
}
