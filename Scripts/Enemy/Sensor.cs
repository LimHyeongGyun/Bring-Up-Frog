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

    public Transform curRound; //플레이라운드 값 저장 변수
    public GameObject target; //몬스터가 참조하는 타겟
    public GameObject[] monsterList; //스폰하는 몬스터 종류
    public List<Enemy> curmonster; //현재 라운드의 몬스터
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
        if (player.playerState == PlayerState.Die) //플레이어 사망시
        {
            curmonster.Clear();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) //일정 범위 안에 플레이어가 인식됐을 때
        {
            Debug.Log("sensor in");
            gameObject.GetComponent<BoxCollider>().enabled = false; //센서가 전투중 재작동하지 않도록
            cameracontroller.sensor = gameObject.transform;
            cameracontroller.fixCamera = true; //해당 라운드에 카메라 고정
            player.sensor = GetComponent<Sensor>();
            target = other.gameObject;
            System_Manager.roundCount++; //라운드 카운트
        }
    }

    IEnumerator Fadeout()
    {
        Debug.Log("플레이어 위치를 다음 스테이지로 이동시킵니다");
        yield return new WaitForSeconds(1f);
    }

    //스테이지 클리어시 보상지급
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
        Round = true; //클리어 했으니 !Round를 false로 복구
        if (spawner.sensorList[spawner.sensorList.Length - 1] == gameObject) //마지막 라운드인지 판단
        {
            //마지막 라운드였다면 스테이지 클리어
            Debug.Log("StageClear");
            PayingCompensation();
            StartCoroutine(Fadeout());
            spawner.clear = true;
            spawner.clear = false;
            System_Manager.roundCount = 0;
            if (systemManager.regame) //스테이지 반복 버튼을 눌렀다면
            {
                //해당 스테이지 다시시작
                player.playerpos = stageManager.StageSpawner[spawner.stageArraynum].transform;
            }
            else if (player.playerpos.position != stageManager.StageSpawner[stageManager.StageSpawner.Count - 1].transform.position && !systemManager.regame) //마지막 스테이지가 아니고 스테이지 반복 버튼이 안켜져있다면
            {
                player.playerpos = stageManager.StageSpawner[spawner.stageArraynum + 1].transform; //다음 스테이지 이동
            }
            else if (player.playerpos.position == stageManager.StageSpawner[stageManager.StageSpawner.Count - 1].transform.position && !systemManager.regame) //마지막스테이지며 스테이지 반복 버튼이 안켜져있다면
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1); //테마이동
                player.playerpos = stageManager.StageSpawner[0].transform; //다음 테마로 이동 했을 때 위치 값
            }
            move = true;
        }
    }
}