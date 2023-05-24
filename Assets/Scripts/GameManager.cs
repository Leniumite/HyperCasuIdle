using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.ComponentModel;
using DG.Tweening;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("General Variables")]
    private bool b_IsEngagedInCombat = false;
    public bool b_canAttack = false;
    private int m_Money = 0, m_LvlSpeed = 1, m_LvlArmor = 1, m_LvlAttack = 1;
    private int m_StageNumber = 1, m_EnnemiesKilled = 0, m_EnnemiesBeforeStage = 10;
    private float m_Speed = 0.5f, m_Attack = 1, m_MoneyToUpgradeSpeed = 10, m_MoneyToUpgradeArmor = 10, m_MoneyToUpgradeAttack = 10;
    [SerializeField] private GameObject m_EnvironnementPrefab;
    [SerializeField] private Transform m_ActualEnvironnement;
    [SerializeField] private float moveEnvironmentSpeed = 0.25f;

    private Tweener m_enemyPunchTween;
    private Tweener m_playerPunchTween;
    [SerializeField] private float rangePunchPosition = 0.25f;

    [Header("Upgrades")]
    [SerializeField] private float m_SpeedUpgrade;

    [SerializeField] private Button Btn_Speed;
    [SerializeField] private Button Btn_Armor;
    [SerializeField] private Button Btn_Attack;

    [Header("UI")]
    [Header("Speed")]
    [SerializeField] private TextMeshProUGUI Txt_Speed;
    [SerializeField] private TextMeshProUGUI Txt_SpeedLvl;
    [SerializeField] private TextMeshProUGUI Txt_SpeedCost;
    [Header("Armor")]
    [SerializeField] private TextMeshProUGUI Txt_Armor;
    [SerializeField] private TextMeshProUGUI Txt_ArmorLvl;
    [SerializeField] private TextMeshProUGUI Txt_ArmorCost;
    [Header("Attack")]
    [SerializeField] private TextMeshProUGUI Txt_Attack;
    [SerializeField] private TextMeshProUGUI Txt_AttackLvl;
    [SerializeField] private TextMeshProUGUI Txt_AttackCost;
    [Space]
    [SerializeField] private TextMeshProUGUI Txt_MoneyText;
    [SerializeField] private TextMeshProUGUI Txt_StageStep;
    [SerializeField] private TextMeshProUGUI Txt_StepsToBoss;
    [SerializeField] private TextMeshProUGUI Txt_EnemyLife;
    [SerializeField] private GameObject m_FillBar;
    [SerializeField] private GameObject m_TimeBar;
    [SerializeField] private Button moneyReward;
    
    [Header("Ennemies")]
    [SerializeField] private GameObject m_EnnemyPrefab;
    [SerializeField] private GameObject m_BossPrefab;
    private Ennemy m_ActualEnnemy;
    [SerializeField] private Transform m_EnnemyPrefabPosition;
    private bool b_BossPhase = false;
    private float m_ActualBossTime;
    [SerializeField] private float m_TimeToDefeatBoss;

    public Player m_Player;

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
        m_ActualBossTime = m_TimeToDefeatBoss;

        m_TimeBar.SetActive(false);

        AdsManager.instance.OnShowAdsRewardedComplete += () =>
        {
            m_Money += 20;
            UpdateMoney();
        };
        
        moneyReward.onClick.AddListener(() =>
        {
            AdsManager.instance.LoadAdRewarded();
        });
    }

    //Spawn ennemies one by one
    private void Update()
    {
        Btn_Speed.interactable = m_Money >= m_MoneyToUpgradeSpeed ? true : false;
        Btn_Armor.interactable = m_Money >= m_MoneyToUpgradeArmor ? true : false;
        Btn_Attack.interactable = m_Money >= m_MoneyToUpgradeAttack ? true : false;

        if (!b_IsEngagedInCombat)
            SpawnEnnemy();

        float healthRatio = Mathf.Max(0, m_ActualEnnemy.m_Health / m_ActualEnnemy.m_MaxHealth);
        m_FillBar.transform.localScale = new Vector3(healthRatio, m_FillBar.transform.localScale.y, m_FillBar.transform.localScale.z);
        Txt_EnemyLife.text = m_ActualEnnemy.m_Health.ToString();

        if (b_BossPhase)
        {
            m_TimeBar.SetActive(true);
            m_ActualBossTime -= Time.deltaTime;

            float timeRatio = Mathf.Max(0, m_ActualBossTime / m_TimeToDefeatBoss);
            m_TimeBar.transform.localScale = new Vector3(timeRatio, m_TimeBar.transform.localScale.y, m_TimeBar.transform.localScale.z);

            //If the boss don't die but the time reaches 0
            if(m_ActualBossTime <= 0)
            {
                m_ActualBossTime = 0;
                b_BossPhase = false;
                m_TimeBar.SetActive(false);
                m_ActualBossTime = m_TimeToDefeatBoss;
                b_canAttack = true;
                Destroy(m_ActualEnnemy);
            }
        }
    }

    private void Init()
    {
        Application.targetFrameRate = 60;
        //m_ActualEnvironnement = Instantiate(m_EnvironnementPrefab, Vector3.zero, Quaternion.identity).transform;

        if (!PlayerPrefs.HasKey("StepToBoss"))
        {
            PlayerPrefs.SetInt("StepToBoss", m_EnnemiesKilled);
            PlayerPrefs.Save();
        }
        else
        {
            m_EnnemiesKilled = PlayerPrefs.GetInt("StepToBoss");
        }
        Txt_StepsToBoss.text = (m_EnnemiesKilled + 1) + "/10";
        
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

        if (!PlayerPrefs.HasKey("Money"))
        {
            PlayerPrefs.SetInt("Money", m_Money);
            PlayerPrefs.Save();

        }
        else
        {
            m_Money = PlayerPrefs.GetInt("Money");
        }
        Txt_MoneyText.text = m_Money.ToString();

        InitPlayerPrefSpeed();
        InitPlayerPrefArmor();
        InitPlayerPrefAttack();
        
        m_Player = GameObject.Find("Player").GetComponent<Player>();
    }

    #region PlayerPrefs
    
    private void InitPlayerPrefSpeed()
    {
        if (!PlayerPrefs.HasKey("Speed"))
        {
            PlayerPrefs.SetFloat("Speed", m_Speed);
            PlayerPrefs.Save();
        }
        else
        {
            m_Speed = PlayerPrefs.GetFloat("Speed");
        }
        
        if (!PlayerPrefs.HasKey("SpeedLvl"))
        {
            PlayerPrefs.SetInt("SpeedLvl", m_LvlSpeed);
            PlayerPrefs.Save();
        }
        else
        {
            m_LvlSpeed = PlayerPrefs.GetInt("SpeedLvl");
        }
        
        if (!PlayerPrefs.HasKey("SpeedCost"))
        {
            PlayerPrefs.SetInt("SpeedCost", (int)m_MoneyToUpgradeSpeed);
            PlayerPrefs.Save();
        }
        else
        {
            m_MoneyToUpgradeSpeed = PlayerPrefs.GetInt("SpeedCost");
        }
        
        UpdateButton(Txt_SpeedLvl,  "Lvl : " + m_LvlSpeed);
        UpdateButton(Txt_SpeedCost, m_MoneyToUpgradeSpeed + " $");
    }
    
    private void InitPlayerPrefArmor()
    {
        if (!PlayerPrefs.HasKey("TimeToBoss"))
        {
            PlayerPrefs.SetFloat("TimeToBoss", m_TimeToDefeatBoss);
            PlayerPrefs.Save();
        }
        else
        {
            m_TimeToDefeatBoss = PlayerPrefs.GetFloat("Armor");
        }
        
        if (!PlayerPrefs.HasKey("ArmorLvl"))
        {
            PlayerPrefs.SetInt("ArmorLvl", m_LvlArmor);
            PlayerPrefs.Save();
        }
        else
        {
            m_LvlArmor = PlayerPrefs.GetInt("ArmorLvl");
        }
        
        if (!PlayerPrefs.HasKey("ArmorCost"))
        {
            PlayerPrefs.SetInt("ArmorCost", (int)m_MoneyToUpgradeArmor);
            PlayerPrefs.Save();
        }
        else
        {
            m_MoneyToUpgradeArmor = PlayerPrefs.GetInt("ArmorCost");
        }
        
        UpdateButton(Txt_ArmorLvl,  "Lvl : " + m_LvlArmor);
        UpdateButton(Txt_ArmorCost, m_MoneyToUpgradeArmor + " $");
    }
    
    private void InitPlayerPrefAttack()
    {
        if (!PlayerPrefs.HasKey("Attack"))
        {
            PlayerPrefs.SetFloat("Attack", m_Attack);
            PlayerPrefs.Save();
        }
        else
        {
            m_Attack = PlayerPrefs.GetFloat("Attack");
        }
        
        if (!PlayerPrefs.HasKey("AttackLvl"))
        {
            PlayerPrefs.SetInt("AttackLvl", m_LvlAttack);
            PlayerPrefs.Save();
        }
        else
        {
            m_LvlAttack = PlayerPrefs.GetInt("AttackLvl");
        }
        
        if (!PlayerPrefs.HasKey("AttackCost"))
        {
            PlayerPrefs.SetInt("AttackCost", (int)m_MoneyToUpgradeAttack);
            PlayerPrefs.Save();
        }
        else
        {
            m_MoneyToUpgradeAttack = PlayerPrefs.GetInt("AttackCost");
        }
        
        UpdateButton(Txt_AttackLvl,  "Lvl : " + m_LvlAttack);
        UpdateButton(Txt_AttackCost, m_MoneyToUpgradeAttack + " $");
    }

    private void SavePlayerPrefSpeed()
    {
        PlayerPrefs.SetFloat("Speed", m_Speed);
        PlayerPrefs.SetInt("SpeedLvl", m_LvlSpeed);
        PlayerPrefs.SetInt("SpeedCost", (int)m_MoneyToUpgradeSpeed);
        PlayerPrefs.Save();
    }
    
    private void SavePlayerPrefArmor()
    {
        PlayerPrefs.SetFloat("Armor", m_TimeToDefeatBoss);
        PlayerPrefs.SetInt("ArmorLvl", m_LvlArmor);
        PlayerPrefs.SetInt("ArmorCost", (int)m_MoneyToUpgradeArmor);
        PlayerPrefs.Save();
    }
    
    private void SavePlayerPrefAttack()
    {
        PlayerPrefs.SetFloat("Attack", m_Attack);
        PlayerPrefs.SetInt("AttackLvl", m_LvlAttack);
        PlayerPrefs.SetInt("AttackCost", (int)m_MoneyToUpgradeAttack);
        PlayerPrefs.Save();
    }
    
    #endregion

    //Spawn ennemies with a simple prefab and set all the variables depending the stage we are in
    private void SpawnEnnemy()
    {
        Ennemy comp;

        if (b_BossPhase)
        {
            GameObject ennemy = Instantiate(m_BossPrefab, m_EnnemyPrefabPosition.position, m_EnnemyPrefabPosition.rotation);
            comp = ennemy.GetComponent<Ennemy>();
            comp.b_Boss = true;
        }
        else
        {
            GameObject boss = Instantiate(m_EnnemyPrefab, m_EnnemyPrefabPosition.position, m_EnnemyPrefabPosition.rotation);
            comp = boss.GetComponent<Ennemy>();
        }
        
        m_ActualEnnemy = comp;
        comp.SetHealth(10 * (m_StageNumber * m_StageNumber));
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

        //If the actual ennemy was not a boss
        if (ennemy.b_Boss == false)
        {
            m_EnnemiesKilled += 1;
            PlayerPrefs.SetInt("StepToBoss", m_EnnemiesKilled);
            Txt_StepsToBoss.text = ((m_EnnemiesKilled % m_EnnemiesBeforeStage) + 1).ToString() + "/10";

            //Check if we get the good numbers of ennemies to spawn boss
            if (m_EnnemiesKilled % m_EnnemiesBeforeStage == 0)
                b_BossPhase = true;
        }
        //If the boss die
        else
        {
            m_TimeBar.SetActive(false);
            m_ActualBossTime = m_TimeToDefeatBoss;
            b_canAttack = true;
            b_BossPhase = false;
            m_StageNumber += 1;
            Txt_StageStep.text = m_StageNumber.ToString();
            
            PlayerPrefs.SetInt("StageNumber", m_StageNumber);
            PlayerPrefs.SetInt("StepToBoss", 0);
        }
        
        MoveEnvironment();

        PlayerPrefs.Save();
    }

    //Mo$t important
    public void UpdateMoney()
    {
        PlayerPrefs.SetInt("Money", m_Money);
        PlayerPrefs.Save();
        Txt_MoneyText.text = m_Money.ToString();
    }

    //Called every seconds to hit ennemy
    public void DamageEnnemy()
    {
        m_ActualEnnemy.TakeDmg(m_Attack);
        
        m_enemyPunchTween.Complete();
        m_enemyPunchTween = m_ActualEnnemy.transform.DOPunchPosition(new Vector3(Random.Range(-rangePunchPosition, rangePunchPosition), m_ActualEnnemy.transform.position.y, Random.Range(-rangePunchPosition, rangePunchPosition)), 0.5f, 5);
    }

    public void UpgradeSpeed()
    {
        m_Speed += m_SpeedUpgrade;
        m_LvlSpeed++;
        m_Money -= (int)m_MoneyToUpgradeSpeed;
        m_Player.upgradeParticle.gameObject.SetActive(true);
        m_Player.upgradeParticle.Play();
        SavePlayerPrefSpeed();
        UpdateMoney();
        UpdateButton(Txt_SpeedLvl,  "Lvl : " + m_LvlSpeed);
        m_MoneyToUpgradeSpeed = Mathf.FloorToInt(m_MoneyToUpgradeSpeed * 1.05f);
        UpdateButton(Txt_SpeedCost, m_MoneyToUpgradeSpeed + " $");
    }

    public void UpgradeArmor()
    {
        m_LvlArmor++;
        m_TimeToDefeatBoss += (m_LvlArmor/100);
        m_Money -= (int)m_MoneyToUpgradeArmor;
        m_Player.upgradeParticle.gameObject.SetActive(true);
        m_Player.upgradeParticle.Play();
        SavePlayerPrefArmor();
        UpdateMoney();
        UpdateButton(Txt_ArmorLvl,  "Lvl : " + m_LvlArmor);
        m_MoneyToUpgradeArmor = Mathf.FloorToInt(m_MoneyToUpgradeArmor * 1.05f);
        UpdateButton(Txt_ArmorCost, m_MoneyToUpgradeArmor + " $");
    }

    public void UpgradeAttack()
    {
        m_Attack += Mathf.CeilToInt(m_Attack / 10);
        m_LvlAttack++;
        m_Money -= (int)m_MoneyToUpgradeAttack;

        m_Player.upgradeParticle.gameObject.SetActive(true);
        m_Player.upgradeParticle.Play();
        SavePlayerPrefAttack();
        UpdateMoney();
        UpdateButton(Txt_AttackLvl,  "Lvl : " + m_LvlAttack);
        m_MoneyToUpgradeAttack = Mathf.FloorToInt(m_MoneyToUpgradeAttack * 1.05f);
        UpdateButton(Txt_AttackCost, m_MoneyToUpgradeAttack + " $");
    }

    public float GetSpeed()
    {
        return m_Speed;
    }
}
