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
        SetStage(); //�̵� �� �������� �� �ʱ�ȭ
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
        Debug.Log("����");
        yield return new WaitForSeconds(10f);

        Time.timeScale = 1;
        Debug.Log("�̵��Ϸ�");
    }

    //�������� �̵��� �������� ���� ������ ���� �ʱ�ȭ
    void SetStage()
    {
        System_Manager.roundCount = 0;
        spawner = player.playerpos.GetComponent<Spawner>();
        spawner.clear = true;
        sensorlist = spawner.sensorList;

        foreach (GameObject sense in sensorlist) //���������� ���� �ʱ�ȭ
        {
            sensor = sense.GetComponent<Sensor>();
            sensor.curmonster.Clear();
            sensor.GetComponent<BoxCollider>().enabled = false; //���� ��Ȱ��ȭ
        }
    }
}