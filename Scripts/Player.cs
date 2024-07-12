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
    public int maxHealth = 500; //�ִ�ü��
    public int curHealth; //����ü��
    public int PlayerDamage = 20;
    public GameObject weapon;
    public GameObject dmgText; //������ �ؽ�Ʈ
    public Transform dmgPos; //�ǰݽ� ������ ǥ�� ��ġ
    public bool isChase;
    public Vector3 respawnPosition = new Vector3(0, 1, 17); // ������ ��ġ

    // ��� Monster�� Transform�� ������ �迭
    public Transform[] monsters;

    // Ÿ������ ������ Monster�� Transform
    public Transform targetMonster;

    // Player�� �̵� �ӵ�
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

    //��ų
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

        // ��� Monster�� Transform�� monsters �迭�� ����
        monsters = GameObject.FindGameObjectsWithTag("Monster").Select(x => x.transform).ToArray();

        // monsters �迭�� Monster�� �ִ��� Ȯ��
        if (monsters.Length == 0)
        {
            Debug.Log("Monster�� �����ϴ�.");
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

        // ���� ������ �ִ� Monster�� Ÿ������ ����
        targetMonster = GetNearestMonster();

        // Ÿ������ ������ Monster���� �̵�
        MoveTowardsTarget();

    }

    void FreezeVelocity() //�������� NavAgent �̵� �������� �ʴ� ����
    {
        if (isChase)
        {
            rigid.velocity = Vector3.zero;
            rigid.angularVelocity = Vector3.zero;
        }
    }
    

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Monster")) //���� ���� �ȿ� ���Ͱ� �νĵ��� ��
        {

        }
        else if (playerstate != State.MJ && other.CompareTag("MonsterAttack")) //���� ������ ��
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
    /// ����
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
    /// �ǰ�
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
    /// ������ 
    /// </summary>
    private void Respawn()
    {
        // �÷��̾� ĳ������ Ʈ�������� ������
        Transform playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        // �÷��̾� ĳ���͸� ������ ��ġ�� �̵���Ŵ
        playerTransform.position = respawnPosition;

        Start();
    }


    // ���� ������ �ִ� Monster�� ��ȯ�ϴ� �Լ�
    Transform GetNearestMonster()
    {
        Transform nearestMonster = null;
        float nearestDistance = float.MaxValue;

        // ��� Monster�� Transform�� �ݺ��ϸ鼭 Player���� �Ÿ��� ���
        foreach (Transform monster in monsters)
        {
            // Monster�� Transform�� null�̸� skip
            if (monster == null)
            {
                continue;
            }

            float distance = Vector3.Distance(transform.position, monster.position);

            // Player�� ���� ª�� �Ÿ��� ���� Monster�� Ÿ������ ����
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestMonster = monster;
            }
        }

        return nearestMonster;
    }

    // Ÿ������ ������ Monster���� �̵��ϴ� �Լ�
    void MoveTowardsTarget()
    {
        if (targetMonster == null)
        {
            return;
        }

        // ���� ����� ���� ã��
        Transform nearestMonster = GetNearestMonster();

        if (nearestMonster == null)
        {
            return;
        }

        // Ÿ������ ������ Monster�� ������ ���
        Vector3 direction = nearestMonster.position - transform.position;

        // Ÿ������ ������ Monster�� ������ �ݴ��
        direction = -direction;

        // �̵� �ӵ��� �������� �̵��մϴ�.
        transform.Translate(direction.normalized * moveSpeed * Time.deltaTime);

        // Monster���� �Ÿ� ���
        float distance = Vector3.Distance(transform.position, nearestMonster.position);

        // Monster���� �Ÿ��� ���� �ȿ� ������ ����
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
    /// ���
    /// </summary>
    void Die()
    {
        anim.SetTrigger("doDie");
    }

    // ��Ÿ��_���彽����
    private IEnumerator LS_Cooldown()
    {
        Debug.Log("LS_cooltime");
        yield return WFS10;

        // ��Ÿ�� ������ ��ų ��� 
        LS_CoolingDown = true;
        Debug.Log("LS_cooltime_end");
    }

    // ��Ÿ��_������
    private IEnumerator P_Cooldown()
    {
        Debug.Log("P_cooltime");
        yield return WFS10;

        // ��Ÿ�� ������ ��ų ��� 
        P_CoolingDown = true;
        Debug.Log("P_cooltime_end");
    }

    // ��Ÿ��_�����
    private IEnumerator WM_Cooldown()
    {
        Debug.Log("W_cooltime");
        yield return WFS10;

        // ��Ÿ�� ������ ��ų ��� 
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

            // ���� ����� ����
            Transform nearestMonster = GetNearestMonster();

            // Monster���� �Ÿ� ���
            float distance = Vector3.Distance(transform.position, nearestMonster.position);

            // ���Ϳ��� �Ÿ��� ���� ���� �ȿ� ������ ��ų ���
            if (distance <= targetRadius)
            {
                // ��ų �ִϸ��̼� ���
                
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

            // ���� ����� ����
            Transform nearestMonster = GetNearestMonster();

            // Monster���� �Ÿ� ���
            float distance = Vector3.Distance(transform.position, nearestMonster.position);

            // ���Ϳ��� �Ÿ��� ���� ���� �ȿ� ������ ��ų ���
            if (distance <= targetRadius)
            {
                // ��ų �ִϸ��̼� ���
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

            // ���� ����� ����
            Transform nearestMonster = GetNearestMonster();

            // Monster���� �Ÿ� ���
            float distance = Vector3.Distance(transform.position, nearestMonster.position);

            // ���Ϳ��� �Ÿ��� ���� ���� �ȿ� ������ ��ų ���
            if (distance <= targetRadius)
            {
                // ��ų �ִϸ��̼� ���
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
