using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TestPlayer : MonoBehaviour
{
    public static TestPlayer instance;

    //�÷��̾� ���µ��������� �� ���� ����
    #region
    public PlayerState playerState;
    public LiveState liveState;
    public AttackState attackState;

    public PlayerStats playerstats = new PlayerStats(5000, 1, 100, 1, 0, 100, 10000000, 0, 24);
    public PlayerStats PlayerStats
    {
        get { return playerstats; }
        set { playerstats = value; }
    }
    public NeedExp needExp = new NeedExp(30);
    public NeedExp NeedExp
    {
        get { return needExp; }
        set { needExp = value; }
    }
    #endregion

    //���� Ŭ���� ����
    #region
    [HideInInspector]
    public StageMove stagemove;
    [HideInInspector]
    public Spawner spawner;
    [HideInInspector]
    public Sensor sensor;
    [HideInInspector]
    public Equip equip;
    [HideInInspector]
    public Infotext infotxt;

    Stage_Management stageManager;
    System_Manager systemManager;
    WeaponDmg WPDmg;
    RefBool<bool> refpierce;
    RefBool<bool> refwindmill;
    RefBool<bool> refchanneling;
    RefBool<bool> reflandslash;
    #endregion

    //�÷��̾� ���Ȱ���
    #region
    
    public float maxhp;
    public float hp;
    public int hplevel;
    public int attackdmg;
    public int attackdmglevel;
    public float genhp;
    public int ticket;
    public int gold;
    public int exp;
    public int level;
    public int curneedexp;

    float genTime;
    bool setstats; //���� ����
    bool set;
    #endregion

    //�÷��̾� ���� ����
    #region
    public float attackRadius = 2f;
    public float attackRange = 2f;
    #endregion

    //�÷��̾� ���
    #region
    [SerializeField]
    private Transform weaponPos;
    [SerializeField]
    private Transform armorPos;
    public GameObject[] weapons;
    public GameObject[] armors;
    #endregion

    //����
    #region
    //������ ���� Collider
    public GameObject attackobj;
    GameObject Lock;

    //��ų Image
    public Image pierceimg;
    public Image windmillimg;
    public Image channelingimg;
    public Image landslashimg;
    public GameObject pierceLockImg;
    public GameObject windmilLockImg;
    public GameObject channelingLockImg;
    public GameObject landslashLockImg;

    //Effect
    public ParticleSystem[] attackEft;
    public ParticleSystem pierceEft;
    public ParticleSystem windmilEft;
    public ParticleSystem channelingEft;
    public ParticleSystem landslashEft;

    int skillNum;//����ϴ� ��ų ��ȣ
    int skillDmg;
    public float durationtime; //���ӽð�
    bool _attack;
    bool _skill;

    bool _pierce; //��ų ȹ��� true
    bool _windmill;
    bool _channeling;
    bool _landslash;
    bool _generateHP;

    //��Ÿ��
    bool pierce_Cooldown;
    bool windmill_Cooldown;
    bool channeling_Cooldown;
    bool landslash_Cooldown;

    //���� ��� ��ų����
    bool nextSkill;
    #endregion

    //�ǰ�
    #region
    public GameObject dmgText;
    public Transform dmgPos;
    #endregion

    //����
    #region
    AudioSource audioSource;
    public AudioClip[] shout;
    public AudioClip[] audioAttack;
    public AudioClip audioPierce;
    public AudioClip audioWindmill;
    public AudioClip audioChanneling;
    public AudioClip audioLandslash;
    #endregion

    //Ÿ���ð� �̵�����
    #region
    NavMeshAgent nav;
    public Transform target;
    public GameObject[] sensorlist; //���� Ŭ���� �� �̵��� ���� �ޱ�
    public List<Enemy> monsterlist;//�÷����ϴ� ������ ���͹迭
    public double[] monsterdistance;
    public Transform playerpos; //�÷��̾��� ���� �������� �� ����
    int layerMask; //�ν��� ���̾��ũ ����
    #endregion

    //UI
    #region
    public Text leveltxt;
    #endregion

    int countsensor; //���° �������� ī��Ʈ
    //�Ķ���� ����ȭ
    #region
    static readonly int _Pierce = Animator.StringToHash("Pierce");
    static readonly int _WindMill = Animator.StringToHash("WindMill");
    static readonly int _Channeling = Animator.StringToHash("Channeling");
    static readonly int _LandSlash = Animator.StringToHash("LandSlash");
    #endregion

    //�ڷ�ƾ ����ȭ
    #region
    readonly WaitForFixedUpdate WfU = new WaitForFixedUpdate();
    #endregion

    MeshRenderer[] meshs;
    Animator anim;
    Animation ani;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        if (instance)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        ani = GetComponent<Animation>();
        meshs = GetComponentsInChildren<MeshRenderer>();
        audioSource = GetComponentInChildren<AudioSource>();
    }
    private void OnLevelWasLoaded(int level)
    {
        
    }
    private void Start()
    {
        stageManager = FindObjectOfType<Stage_Management>();
        systemManager = FindObjectOfType<System_Manager>();
        stagemove = FindObjectOfType<StageMove>();
        equip = FindObjectOfType<Equip>();

        if (!setstats)
            SetStats();
        setstats = true;

        layerMask = 1 << LayerMask.NameToLayer("Enemy");

        refpierce = new RefBool<bool>(ref pierce_Cooldown);
        refwindmill = new RefBool<bool>(ref windmill_Cooldown);
        refchanneling = new RefBool<bool>(ref channeling_Cooldown);
        reflandslash = new RefBool<bool>(ref landslash_Cooldown);
        UpdateLevel();
    }

    private void Update()
    {
        if (target != null)
        {
            nav.SetDestination(target.position);
            transform.LookAt(target.position);
        }
        StateManagement();
        StageMove();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Spawner"))
        {
            spawner = other.GetComponent<Spawner>();
            countsensor = 0; //���������� ������ Ȱ��ȭ�� �ʱ�ȭ
            Array.Resize(ref sensorlist, spawner.sensorList.Length);
            for (int i = 0; i < spawner.sensorList.Length; i++)
            {
                sensorlist[i] = spawner.sensorList[i];
            }
            sensor = sensorlist[0].GetComponent<Sensor>(); //ù ���� �ν�
            target = sensorlist[countsensor].transform; //������ ������ ������ target���� ����
            StartCoroutine(DelayTime(1f));
        }
        if (other.CompareTag("Sensor"))
        {
            sensor = other.GetComponent<Sensor>();
            for (int i = 0; i < spawner.spawnPoint[countsensor].transform.childCount; i++)
            {
                monsterlist.Add(spawner.spawnPoint[countsensor].transform.GetChild(i).gameObject.GetComponentInChildren<Enemy>());
            }
            countsensor++;
            SetTarget();
        }
        if (other.CompareTag("EnemyAttack"))
        {
            if (playerState == PlayerState.Live)
            {
                StartCoroutine(DamageAction());
            }
        }
    }

    /// <summary>
    /// ���µ���������
    /// </summary>
    #region
    void StateManagement()
    {
        if (playerState == PlayerState.Live)
        {
            AttackSensorRange();

            if (!sensor.move)
            {
                Targetting();
            }

            if (_generateHP && hp < maxhp)
                StartCoroutine(GenerateHP());

            //������尡 �ƴ� ��
            if (liveState == LiveState.Walk || liveState == LiveState.Idle)
            {
                attackState = AttackState.None;
            }

            // ��������� �� ���� �׼�
            if (liveState == LiveState.Attack)
            {
                AttackAction();
                if (!_attack)
                {
                    _attack = !_attack;
                    anim.SetBool("Attack", _attack);
                    StartCoroutine(BasicAttack());
                }

                // �ڵ� ��������� �� ��ų���
                if (systemManager.autoPlay)
                {
                    if (!_skill)
                    {
                        _skill = !_skill;
                        if (attackState == AttackState.Pierce && _pierce)
                        {
                            Pierce();
                        }
                        else if (attackState == AttackState.Windmill && _windmill)
                        {
                            WindMill();
                        }
                        else if (attackState == AttackState.Channeling && _channeling)
                        {
                            Channeling();
                        }
                        else if (attackState == AttackState.Landslash && _landslash)
                        {
                            LandSlash();
                        }
                    }
                }
            }
        }
        else if (playerState == PlayerState.Die && !set)
        {
            set = true; //��� �� �ʱ�ȭ ����
            target = null;
            anim.SetTrigger("Die"); //����ִϸ��̼� ȣ��
            liveState = LiveState.Idle; //state �ʱ�ȭ
            attackState = AttackState.None;
            StageReSet();
        }
    }
    #endregion

    /// <summary>
    /// "Player"�� ���� ����
    /// </summary>
    #region
    void SetStats()
    {
        maxhp = playerstats.maxHp;
        hp = maxhp;
        hplevel = playerstats.hpLevel;
        attackdmg = playerstats.attack;
        attackdmglevel = playerstats.attackLevel;
        genhp = playerstats.genHp;
        level = playerstats.level;
        ticket = playerstats.ticket;
        gold = playerstats.gold;
        exp = playerstats.exp;
        curneedexp = needExp.needExp;

        systemManager.playerticket[0].text = ticket.ToString();
        systemManager.playergold[0].text = gold.ToString();
        leveltxt.text = "Lv" + level.ToString();
    }

    public void UpdateLevel()
    {
        if (exp != 0)
        {
            while (exp >= curneedexp)
            {
                exp -= curneedexp;
                level += 1;
                ticket += 1;
                curneedexp = needExp.needExp + ((level-1)*15);
                leveltxt.text = "Lv" + level.ToString();
            }
        }
        if (level >= 1)
        {
            _generateHP = true;
        }
        if (level >= 2)
        {
            _pierce = true;
            pierceLockImg.SetActive(!_pierce);
            pierceimg.enabled = false;
        }
        if (level >= 12)
        {
            _windmill = true;
            windmilLockImg.SetActive(!_windmill);
            windmillimg.enabled = false;
        }
        if (level >= 18)
        {
            _channeling = true;
            channelingLockImg.SetActive(!_channeling);
            channelingimg.enabled = false;
        }
        if (level >= 24)
        {
            _landslash = true;
            landslashLockImg.SetActive(!_landslash);
            landslashimg.enabled = false;
        }
    }

    //���ſ� ���� ��������
    public void UpgradeDamage()
    {
        gold -= Mathf.FloorToInt(30 + attackdmglevel * 30 * 0.2f);
        attackdmglevel += 1;
        attackdmg += Mathf.RoundToInt(attackdmglevel * attackdmg * 0.5f);
        infotxt.ChangeStats();
        systemManager.playergold[0].text = gold.ToString();
    }

    public void UpgradeHealth()
    {
        gold -= Mathf.RoundToInt(30 + hplevel * 30 * 0.2f);
        hplevel += 1;
        maxhp += hplevel * 5;
        infotxt.ChangeStats();
        systemManager.playergold[0].text = gold.ToString();
    }
    #endregion

    /// <summary>
    /// �÷��̾� ��� ���� ����
    /// </summary>
    /// <param name="itemname"></param>
    #region

    public void ChangeWeapon(string itemname)
    {
        foreach (GameObject wp in weapons)
        {
            if (wp.name == itemname)
            {
                wp.SetActive(true);
                wp.transform.position = weaponPos.position;
            }
            else
                wp.SetActive(false);
        }
    }
    public void ChangeArmor(string itemname)
    {
        foreach (GameObject ar in armors)
        {
            if (ar.name == itemname)
            {
                ar.SetActive(true);
                ar.transform.position = armorPos.position;
            }
            else
                ar.SetActive(false);
        }
    }
    #endregion

    /// <summary>
    /// ����� �Ÿ��� "Enemy"�ĺ�
    /// </summary>
    #region
    void SetTarget()
    {
        float distance;
        float x, y, z;

        Array.Resize(ref monsterdistance, monsterlist.Count);
        // ���Ϳ� �÷��̾��� �Ÿ����
        for (int i = 0; i < monsterdistance.Length; i++)
        {
            x = transform.position.x - monsterlist[i].transform.position.x;
            y = transform.position.y - monsterlist[i].transform.position.y;
            z = transform.position.z - monsterlist[i].transform.position.z;
            distance = (Mathf.Pow(x, 2) + Mathf.Pow(y, 2) + Mathf.Pow(z, 2));
            monsterdistance[i] = Math.Sqrt(distance);
            // �÷��̾�� ������ �Ÿ��� ����� ������ ����
            for (int j = 0; j < i; j++)
            {
                //monsterdistance[j]�� ���� monsterdistance[i]�� ������ �۰ų� ���� ��
                if (monsterdistance[j] <= monsterdistance[i])
                {
                    monsterdistance[j] = monsterdistance[j];
                }
                //monsterdistance[j]�� ���� monsterdistance[i+1]�� ������ Ŭ ��
                else if (monsterdistance[j] > monsterdistance[i])
                {
                    double tempvalue = monsterdistance[j];
                    monsterdistance[j] = monsterdistance[i];
                    monsterdistance[i] = tempvalue;

                    Enemy temptransform = monsterlist[j];
                    monsterlist[j] = monsterlist[i];
                    monsterlist[i] = temptransform;
                }
            }
            target = monsterlist[0].transform;
        }
    }

    void Targetting()
    {
        if (monsterlist.Count != 0 && !sensor.Round)
            SetTarget(); //���忡 ���Ͱ� �����ִٸ� ����� ���� ��Ž��

        else if (monsterlist.Count == 0 && sensor.Round && !sensor.move)
            RoundChage(); //���忡 ���Ͱ� �������� �ʴٸ� ���� �̵�
    }
    #endregion

    /// <summary>
    /// �������� �̵�
    /// </summary>
    #region

    void RoundChage()
    {
        //���� ���� ������ �̵�
        if (!sensor.move && countsensor != sensorlist.Length)
        {
            Array.Resize(ref monsterdistance, 0);
            target = sensorlist[countsensor].transform;
        }
    }

    void StageMove()
    {
        if (sensor.move == true) //�������� Ŭ���� �� �̵���
        {
            sensor.move = false;
            nav.enabled = false;
            target = null;
            transform.position = playerpos.position;
            nav.enabled = true;
        }
        else if (stagemove.move == true) //�������� �������� �̵���
        {
            sensor.target = null;
            nav.enabled = false;
            target = null;
            ReSetList();
            stageManager = FindObjectOfType<Stage_Management>();
            playerpos = stageManager.StageSpawner[stagemove.stageNum].transform;
            transform.position = playerpos.position;
            stagemove.move = false;
            nav.enabled = true;
        }
    }
    //�������� �����̵��� �÷��̾� ����Ʈ �ʱ�ȭ
    void ReSetList()
    {
        monsterlist.Clear();
        Array.Resize(ref monsterdistance, 0);
        Array.Resize(ref sensorlist, 0);
    }
    #endregion

    /// <summary>
    /// �÷��̾� ���� ���� �ĺ�
    /// </summary>
    #region
    void AttackSensorRange()
    {
        layerMask = 1 << LayerMask.NameToLayer("Enemy");
        RaycastHit[] rayHits = Physics.SphereCastAll(transform.position, attackRadius, transform.up * 2.5f + transform.forward, attackRange, layerMask); //�÷��̾��� ���� ��Ÿ� ǥ��
        if (rayHits.Length > 0)
        {
            Debug.Log("sense");
            if (target != null)
            {
                liveState = LiveState.Attack;
                nav.isStopped = true;
            }
        }
        else if(rayHits.Length == 0)
        {
            nav.isStopped = false;
            liveState = LiveState.Walk;
            anim.SetBool("Attack", false);
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward);
        Gizmos.DrawWireSphere(transform.position + transform.up * 3.5f + transform.forward * attackRange, attackRadius);
    }
    #endregion

    /// <summary>
    /// �÷��̾� ����
    /// </summary>
    #region
    void AttackAction() //���� ����
    {
        //�ڵ����� Ȱ��ȭ
        if (systemManager.autoPlay)
        {
            if (attackState == AttackState.None || attackState == AttackState.Attack)
            {
                AutoAttack();
            }
        }
        //�ڵ����� ��Ȱ��ȭ
        else if (!systemManager.autoPlay)
        {
            if (nextSkill) //���� ����� ��ų�� �ִٸ�
            {
                OnCallSkill(); //������� ��ų ���
            }
        }
    }

    //��ų ����� ������ �� ��ų��� �켱���� Skill1 -> Skill2 -> Skill3 -> Skill4
    void AutoAttack()
    {
        //��� ��ų�� ��Ÿ���� ���ư��� ���� �� �⺻���� ����
        if (refpierce.Value && refwindmill.Value && refchanneling.Value && reflandslash.Value)
        {
            attackState = AttackState.Attack;
        }
        if(!_pierce && !_windmill && !_channeling && !_landslash) //�������� ��ų�� ������
        {
            attackState = AttackState.Attack;
        }
        //�ٸ� ��ų�� ��������� ���� ��
        //��ų1���� ����� ���� �� ��
        if (!refpierce.Value && _pierce)
        {
            attackState = AttackState.Pierce;
        }
        //��ų1���� ��Ÿ���� �����ְ� 2����ų�� ����� �����ϸ� �ٸ� ��ų�� ��������� ���� ��
        else if (refpierce.Value && !refwindmill.Value && _pierce && _windmill)
        {
            attackState = AttackState.Windmill;
        }
        //��ų 1�� 2���� ��Ÿ���� �����ְ� 3����ų�� ����� �����ϸ� �ٸ� ��ų�� ��������� ���� ��
        else if (refpierce.Value && refwindmill.Value && !refchanneling.Value && _pierce && _windmill && _channeling)
        {
            attackState = AttackState.Channeling;
        }
        //��ų 1�� 2�� 3���� ��Ÿ���� �����ְ� 4�� ��ų�� ����� �����ϸ� �ٸ� ��ų�� ��������� ���� ��
        else if (refpierce.Value && refwindmill.Value && refchanneling.Value && !reflandslash.Value && _pierce && _windmill && _channeling && _landslash)
        {
            attackState = AttackState.Landslash;
        }
    }

    //��ų ����߿� �� ������ ����� ��ų
    void OnCallSkill()
    {
        if (skillNum == 1)
        {
            Pierce();
        }
        else if (skillNum == 2)
        {
            WindMill();
        }
        else if (skillNum == 3)
        {
            Channeling();
        }
        else if (skillNum == 4)
        {
            LandSlash();
        }
    }
    #endregion

    /// <summary>
    /// ���� && ��ų
    /// </summary>
    #region
    IEnumerator BasicAttack() //�⺻����
    {
        skillDmg = attackdmg;
        yield return new WaitForSeconds(2f);

        _attack = false;
    }

    public void Pierce()
    {
        skillNum = 1;
        if (!systemManager.autoPlay)
            nextSkill = true;
        if (_pierce && !refpierce.Value && attackState == AttackState.Pierce)
        {
            pierceimg.enabled = true;
            nextSkill = false;
            skillNum = 0;
            attackState = AttackState.Pierce;
            skillDmg = attackdmg * 2;
            anim.SetTrigger(_Pierce);
            StartCoroutine(CoolDown(pierceimg, 4f, pierce_Cooldown, refpierce));
        }
    }
    public void WindMill()
    {
        skillNum = 2;
        if (!systemManager.autoPlay) //�ڵ������� �ƴҶ� �ٷ� ����
            nextSkill = true;
        if (_windmill && !refwindmill.Value && attackState == AttackState.Windmill)
        {
            windmillimg.enabled = true;
            nextSkill = false;
            skillNum = 0;
            attackState = AttackState.Windmill;
            skillDmg = Mathf.RoundToInt(attackdmg * 0.8f);
            anim.SetTrigger(_WindMill);

            StartCoroutine(CoolDown(windmillimg, 12f, windmill_Cooldown, refwindmill));
        }
    }
    public void Channeling()
    {
        skillNum = 3;
        if (!systemManager.autoPlay) //�ڵ������� �ƴҶ� �ٷ� ����
            nextSkill = true;

        if (_channeling && !refchanneling.Value && attackState == AttackState.Channeling)
        {
            channelingimg.enabled = true;
            nextSkill = false;
            skillNum = 0;
            attackState = AttackState.Channeling;
            anim.SetTrigger(_Channeling);

            StartCoroutine(CoolDown(channelingimg, 15f, channeling_Cooldown, refchanneling));
            attackdmg = attackdmg * 2;
            StartCoroutine(Cou_Channeling());
        }
    }
    IEnumerator Cou_Channeling()
    {
        while (durationtime < 5.0f)
        {
            durationtime += Time.deltaTime;
            yield return null;
            if (durationtime >= 5.0f)
            {
                attackdmg = attackdmg / 2;
                durationtime = 0;
                break;
            }
        }
    }
    public void LandSlash()
    {
        skillNum = 4;
        if (!systemManager.autoPlay)
            nextSkill = true;
        if (_landslash && !reflandslash.Value && attackState == AttackState.Landslash)
        {
            landslashimg.enabled = true;
            nextSkill = false;
            skillNum = 0;
            attackState = AttackState.Landslash;
            skillDmg = attackdmg * 4;
            anim.SetTrigger(_LandSlash);

            StartCoroutine(CoolDown(landslashimg, 15f, landslash_Cooldown, reflandslash));
        }
    }

    //ü����� �нú�
    IEnumerator GenerateHP()
    {
        genTime += Time.deltaTime;
        yield return null;
        if (genTime >= 1f)
        {
            hp += genhp = maxhp * 0.02f; ;
            if(hp > maxhp)
                hp = maxhp;
            genTime = 0.0f;
        }
    }

    public void InstantAttack()
    {
        GameObject attack = attackobj;
        attack.GetComponent<BoxCollider>();
        //���� ũ�⿡ ���� attack������ ����
        if (attackState == AttackState.Attack)
            attack.transform.localScale = new Vector3(equip.Weapon.equipMent.transform.localScale.x, equip.Weapon.equipMent.transform.localScale.y, equip.Weapon.equipMent.transform.localScale.z); ;
        if (attackState == AttackState.Pierce)
            attack.transform.localScale = new Vector3(equip.Weapon.equipMent.transform.localScale.x * 2, equip.Weapon.equipMent.transform.localScale.y * 2, equip.Weapon.equipMent.transform.localScale.z * 2); ;
        if (attackState == AttackState.Windmill)
            attack.transform.localScale = new Vector3(equip.Weapon.equipMent.transform.localScale.x, equip.Weapon.equipMent.transform.localScale.y, equip.Weapon.equipMent.transform.localScale.z); ;
        if(attackState == AttackState.Landslash)
            attack.transform.localScale = new Vector3(equip.Weapon.equipMent.transform.localScale.x * 8, equip.Weapon.equipMent.transform.localScale.y * 8, equip.Weapon.equipMent.transform.localScale.z * 8);
        
        WPDmg = attack.GetComponentInChildren<WeaponDmg>();
        WPDmg.playerDMG = skillDmg;

        Instantiate(attack, weaponPos.position, Quaternion.identity);
    }

    public void IntantiateVFX(int num)
    {
        ParticleSystem action = null;
        if (attackState == AttackState.Attack)
        {
            Vector3 pos = new Vector3(weaponPos.position.x, weaponPos.position.y, weaponPos.position.z - 2);
            switch (num)
            {
                case 1:
                    action = attackEft[0];
                    break;
                case 2:
                    action = attackEft[1];
                    break;
                case 3:
                    action = attackEft[2];
                    break;
            }
            Instantiate(action, pos, Quaternion.identity);
        }
        if (attackState == AttackState.Pierce)
        {
            Vector3 pos = new Vector3(weaponPos.position.x, weaponPos.position.y, weaponPos.position.z -1);
            Instantiate(pierceEft, pos, Quaternion.identity);
        }
        if (attackState == AttackState.Windmill)
        {

            Vector3 pos = new Vector3(weaponPos.position.x, weaponPos.position.y, weaponPos.position.z - 2);
            Instantiate(windmilEft, pos, Quaternion.identity);
        }
        if (attackState == AttackState.Channeling)
        {
            Vector3 pos = new Vector3(transform.position.x, transform.position.y + 8, transform.position.z);
            Instantiate(channelingEft, pos, Quaternion.identity);
        }
        if (attackState == AttackState.Landslash)
        {
            Vector3 pos = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z - 5);
            Instantiate(landslashEft, pos, Quaternion.identity);
        }
    }
    public void AttackStateNone()
    {
        attackState = AttackState.None;
        _attack = false;
        _skill = false;
    }

    /// <summary>
    /// ��ų ��Ÿ��
    /// </summary>
    /// <param name="image"></param>
    /// <param name="cool"></param>
    /// <param name="skill"></param>
    /// <returns></returns>

    IEnumerator CoolDown(Image image, float cool, bool skill, RefBool<bool> rbool)
    {
        rbool.Value = !skill;
        cool = cool + 1;
        while (cool > 1.0f)
        {
            cool -= Time.deltaTime;
            image.fillAmount = (1.0f / cool);
            yield return WfU;
            if (cool <= 1.0f)
            {
                image.enabled = false;
                rbool.Value = false;
                Debug.Log("End");
            }
        }
    }
    #endregion
    /// <summary>
    /// �ǰݽ�
    /// </summary>
    #region
    public void _DecreaseHP(int damage)
    {
        hp += damage;
        GameObject playerText = Instantiate(dmgText);
        playerText.transform.position = dmgPos.position;
        playerText.GetComponent<DamageText>().damage = damage;
    }

    IEnumerator DamageAction()
    {
        foreach (MeshRenderer mesh in meshs)
        {
            mesh.material.color = Color.red;
        }
        yield return new WaitForSeconds(0.2f);

        if (hp > 0)
        {
            foreach (MeshRenderer mesh in meshs)
            {
                mesh.material.color = new Color(1, 1, 1);
            }
        }
        else
        {
            playerState = PlayerState.Die;
            yield return new WaitForSeconds(2f); //��Ȱ ���ð�
            yield return StartCoroutine(FadeOut(2f));

            //��Ȱ
            playerState = PlayerState.Live;
            set = false; //�������� �ʱ�ȭ�� �������� ���� ���·�
            anim.SetBool("Idle", true);
            playerpos = spawner.transform; //���������� ó������ ���ư�
        }
    }

    //����� �ʱ�ȭ
    void StageReSet()
    {
        spawner.clear = true;
        foreach (GameObject sense in sensorlist)
        {
            sensor = sense.GetComponent<Sensor>();
            sensor.curmonster.Clear();
            sensor.GetComponent<BoxCollider>().enabled = false;
        }
        spawner.clear = false;
    }
    #endregion

    /// <summary>
    /// ���ݻ������
    /// </summary>
    #region

    //�ִϸ��̼ǿ��� ���� ���� �� �� ����ҽ�
    public void AnimationSound(int sequence)
    {
        //����ó�� ���ִٸ� false
        audioSource.loop = false;
        switch (sequence)
        {
            case 0:
                audioSource.clip = null;
                break;
            case 1:
                audioSource.clip = audioAttack[0]; //����1
                break;
            case 2:
                audioSource.clip = audioAttack[1]; //����2
                break;
            case 3:
                audioSource.clip = audioAttack[2]; //����3
                break;

            case 4:
                audioSource.clip = audioAttack[0]; //basic����1
                break;
            case 5:
                audioSource.clip = audioAttack[1]; //basic����2
                break;
            case 6:
                audioSource.clip = audioAttack[2]; //basic����3
                break;
            case 7:
                audioSource.clip = audioPierce;
                break;
            case 8:
                audioSource.loop = true;
                audioSource.clip = audioWindmill;
                break;
            case 9:
                audioSource.clip = audioChanneling;
                break;
            case 10:
                audioSource.clip = audioLandslash; //LandSlash
                break;
        }
        if (sequence != 0)
            audioSource.Play();
        else if(sequence == 0)
            audioSource.Stop();
    }
    #endregion
    //�ð� ������
    IEnumerator DelayTime(float delay)
    {
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(delay);

        Time.timeScale = 1f;
    }
    //���̵�ƿ�
    IEnumerator FadeOut(float fadetime)
    {
        yield return null;
    }
}

//��Ÿ�� bool�� ��ȯ�� ���� ���׸� Ŭ����
public class RefBool<T>
{
    private T revalue;
    public T Value
    {
        get { return revalue; }
        set { revalue = value; }
    }
    public RefBool(ref T reference)
    {
        revalue = reference;
    }
}
