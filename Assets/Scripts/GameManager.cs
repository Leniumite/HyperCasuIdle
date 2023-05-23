using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.ComponentModel;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("General Variables")]
    private bool b_IsEngagedInCombat = false;
    public bool b_canAttack = false;
    private int m_Money = 0, m_MoneyToUpgradeSpeed = 10, m_MoneyToUpgradeArmor = 10, m_MoneyToUpgradeAttack = 10, m_LvlSpeed = 1, m_LvlArmor = 1, m_LvlAttack = 1;
    private int m_StageNumber = 1, m_EnnemiesKilled = 0, m_EnnemiesBeforeStage = 10;
    private float m_Speed = 0.5f, m_Attack, m_Armor = 1;
    [SerializeField] private GameObject m_EnvironnementPrefab;
    [SerializeField] private Transform m_ActualEnvironnement;
    [SerializeField] private float moveEnvironmentSpeed = 0.25f;

    [Header("Upgrades")]
    [SerializeField] private float m_SpeedUpgrade;

    [SerializeField] private Button Btn_Speed;
    [SerializeField] private Button Btn_Armor;
    [SerializeField] private Button Btn_Attack;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI Txt_Speed;
    [SerializeField] private TextMeshProUGUI Txt_Armor;
    [SerializeField] private TextMeshProUGUI Txt_Attack;
    [SerializeField] private TextMeshProUGUI Txt_MoneyText;
    [SerializeField] private TextMeshProUGUI Txt_StageStep;
    [SerializeField] private TextMeshProUGUI Txt_StepsToBoss;
    [SerializeField] private TextMeshProUGUI Txt_EnemyLife;
    [SerializeField] private GameObject m_FillBar;

    [Header("Ennemies")]
    [SerializeField] private GameObject m_EnnemyPrefab;
    [SerializeField] private GameObject m_BossPrefab;
    private Ennemy m_ActualEnnemy;
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
        Init();
    }

    //Spawn ennemies one by one
    private void Update()
    {
        Btn_Speed.interactable = m_Money >= m_MoneyToUpgradeSpeed ? true : false;
        Btn_Armor.interactable = m_Money >= m_MoneyToUpgradeArmor ? true : false;
        Btn_Attack.interactable = m_Money >= m_MoneyToUpgradeAttack ? true : false;

        if (!b_IsEngagedInCombat)
            SpawnEnnemy();

        float healthRatio = m_ActualEnnemy.m_Health / m_ActualEnnemy.m_MaxHealth;
        m_FillBar.transform.localScale = new Vector3(healthRatio, m_FillBar.transform.localScale.y, m_FillBar.transform.localScale.z);
        Txt_EnemyLife.text = m_ActualEnnemy.m_Health.ToString();
    }

    private void Init()
    {
        Application.targetFrameRate = 60;
        m_ActualEnvironnement = Instantiate(m_EnvironnementPrefab, Vector3.zero, Quaternion.identity).transform;

        if (!PlayerPrefs.HasKey("StepToBoss"))
        {
            PlayerPrefs.SetInt("StepToBoss", m_EnnemiesKilled);
            PlayerPrefs.Save();
        }
        else
        {
            m_EnnemiesKilled = PlayerPrefs.GetInt("StepToBoss");
        }
        Txt_StepsToBoss.text = m_EnnemiesKilled + "/10";
        
        if (!PlayerPrefs.HasKey("StageNumber"))
        {
            PlayerPrefs.SetInt("StageNumber", m_StageNumber);
            PlayerPrefs.Save();
        }
        else
        {
            m_StageNumber = PlayerPrefs.GetInt("StageNumber");
        }
        Txt_StageStep.text = m_StageNumber.ToString();
        
        
        m_Attack = 1;
        m_Player = GameObject.Find("Player");
    }
    
    //Spawn ennemies with a simple prefab and set all the variables depending the stage we are in
    private void SpawnEnnemy()
    { 
        GameObject ennemy = Instantiate(m_EnnemyPrefab, m_EnnemyPrefabPosition.position, m_EnnemyPrefabPosition.rotation);
        Ennemy comp = ennemy.GetComponent<Ennemy>();
        m_ActualEnnemy = comp;
        comp.SetHealth(10 * (m_StageNumber * m_StageNumber));
        comp.SetArmor(Mathf.FloorToInt(m_StageNumber * 1.2f));
        comp.SetRewards(Random.Range(2,10) * m_StageNumber);

        b_IsEngagedInCombat = true;
        b_canAttack = false;
    }

    //Update the specified button
    private void UpdateButton(TextMeshProUGUI text, string contenu)
    {
         text.text = contenu;
    }

    private void MoveEnvironment()
    {
        Vector3 newPos = new Vector3(m_ActualEnvironnement.position.x, m_ActualEnvironnement.position.y, m_ActualEnvironnement.position.z + 30);
        GameObject nextPart = Instantiate(m_EnvironnementPrefab, newPos, m_ActualEnvironnement.rotation);

        StartCoroutine(MoveEnvironmentCoroutine(nextPart.transform));
    }

    private IEnumerator MoveEnvironmentCoroutine(Transform nextPart)
    {
        while (nextPart.position != Vector3.zero)
        {
            nextPart.position = new Vector3(0, 0, nextPart.position.z - moveEnvironmentSpeed);
            m_ActualEnvironnement.position = new Vector3(0, 0, m_ActualEnvironnement.position.z - moveEnvironmentSpeed);
            yield return null;
        }
        
        Destroy(m_ActualEnvironnement.gameObject);
        m_ActualEnvironnement = nextPart.transform;
        b_IsEngagedInCombat = false;
    }

    //All these deaths increments several counters to knoww where the player is in the progression
    public void EnnemyDeath(Ennemy ennemy)
    {
        b_canAttack = true;
        m_Money += ennemy.m_Rewards;
        UpdateMoney();
        Destroy(ennemy.gameObject);
        m_EnnemiesKilled += 1;

        UpdateStepsToBoss();
        
        PlayerPrefs.SetInt("StepToBoss", m_EnnemiesKilled);
        Txt_StepsToBoss.text = (m_EnnemiesKilled+1).ToString() + "/10";
        
        MoveEnvironment();

        if (m_EnnemiesKilled % m_EnnemiesBeforeStage == 0)
        {
            m_StageNumber += 1;
            
            PlayerPrefs.SetInt("StageNumber", m_StageNumber);
            Txt_StageStep.text = m_StageNumber.ToString();
        }
        PlayerPrefs.Save();
    }

    //Mo$t important
    public void UpdateMoney()
    {
        Txt_MoneyText.text = m_Money.ToString();
    }

    public void UpdateStepsToBoss()
    {
        Txt_StepsToBoss.text = (1+(m_EnnemiesKilled % m_EnnemiesBeforeStage)).ToString() + "/10";
    }

    //Called every seconds to hit ennemy
    public void DamageEnnemy()
    {        
        m_ActualEnnemy.TakeDmg(m_Attack);
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
