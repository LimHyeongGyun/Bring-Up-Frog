using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Xml;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public int maxHealth = 500; //최대체력
    public int curHealth; //현재체력
    public int PlayerDamage = 20;
    public GameObject weapon;
    public GameObject dmgText; //데미지 텍스트
    public Transform dmgPos; //피격시 데미지 표기 위치
    public bool isChase;
    public Vector3 respawnPosition = new Vector3(0, 1, 17); // 리스폰 위치

    // 모든 Monster의 Transform을 저장할 배열
    public Transform[] monsters;

    // 타겟으로 지정된 Monster의 Transform
    public Transform targetMonster;

    // Player의 이동 속도
    public float moveSpeed = 5.0f;


    public float targetRadius = 2.0f;
    float targetRange = 2.5f;
    bool pos;

    Rigidbody rigid;
    CapsuleCollider capCollider;
    NavMeshAgent nav;
    Animator anim;
    Material mat;

    Skill skill;
    AudioManager audioManager;

    WaitForSeconds WFS01 = new(0.1f);
    WaitForSeconds WFS1 = new(1.0f);
    WaitForSeconds WFS2 = new(2.0f);
    WaitForSeconds WFS10 = new(10.0f);

    public enum State { Idle, walk, attack, Die, MJ }
    public State playerstate;

    //스킬
    public float skill_cool = 1f;
    private bool LS_CoolingDown = true;
    private bool P_CoolingDown = true;
    private bool WM_CoolingDown = true;
    public GameObject landslash;
    public GameObject pierce;
    public GameObject windmill;

    public float StartTime { get; set; }

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        capCollider = GetComponent<CapsuleCollider>();
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        mat = GetComponentInChildren<SkinnedMeshRenderer>().material;
    }

    void Start()
    {
        curHealth = maxHealth;
        isChase = true;
        anim.SetBool("isWalk", true);
        nav.enabled = true;

        // 모든 Monster의 Transform을 monsters 배열에 저장
        monsters = GameObject.FindGameObjectsWithTag("Monster").Select(x => x.transform).ToArray();

        // monsters 배열에 Monster가 있는지 확인
        if (monsters.Length == 0)
        {
            Debug.Log("Monster가 없습니다.");
        }
    }
    void Update()
    {
        /*
        if (isChase)
        {
            nav.SetDestination(targetMonster.position);
        }*/

        if (Timer.game_time < 0.2f)
        {
            Die();
            curHealth = 0;
            Invoke("Respawn", 3.0f);
        }

        // 가장 가까이 있는 Monster를 타겟으로 지정
        targetMonster = GetNearestMonster();

        // 타겟으로 지정된 Monster에게 이동
        MoveTowardsTarget();

    }

    void FreezeVelocity() //물리력이 NavAgent 이동 방해하지 않는 로직
    {
        if (isChase)
        {
            rigid.velocity = Vector3.zero;
            rigid.angularVelocity = Vector3.zero;
        }
    }
    

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Monster")) //일정 범위 안에 몬스터가 인식됐을 때
        {

        }
        else if (playerstate != State.MJ && other.CompareTag("MonsterAttack")) //공격 당했을 때
        {
            StartCoroutine(DamageAction());
        }
    }

    void FixedUpdate()
    {
        FreezeVelocity();
        //Targetting();
    }

    /*void P_Attack()
    {
        AudioManager.instance.Attack(AudioManager.Atk.Attack, false);
        StartCoroutine(Attack());
    }*/
    /// <summary>
    /// 공격
    /// </summary>
    /// <returns></returns>
    IEnumerator Attack()
    {
        pos = true;
        anim.SetBool("isAttack", true);
        AudioManager.instance.Attack(AudioManager.Atk.Attack, false);
        playerstate = State.attack;
        yield return WFS1;
    }

    IEnumerator NoAttack()
    {
        anim.SetBool("isAttack", false);
        playerstate = State.walk;
        nav.isStopped = false;
        pos = false;
        yield return WFS1;
    }

    /// <summary>
    /// 피격
    /// </summary>
    /// <returns></returns>
    IEnumerator DamageAction()
    {
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Shot);
        curHealth -= 10; 
        GameObject playerText = Instantiate(dmgText);
        playerText.transform.position = dmgPos.position;
        playerText.GetComponent<DamageText>().damage = 20; 
        mat.color = Color.red;
        yield return WFS01;

        if (curHealth > 0)
        {
            playerstate = State.walk;
            mat.color = new Color(1, 1, 1);
        }

        else
        {
            playerstate = State.Die;
            mat.color = Color.gray;
            nav.enabled = false;
            gameObject.layer = LayerMask.NameToLayer("Die");
            anim.SetTrigger("doDie");
            Destroy(gameObject, 1f);
        }
    }

    /// <summary>
    /// 리스폰 
    /// </summary>
    private void Respawn()
    {
        // 플레이어 캐릭터의 트랜스폼을 가져옴
        Transform playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        // 플레이어 캐릭터를 리스폰 위치로 이동시킴
        playerTransform.position = respawnPosition;

        Start();
    }


    // 가장 가까이 있는 Monster를 반환하는 함수
    Transform GetNearestMonster()
    {
        Transform nearestMonster = null;
        float nearestDistance = float.MaxValue;

        // 모든 Monster의 Transform을 반복하면서 Player와의 거리를 계산
        foreach (Transform monster in monsters)
        {
            // Monster의 Transform이 null이면 skip
            if (monster == null)
            {
                continue;
            }

            float distance = Vector3.Distance(transform.position, monster.position);

            // Player와 가장 짧은 거리를 갖는 Monster를 타겟으로 지정
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestMonster = monster;
            }
        }

        return nearestMonster;
    }

    // 타겟으로 지정된 Monster에게 이동하는 함수
    void MoveTowardsTarget()
    {
        if (targetMonster == null)
        {
            return;
        }

        // 가장 가까운 몬스터 찾기
        Transform nearestMonster = GetNearestMonster();

        if (nearestMonster == null)
        {
            return;
        }

        // 타겟으로 지정된 Monster의 방향을 계산
        Vector3 direction = nearestMonster.position - transform.position;

        // 타겟으로 지정된 Monster의 방향을 반대로
        direction = -direction;

        // 이동 속도와 방향으로 이동합니다.
        transform.Translate(direction.normalized * moveSpeed * Time.deltaTime);

        // Monster와의 거리 계산
        float distance = Vector3.Distance(transform.position, nearestMonster.position);

        // Monster와의 거리가 범위 안에 들어오면 공격
        if (distance <= targetRadius)
        {
            StartCoroutine(Attack());
            //P_Attack();
            
        }

        else if(distance > targetRadius)
        {
            StartCoroutine(NoAttack());
        }
    }

    /// <summary>
    /// 사망
    /// </summary>
    void Die()
    {
        anim.SetTrigger("doDie");
    }

    // 쿨타임_랜드슬래쉬
    private IEnumerator LS_Cooldown()
    {
        Debug.Log("LS_cooltime");
        yield return WFS10;

        // 쿨타임 지나면 스킬 사용 
        LS_CoolingDown = true;
        Debug.Log("LS_cooltime_end");
    }

    // 쿨타임_지르기
    private IEnumerator P_Cooldown()
    {
        Debug.Log("P_cooltime");
        yield return WFS10;

        // 쿨타임 지나면 스킬 사용 
        P_CoolingDown = true;
        Debug.Log("P_cooltime_end");
    }

    // 쿨타임_윈드밀
    private IEnumerator WM_Cooldown()
    {
        Debug.Log("W_cooltime");
        yield return WFS10;

        // 쿨타임 지나면 스킬 사용 
        WM_CoolingDown = true;
        Debug.Log("W_cooltime_end");
    }

    public void LandSlash()
    {
        if (LS_CoolingDown)
        {
            LS_CoolingDown = false;

            //StartCoroutine(CoolDown(3f, LS_CoolingDown));
            StopCoroutine(Attack());
            StartCoroutine(LS_Cooldown());

            // 가장 가까운 몬스터
            Transform nearestMonster = GetNearestMonster();

            // Monster와의 거리 계산
            float distance = Vector3.Distance(transform.position, nearestMonster.position);

            // 몬스터와의 거리가 공격 범위 안에 들어오면 스킬 사용
            if (distance <= targetRadius)
            {
                // 스킬 애니메이션 재생
                
                anim.SetTrigger("LandSlash");
                anim.SetBool("isAttack", true);
            }

            weapon = Instantiate(landslash, weapon.transform.position, Quaternion.identity);
            skill = landslash.GetComponent<Skill>();
            skill.Skill_Damage = PlayerDamage * 2;
        }
    }
    
    public void Pierce()
    {
        if(P_CoolingDown) 
        {
            
            P_CoolingDown = false;
            StartCoroutine(P_Cooldown());
            
            AudioManager.instance.PlaySfx(AudioManager.Sfx.KiHap);

            // 가장 가까운 몬스터
            Transform nearestMonster = GetNearestMonster();

            // Monster와의 거리 계산
            float distance = Vector3.Distance(transform.position, nearestMonster.position);

            // 몬스터와의 거리가 공격 범위 안에 들어오면 스킬 사용
            if (distance <= targetRadius)
            {
                // 스킬 애니메이션 재생
                anim.SetTrigger("Pierce");
                anim.SetBool("isAttack", true);
            }
            //yield return WFS1;
            weapon = Instantiate(pierce, weapon.transform.position, Quaternion.identity);
            skill = pierce.GetComponent<Skill>();
            skill.Skill_Damage = PlayerDamage * 4;
        }
    }

    public void Windmill()
    {
        if (WM_CoolingDown)
        {
            WM_CoolingDown = false;
            StartCoroutine(WM_Cooldown());
            
            AudioManager.instance.PlaySfx(AudioManager.Sfx.Windmill);

            // 가장 가까운 몬스터
            Transform nearestMonster = GetNearestMonster();

            // Monster와의 거리 계산
            float distance = Vector3.Distance(transform.position, nearestMonster.position);

            // 몬스터와의 거리가 공격 범위 안에 들어오면 스킬 사용
            if (distance <= targetRadius)
            {
                // 스킬 애니메이션 재생
                anim.SetTrigger("Windmill");
                anim.SetBool("isAttack", true);
            }
            //yield return WFS1;
            weapon = Instantiate(windmill, weapon.transform.position, Quaternion.identity);
            skill = windmill.GetComponent<Skill>();
            skill.Skill_Damage = PlayerDamage + PlayerDamage / 2;
        }
    }

    IEnumerator StopSound()
    {
        AudioManager.instance.Attack(AudioManager.Atk.Attack, true);
        yield return WFS10;

    }
}
