using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using Random = UnityEngine.Random;

public class Sensor : MonoBehaviour
{
    CameraController cameracontroller;
    Spawner spawner;
    System_Manager systemManager;
    Stage_Management stageManager;
    TestPlayer player;

    public Transform curRound; //�÷��̶��� �� ���� ����
    public GameObject target; //���Ͱ� �����ϴ� Ÿ��
    public GameObject[] monsterList; //�����ϴ� ���� ����
    public List<Enemy> curmonster; //���� ������ ����
    public bool Round;
    public bool move;
    int themanum;
    int themaexp;
    int stageexp;
    int clearExp;

    private void Awake()
    {
        player = FindObjectOfType<TestPlayer>();
        spawner = GetComponentInParent<Spawner>();
        systemManager = FindObjectOfType<System_Manager>();
        stageManager = FindObjectOfType<Stage_Management>();
        cameracontroller = FindObjectOfType<CameraController>();
    }
    private void Start()
    {
        Round = true;
        curRound = gameObject.transform;
        target = gameObject;
    }

    private void Update()
    {
        if (player.playerState == PlayerState.Die) //�÷��̾� �����
        {
            curmonster.Clear();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) //���� ���� �ȿ� �÷��̾ �νĵ��� ��
        {
            Debug.Log("sensor in");
            gameObject.GetComponent<BoxCollider>().enabled = false; //������ ������ ���۵����� �ʵ���
            cameracontroller.sensor = gameObject.transform;
            cameracontroller.fixCamera = true; //�ش� ���忡 ī�޶� ����
            player.sensor = GetComponent<Sensor>();
            target = other.gameObject;
            System_Manager.roundCount++; //���� ī��Ʈ
        }
    }

    IEnumerator Fadeout()
    {
        Debug.Log("�÷��̾� ��ġ�� ���� ���������� �̵���ŵ�ϴ�");
        yield return new WaitForSeconds(1f);
    }

    //�������� Ŭ����� ��������
    void PayingCompensation()
    {
        player.gold += Random.Range(1, 100) + (10 * stageManager.themanum) + (spawner.stageArraynum * Random.Range(1, 50));
        ClearExp();
        player.exp += clearExp;
        player.UpdateLevel();
    }
    void ClearExp()
    {
        themanum = SceneManager.GetActiveScene().buildIndex;
        if (themanum == 0) themaexp = 0;
        else if (themanum == 1) themaexp = 0;
        else if (themanum == 2) themaexp = 0;
        else if (themanum == 3) themaexp = 0;
        else if (themanum == 4) themaexp = 0;

        if (spawner.stageArraynum == 0) stageexp = 0;
        else if (spawner.stageArraynum == 1) stageexp = 0;
        else if (spawner.stageArraynum == 2) stageexp = 0;
        else if (spawner.stageArraynum == 3) stageexp = 0;
        else if (spawner.stageArraynum == 4) stageexp = 0;
        else if (spawner.stageArraynum == 5) stageexp = 0;
        else if (spawner.stageArraynum == 6) stageexp = 0;
        else if (spawner.stageArraynum == 7) stageexp = 0;
        else if (spawner.stageArraynum == 8) stageexp = 0;
        else if (spawner.stageArraynum == 9) stageexp = 0;

        clearExp = 10 + (20 * themanum) + (2 * spawner.stageArraynum + 1);
    }
    public void ClearCheck()
    {
        if (curmonster.Count == 0)
        {
            if (!Round && !spawner.clear)
            {
                Clear();
            }
        }
    }
    void Clear()
    {
        cameracontroller.fixCamera = false;
        Debug.Log("RoundClear");
        target = null;
        Round = true; //Ŭ���� ������ !Round�� false�� ����
        if (spawner.sensorList[spawner.sensorList.Length - 1] == gameObject) //������ �������� �Ǵ�
        {
            //������ ���忴�ٸ� �������� Ŭ����
            Debug.Log("StageClear");
            PayingCompensation();
            StartCoroutine(Fadeout());
            spawner.clear = true;
            spawner.clear = false;
            System_Manager.roundCount = 0;
            if (systemManager.regame) //�������� �ݺ� ��ư�� �����ٸ�
            {
                //�ش� �������� �ٽý���
                player.playerpos = stageManager.StageSpawner[spawner.stageArraynum].transform;
            }
            else if (player.playerpos.position != stageManager.StageSpawner[stageManager.StageSpawner.Count - 1].transform.position && !systemManager.regame) //������ ���������� �ƴϰ� �������� �ݺ� ��ư�� �������ִٸ�
            {
                player.playerpos = stageManager.StageSpawner[spawner.stageArraynum + 1].transform; //���� �������� �̵�
            }
            else if (player.playerpos.position == stageManager.StageSpawner[stageManager.StageSpawner.Count - 1].transform.position && !systemManager.regame) //���������������� �������� �ݺ� ��ư�� �������ִٸ�
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1); //�׸��̵�
                player.playerpos = stageManager.StageSpawner[0].transform; //���� �׸��� �̵� ���� �� ��ġ ��
            }
            move = true;
        }
    }
}