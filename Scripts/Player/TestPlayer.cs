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

    //플레이어 상태디자인패턴 및 스탯 관리
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

    //참조 클래스 관리
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

    //플레이어 스탯관리
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
    bool setstats; //스탯 셋팅
    bool set;
    #endregion

    //플레이어 공격 범위
    #region
    public float attackRadius = 2f;
    public float attackRange = 2f;
    #endregion

    //플레이어 장비
    #region
    [SerializeField]
    private Transform weaponPos;
    [SerializeField]
    private Transform armorPos;
    public GameObject[] weapons;
    public GameObject[] armors;
    #endregion

    //공격
    #region
    //생성할 공격 Collider
    public GameObject attackobj;
    GameObject Lock;

    //스킬 Image
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

    int skillNum;//사용하는 스킬 번호
    int skillDmg;
    public float durationtime; //지속시간
    bool _attack;
    bool _skill;

    bool _pierce; //스킬 획득시 true
    bool _windmill;
    bool _channeling;
    bool _landslash;
    bool _generateHP;

    //쿨타임
    bool pierce_Cooldown;
    bool windmill_Cooldown;
    bool channeling_Cooldown;
    bool landslash_Cooldown;

    //다음 사용 스킬선택
    bool nextSkill;
    #endregion

    //피격
    #region
    public GameObject dmgText;
    public Transform dmgPos;
    #endregion

    //사운드
    #region
    AudioSource audioSource;
    public AudioClip[] shout;
    public AudioClip[] audioAttack;
    public AudioClip audioPierce;
    public AudioClip audioWindmill;
    public AudioClip audioChanneling;
    public AudioClip audioLandslash;
    #endregion

    //타겟팅과 이동관리
    #region
    NavMeshAgent nav;
    public Transform target;
    public GameObject[] sensorlist; //라운드 클리어 후 이동할 센서 받기
    public List<Enemy> monsterlist;//플레이하는 라운드의 몬스터배열
    public double[] monsterdistance;
    public Transform playerpos; //플레이어의 현재 스테이지 값 저장
    int layerMask; //인식할 레이어마스크 저장
    #endregion

    //UI
    #region
    public Text leveltxt;
    #endregion

    int countsensor; //몇번째 센서인지 카운트
    //파라미터 최적화
    #region
    static readonly int _Pierce = Animator.StringToHash("Pierce");
    static readonly int _WindMill = Animator.StringToHash("WindMill");
    static readonly int _Channeling = Animator.StringToHash("Channeling");
    static readonly int _LandSlash = Animator.StringToHash("LandSlash");
    #endregion

    //코루틴 최적화
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
            countsensor = 0; //스테이지의 스포너 활성화시 초기화
            Array.Resize(ref sensorlist, spawner.sensorList.Length);
            for (int i = 0; i < spawner.sensorList.Length; i++)
            {
                sensorlist[i] = spawner.sensorList[i];
            }
            sensor = sensorlist[0].GetComponent<Sensor>(); //첫 센서 인식
            target = sensorlist[countsensor].transform; //시작할 라운드의 센서를 target으로 저장
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
    /// 상태디자인패턴
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

            //전투모드가 아닐 때
            if (liveState == LiveState.Walk || liveState == LiveState.Idle)
            {
                attackState = AttackState.None;
            }

            // 전투모드일 때 공격 액션
            if (liveState == LiveState.Attack)
            {
                AttackAction();
                if (!_attack)
                {
                    _attack = !_attack;
                    anim.SetBool("Attack", _attack);
                    StartCoroutine(BasicAttack());
                }

                // 자동 전투모드일 때 스킬사용
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
            set = true; //사망 후 초기화 시작
            target = null;
            anim.SetTrigger("Die"); //사망애니메이션 호출
            liveState = LiveState.Idle; //state 초기화
            attackState = AttackState.None;
            StageReSet();
        }
    }
    #endregion

    /// <summary>
    /// "Player"의 스탯 관리
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

    //구매에 따른 스탯증가
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
    /// 플레이어 장비 착용 관리
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
    /// 가까운 거리의 "Enemy"식별
    /// </summary>
    #region
    void SetTarget()
    {
        float distance;
        float x, y, z;

        Array.Resize(ref monsterdistance, monsterlist.Count);
        // 몬스터와 플레이어의 거리계산
        for (int i = 0; i < monsterdistance.Length; i++)
        {
            x = transform.position.x - monsterlist[i].transform.position.x;
            y = transform.position.y - monsterlist[i].transform.position.y;
            z = transform.position.z - monsterlist[i].transform.position.z;
            distance = (Mathf.Pow(x, 2) + Mathf.Pow(y, 2) + Mathf.Pow(z, 2));
            monsterdistance[i] = Math.Sqrt(distance);
            // 플레이어와 몬스터의 거리가 가까운 순으로 정렬
            for (int j = 0; j < i; j++)
            {
                //monsterdistance[j]의 값이 monsterdistance[i]의 값보다 작거나 같을 때
                if (monsterdistance[j] <= monsterdistance[i])
                {
                    monsterdistance[j] = monsterdistance[j];
                }
                //monsterdistance[j]의 값이 monsterdistance[i+1]의 값보다 클 때
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
            SetTarget(); //라운드에 몬스터가 남아있다면 가까운 몬스터 재탐색

        else if (monsterlist.Count == 0 && sensor.Round && !sensor.move)
            RoundChage(); //라운드에 몬스터가 남아있지 않다면 라운드 이동
    }
    #endregion

    /// <summary>
    /// 스테이지 이동
    /// </summary>
    #region

    void RoundChage()
    {
        //다음 라운드 센서로 이동
        if (!sensor.move && countsensor != sensorlist.Length)
        {
            Array.Resize(ref monsterdistance, 0);
            target = sensorlist[countsensor].transform;
        }
    }

    void StageMove()
    {
        if (sensor.move == true) //스테이지 클리어 후 이동시
        {
            sensor.move = false;
            nav.enabled = false;
            target = null;
            transform.position = playerpos.position;
            nav.enabled = true;
        }
        else if (stagemove.move == true) //스테이지 선택으로 이동시
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
    //스테이지 선택이동시 플레이어 리스트 초기화
    void ReSetList()
    {
        monsterlist.Clear();
        Array.Resize(ref monsterdistance, 0);
        Array.Resize(ref sensorlist, 0);
    }
    #endregion

    /// <summary>
    /// 플레이어 공격 범위 식별
    /// </summary>
    #region
    void AttackSensorRange()
    {
        layerMask = 1 << LayerMask.NameToLayer("Enemy");
        RaycastHit[] rayHits = Physics.SphereCastAll(transform.position, attackRadius, transform.up * 2.5f + transform.forward, attackRange, layerMask); //플레이어의 공격 사거리 표시
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
    /// 플레이어 전투
    /// </summary>
    #region
    void AttackAction() //전투 관리
    {
        //자동전투 활성화
        if (systemManager.autoPlay)
        {
            if (attackState == AttackState.None || attackState == AttackState.Attack)
            {
                AutoAttack();
            }
        }
        //자동전투 비활성화
        else if (!systemManager.autoPlay)
        {
            if (nextSkill) //다음 사용할 스킬이 있다면
            {
                OnCallSkill(); //대기중인 스킬 사용
            }
        }
    }

    //스킬 사용이 가능할 때 스킬사용 우선순위 Skill1 -> Skill2 -> Skill3 -> Skill4
    void AutoAttack()
    {
        //모든 스킬의 쿨타임이 돌아가고 있을 때 기본공격 실행
        if (refpierce.Value && refwindmill.Value && refchanneling.Value && reflandslash.Value)
        {
            attackState = AttackState.Attack;
        }
        if(!_pierce && !_windmill && !_channeling && !_landslash) //보유중인 스킬이 없을때
        {
            attackState = AttackState.Attack;
        }
        //다른 스킬을 사용중이지 않을 때
        //스킬1번의 사용이 가능 할 때
        if (!refpierce.Value && _pierce)
        {
            attackState = AttackState.Pierce;
        }
        //스킬1번의 쿨타임이 돌고있고 2번스킬의 사용이 가능하며 다른 스킬을 사용중이지 않을 때
        else if (refpierce.Value && !refwindmill.Value && _pierce && _windmill)
        {
            attackState = AttackState.Windmill;
        }
        //스킬 1번 2번의 쿨타임이 돌고있고 3번스킬의 사용이 가능하며 다른 스킬을 사용중이지 않을 때
        else if (refpierce.Value && refwindmill.Value && !refchanneling.Value && _pierce && _windmill && _channeling)
        {
            attackState = AttackState.Channeling;
        }
        //스킬 1번 2번 3번의 쿨타임이 돌고있고 4번 스킬의 사용이 가능하며 다른 스킬을 사용중이지 않을 때
        else if (refpierce.Value && refwindmill.Value && refchanneling.Value && !reflandslash.Value && _pierce && _windmill && _channeling && _landslash)
        {
            attackState = AttackState.Landslash;
        }
    }

    //스킬 사용중에 고른 다음에 사용할 스킬
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
    /// 공격 && 스킬
    /// </summary>
    #region
    IEnumerator BasicAttack() //기본공격
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
        if (!systemManager.autoPlay) //자동전투가 아닐때 바로 실행
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
        if (!systemManager.autoPlay) //자동전투가 아닐때 바로 실행
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

    //체력재생 패시브
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
        //무기 크기에 따라 attack사이즈 변경
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
    /// 스킬 쿨타임
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
    /// 피격시
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
            yield return new WaitForSeconds(2f); //부활 대기시간
            yield return StartCoroutine(FadeOut(2f));

            //부활
            playerState = PlayerState.Live;
            set = false; //스테이지 초기화가 끝났으니 원래 상태로
            anim.SetBool("Idle", true);
            playerpos = spawner.transform; //스테이지의 처음으로 돌아감
        }
    }

    //사망시 초기화
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
    /// 공격사운드관리
    /// </summary>
    #region

    //애니메이션에서 사운드 송출 해 줄 사운드소스
    public void AnimationSound(int sequence)
    {
        //루프처리 돼있다면 false
        audioSource.loop = false;
        switch (sequence)
        {
            case 0:
                audioSource.clip = null;
                break;
            case 1:
                audioSource.clip = audioAttack[0]; //기합1
                break;
            case 2:
                audioSource.clip = audioAttack[1]; //기합2
                break;
            case 3:
                audioSource.clip = audioAttack[2]; //기합3
                break;

            case 4:
                audioSource.clip = audioAttack[0]; //basic공격1
                break;
            case 5:
                audioSource.clip = audioAttack[1]; //basic공격2
                break;
            case 6:
                audioSource.clip = audioAttack[2]; //basic공격3
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
    //시간 딜레이
    IEnumerator DelayTime(float delay)
    {
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(delay);

        Time.timeScale = 1f;
    }
    //페이드아웃
    IEnumerator FadeOut(float fadetime)
    {
        yield return null;
    }
}

//쿨타임 bool값 반환을 위한 제네릭 클래스
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
