using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using static UnityEngine.EventSystems.EventTrigger;

public class Enemy : MonoBehaviour
{
    public EnemyType enemyType;
    public EnemyState enemystate;

    public EnemyStats enemystats = new EnemyStats();
    public EnemyStats EnemyStats
    {
        get { return enemystats; }
        set { enemystats = value; }
    }
    public Spawner spawner;
    TestPlayer player;
    EnemyWeapon EWDmg;
    Sensor sensor;
    public StageMove stagemove;

    public int sensorpos;
    public int stagenum;
    public int hp;
    public int damage;

    public GameObject oneself;
    public GameObject attackobj;
    public GameObject sense;
    public GameObject dmgText;
    public Transform target;
    public Transform dmgPos; //피격시 데미지 표기 위치
    public Transform weaponPos;

    bool possible;
    float targetRadius;
    float targetRange;

    Material mat;
    Animator anim;
    Rigidbody rigid;
    NavMeshAgent nav;

    new WaitForSeconds WFS01 = new(0.1f);
    new WaitForSeconds WFS1 = new(1f);
    new WaitForSeconds WFS2 = new(2f);

    private void Awake()
    {
        player = FindObjectOfType<TestPlayer>();
        sensor = sense.GetComponent<Sensor>();
        stagemove = player.stagemove;

        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody>();
        mat = GetComponentInChildren<SkinnedMeshRenderer>().material;
        nav = GetComponent<NavMeshAgent>();
    }
    void Start()
    {
        ChangeStats();
        enemystate = EnemyState.Walk;
    }
    
    void Update()
    {
        if (enemystate != EnemyState.Die && target != null)
        {
            nav.enabled = true;
            nav.SetDestination(target.position);
            if(!nav.isStopped)
                transform.LookAt(target.position); //추격시 목표물을 바라보게 함
            Targetting();
        }

        if (enemystate == EnemyState.Idle)
        {
            anim.SetBool("Walk", true);
        }
        if (enemystate == EnemyState.Walk)
        {
            anim.SetBool("Walk", true);
        }
        if (enemystate == EnemyState.Attack)
        {
            if (!possible && enemystate != EnemyState.Unbeatable)
            {
                StartCoroutine(Attack());
            }
        }
        if (player.playerState == PlayerState.Die) //플레이어 사망시
        {
            Destroy(oneself); //오브젝트 삭제
        }
        if (spawner.clear)//맵 이동시 오브젝트 삭제
        {
            Destroy(oneself);
        }
        if (enemystate != EnemyState.Die && sensor.target.CompareTag("Player"))
            target = sensor.target.transform;
    }

    void Targetting()
    {
        switch (enemyType)
        {
            case EnemyType.Melee:
                targetRange = 2f;
                targetRadius = 1.5f;
                break;
            case EnemyType.Range:
                targetRange = 2f;
                targetRadius = 4f;
                break;
        }
        //공격 사거리인식
        RaycastHit[] rayHits = Physics.SphereCastAll(transform.position, targetRadius, transform.forward, targetRange, LayerMask.GetMask("Player"));

        if(rayHits.Length > 0 && !possible && enemystate != EnemyState.Die)
        {
            nav.isStopped = true;
            enemystate = EnemyState.Attack;
        }
    }

    /// <summary>
    /// 공격
    /// </summary>
    /// <returns></returns>
    IEnumerator Attack()
    {
        possible = true;
        anim.SetTrigger("Attack");
        
        yield return WFS2;

        anim.SetBool("Walk", true);
        enemystate = EnemyState.Walk;
        nav.isStopped = false;
        possible = false;
    }

    public void _DecreaseHP(int damage)
    {
        hp += damage;
        GameObject enemyText = Instantiate(dmgText);
        enemyText.transform.position = dmgPos.position;
        enemyText.GetComponent<DamageText>().damage = damage;
        StartCoroutine(DamageAction());
    }

    public void InstantAttack()
    {
        GameObject attack = Instantiate(attackobj, weaponPos.position, Quaternion.identity);
        attack.GetComponent<BoxCollider>();
        attack.transform.localScale = new Vector3(gameObject.transform.localScale.x*2, gameObject.transform.localScale.y * 2, gameObject.transform.localScale.z * 2);
        EWDmg = attack.GetComponentInChildren<EnemyWeapon>();
        EWDmg.enemyDMG = damage;
    }
    /// <summary>
    /// 피격시
    /// </summary>
    /// <returns></returns>
    IEnumerator DamageAction()
    {
        mat.color = Color.red;

        if (hp > 0)
        {
            enemystate = EnemyState.Unbeatable;
            yield return WFS01;

            enemystate = EnemyState.Walk;
            mat.color = new Color(1, 1, 1);
        }

        else
        {
            Debug.Log("죽었다");
            enemystate = EnemyState.Die;
            gameObject.layer = LayerMask.NameToLayer("Die");
            target = null;
            nav.enabled = false;
            anim.SetTrigger("Die");
            mat.color = Color.gray;
            //yield return WFS1;

            sensor.curmonster.Remove(gameObject.GetComponent<Enemy>()); //자신이 리스트의 몇번째 위치인지 기억한 후 그 위치에서 지워지도록 해야함
            player.monsterlist.Remove(gameObject.GetComponent<Enemy>());
            sensor.ClearCheck();
            player.target = null;
            Destroy(oneself);
        }
    }

    void ChangeStats()
    {
        if (EnemyStats.name == "Fish")
        {
            hp = enemystats.maxHp + (stagenum * 50);
            damage = enemystats.damage + (stagenum * 3);
        }
        else if (enemystats.name == "Snake")
        {
            hp = enemystats.maxHp + (stagenum * 75);
            damage = enemystats.damage + (stagenum * 5);
        }
        else if (enemystats.name == "Squid")
        {
            hp = enemystats.maxHp + (stagenum * 75);
            damage = enemystats.damage + (stagenum * 4);
        }
        else if (enemystats.name == "Bird")
        {
            hp = enemystats.maxHp + (stagenum * 100);
            damage = enemystats.damage + (stagenum * 6);
        }
        else if (enemystats.name == "Rats")
        {
            hp = enemystats.maxHp + (stagenum * 100);
            damage = enemystats.damage + (stagenum * 5);
        }
        else if (enemystats.name == "Rizard")
        {
            hp = enemystats.maxHp + (stagenum * 150);
            damage = enemystats.damage + (stagenum * 72);
        }
        else if (enemystats.name == "Monkey")
        {
            hp = enemystats.maxHp + (stagenum * 150);
            damage = enemystats.damage + (stagenum * 6);
        }
        else if (enemystats.name == "Deer")
        {
            hp = enemystats.maxHp + (stagenum * 200);
            damage = enemystats.damage + (stagenum * 8);
        }
        else if (enemystats.name == "StingRay")
        {
            hp = enemystats.maxHp + (stagenum * 200);
            damage = enemystats.damage + (stagenum * 7);
        }
        else if (enemystats.name == "Bat")
        {
            hp = enemystats.maxHp + (stagenum * 250);
            damage = enemystats.damage + (stagenum * 8);
        }
        else if (enemystats.name == "Salamander")
        {
            hp = enemystats.maxHp + (stagenum * 300);
            damage = enemystats.damage + (stagenum * 10);
        }
        else if (enemystats.name == "DemonKing")
        {
            hp = enemystats.maxHp + (stagenum * 400);
            damage = enemystats.damage + (stagenum * 12);
        }
        else if (enemystats.name == "Skeleton")
        {
            hp = enemystats.maxHp + (stagenum * 300);
            damage = enemystats.damage + (stagenum * 9);
        }
        else if (enemystats.name == "Turtle")
        {
            hp = enemystats.maxHp + (stagenum * 350);
            damage = enemystats.damage + (stagenum * 10);
        }
        else if (enemystats.name == "Golem")
        {
            hp = enemystats.maxHp + (stagenum * 500);
            damage = enemystats.damage + (stagenum * 15);
        }
        else if (enemystats.name == "Dragon")
        {
            hp = enemystats.maxHp + (stagenum * 600);
            damage = enemystats.damage + (stagenum * 20);
        }
        else if (enemystats.name == "Slime")
        {
            hp = enemystats.maxHp + (stagenum * 400);
            damage = enemystats.damage + (stagenum * 11);
        }
        else if (enemystats.name == "Plant")
        {
            hp = enemystats.maxHp + (stagenum * 450);
            damage = enemystats.damage + (stagenum * 12);
        }
        else if (enemystats.name == "Spider")
        {
            hp = enemystats.maxHp + (stagenum * 700);
            damage = enemystats.damage + (stagenum * 25);
        }
        else if (enemystats.name == "EvilMage")
        {
            hp = enemystats.maxHp + (stagenum * 800);
            damage = enemystats.damage + (stagenum * 30);
        }
    }
}
