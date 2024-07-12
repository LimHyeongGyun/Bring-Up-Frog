using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class StageMove : MonoBehaviour
{
    TestPlayer player;
    GameObject[] sensorlist;
    Spawner spawner;
    Sensor sensor;
    Enemy enemy;
    public Stage_Management stageManager;
    CameraController camera;

    public int themaPos;
    public int stageNum;
    public bool move;
    public bool setstage;

    private void Start()
    {
        player = FindObjectOfType<TestPlayer>();
        enemy = FindObjectOfType<Enemy>();
        stageManager = FindObjectOfType<Stage_Management>();
        camera = FindObjectOfType<CameraController>();
    }

    private void OnLevelWasLoaded(int level)
    {
        if (SceneManager.GetActiveScene().buildIndex == themaPos)
        {
            Debug.Log("tlqkf");
            camera.fixCamera = false;
            move = true;
        }
    }
    public void StageChange()
    {
        SetStage(); //이동 전 스테이지 값 초기화
        player.stagemove = gameObject.GetComponent<StageMove>();
        enemy.stagemove = gameObject.GetComponent<StageMove>();
        if (SceneManager.GetActiveScene().buildIndex != themaPos)
            SceneManager.LoadScene(themaPos);
        stageManager = FindObjectOfType<Stage_Management>();
        player.playerpos = stageManager.StageSpawner[stageNum].transform;
        camera.fixCamera = false;
        move = true;
        StartCoroutine(Fadeout());
    }
    IEnumerator Fadeout()
    {
        Debug.Log("정지");
        yield return new WaitForSeconds(10f);

        Time.timeScale = 1;
        Debug.Log("이동완료");
    }

    //스테이지 이동시 스테이지 내의 센서와 몬스터 초기화
    void SetStage()
    {
        System_Manager.roundCount = 0;
        spawner = player.playerpos.GetComponent<Spawner>();
        spawner.clear = true;
        sensorlist = spawner.sensorList;

        foreach (GameObject sense in sensorlist) //스테이지의 센서 초기화
        {
            sensor = sense.GetComponent<Sensor>();
            sensor.curmonster.Clear();
            sensor.GetComponent<BoxCollider>().enabled = false; //센서 비활성화
        }
    }
}