using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    private EnemyStats FishStats = new EnemyStats("Fish", 150, 3);
    private EnemyStats SnakeStats = new EnemyStats("Snake", 250, 5);
    private EnemyStats SquidStats = new EnemyStats("Squid", 650, 35);
    private EnemyStats BirdStats = new EnemyStats("Bird", 650, 55);
    private EnemyStats RatsStats = new EnemyStats("Rats", 1400, 75);
    private EnemyStats RizardStats = new EnemyStats("Rizard", 1700, 120);
    private EnemyStats MonkeyStats = new EnemyStats("Monkey", 2500, 130);
    private EnemyStats DeerStats = new EnemyStats("Deer", 3200, 200);
    private EnemyStats StingRayStats = new EnemyStats("StingRay", 4000, 200);
    private EnemyStats BatStats = new EnemyStats("Bat", 5200, 280);
    private EnemyStats SalamanderStats = new EnemyStats("Salamander", 6000, 285);
    private EnemyStats DemonKingStats = new EnemyStats("DemonKing", 8200, 380);
    private EnemyStats SkeletonStats = new EnemyStats("Skeleton", 8500, 365);
    private EnemyStats TurtleStats = new EnemyStats("Turtle", 12000, 450);
    private EnemyStats GolemStats = new EnemyStats("Golem", 12500, 500);
    private EnemyStats DragonStats = new EnemyStats("Dragon", 17500, 650);
    private EnemyStats SlimeStats = new EnemyStats("Slime", 8500, 365);
    private EnemyStats PlantStats = new EnemyStats("Plant", 12000, 450);
    private EnemyStats SpiderStats = new EnemyStats("Spider", 12500, 500);
    private EnemyStats EvilMageStats = new EnemyStats("EvilMage", 17500, 650);

    TestPlayer player;
    Stage_Management stageManager;
    System_Manager systemmanager;
    Sensor sensor;
    Enemy enemy;

    public GameObject[] sensorList;
    public GameObject[] spawnPoint;
    public int senpos;
    GameObject[] sen;
    public int stageArraynum;
    public bool clear = false; //맵 이동으로 스테이지 셋 시킬 때 사용
    public static int spawnCount; //스폰포인트
    public static bool StageClear;

    private void Awake()
    {
        player = FindObjectOfType<TestPlayer>();
        sensor = GetComponent<Sensor>();
        stageManager = GetComponentInParent<Stage_Management>();
    }

    private void Start()
    {
        stageArraynum = stageManager.StageSpawner.IndexOf(gameObject); //현재 스테이지의 위치를 받아오기 위한 값
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) //현재 스테이지의 몬스터 스폰
        {
            systemmanager = FindObjectOfType<System_Manager>();
            systemmanager.ExpressStage(stageArraynum);
            if(clear)
                clear = false;
            player.playerpos = stageManager.StageSpawner[stageArraynum].transform; //플레이어의 현재 스테이지 위치 저장
            //현재 스테이지 위치 값 받아오기
            foreach (GameObject sense in sensorList)
            {
                sense.GetComponent<BoxCollider>().enabled = true; //센서 활성화 시켜주기
                sen = sense.GetComponent<Sensor>().monsterList;
                sensor = sense.GetComponent<Sensor>();
                for (int i = 0; i < sen.Length; i++)
                {
                    sen[i].GetComponentInChildren<Enemy>().stagenum = stageArraynum;//현재 몬스터의 스테이지 값
                    sen[i].GetComponentInChildren<Enemy>().sense = sense; //해당 몬스터에게 현재 센서값을 입력해줌
                    sen[i].GetComponentInChildren<Enemy>().target = sen[i].transform; //센서 활성화 전 몬스터 자기 자신 위치에서 제자리 걸음 시키기 위한 값
                    sen[i].GetComponentInChildren<Enemy>().sensorpos = i;
                    sen[i].GetComponentInChildren<Enemy>().spawner = GetComponent<Spawner>();
                    if (sen[i].name == "Fish")
                    {
                        var enemyobj = Instantiate(sen[i], spawnPoint[spawnCount].transform).GetComponentInChildren<Enemy>();
                        enemyobj.EnemyStats = FishStats;
                    }
                    else if (sen[i].name == "Snake")
                    {
                        var enemyobj = Instantiate(sen[i], spawnPoint[spawnCount].transform).GetComponentInChildren<Enemy>();
                        enemyobj.EnemyStats = SnakeStats;
                    }
                    else if (sen[i].name == "Squid")
                    {
                        var enemyobj = Instantiate(sen[i], spawnPoint[spawnCount].transform).GetComponentInChildren<Enemy>();
                        enemyobj.EnemyStats = SquidStats;
                    }
                    else if (sen[i].name == "Bird")
                    {
                        var enemyobj = Instantiate(sen[i], spawnPoint[spawnCount].transform).GetComponentInChildren<Enemy>();
                        enemyobj.EnemyStats = BirdStats;
                    }
                    else if (sen[i].name == "Rats")
                    {
                        var enemyobj = Instantiate(sen[i], spawnPoint[spawnCount].transform).GetComponentInChildren<Enemy>();
                        enemyobj.EnemyStats = RatsStats;
                    }
                    else if (sen[i].name == "Rizard")
                    {
                        var enemyobj = Instantiate(sen[i], spawnPoint[spawnCount].transform).GetComponentInChildren<Enemy>();
                        enemyobj.EnemyStats = RizardStats;
                    }
                    else if (sen[i].name == "Monkey")
                    {
                        var enemyobj = Instantiate(sen[i], spawnPoint[spawnCount].transform).GetComponentInChildren<Enemy>();
                        enemyobj.EnemyStats = MonkeyStats;
                    }
                    else if (sen[i].name == "Deer")
                    {
                        var enemyobj = Instantiate(sen[i], spawnPoint[spawnCount].transform).GetComponentInChildren<Enemy>();
                        enemyobj.EnemyStats = DeerStats;
                    }
                    else if (sen[i].name == "StingRay")
                    {
                        var enemyobj = Instantiate(sen[i], spawnPoint[spawnCount].transform).GetComponentInChildren<Enemy>();
                        enemyobj.EnemyStats = StingRayStats;
                    }
                    else if (sen[i].name == "Bat")
                    {
                        var enemyobj = Instantiate(sen[i], spawnPoint[spawnCount].transform).GetComponentInChildren<Enemy>();
                        enemyobj.EnemyStats = BatStats;
                    }
                    else if (sen[i].name == "Salamander")
                    {
                        var enemyobj = Instantiate(sen[i], spawnPoint[spawnCount].transform).GetComponentInChildren<Enemy>();
                        enemyobj.EnemyStats = SalamanderStats;
                    }
                    else if (sen[i].name == "DemonKing")
                    {
                        var enemyobj = Instantiate(sen[i], spawnPoint[spawnCount].transform).GetComponentInChildren<Enemy>();
                        enemyobj.EnemyStats = DemonKingStats;
                    }
                    else if (sen[i].name == "Skeleton")
                    {
                        var enemyobj = Instantiate(sen[i], spawnPoint[spawnCount].transform).GetComponentInChildren<Enemy>();
                        enemyobj.EnemyStats = SkeletonStats;
                    }
                    else if (sen[i].name == "Turtle")
                    {
                        var enemyobj = Instantiate(sen[i], spawnPoint[spawnCount].transform).GetComponentInChildren<Enemy>();
                        enemyobj.EnemyStats = TurtleStats;
                    }
                    else if (sen[i].name == "Golem")
                    {
                        var enemyobj = Instantiate(sen[i], spawnPoint[spawnCount].transform).GetComponentInChildren<Enemy>();
                        enemyobj.EnemyStats = GolemStats;
                    }
                    else if (sen[i].name == "Dragon")
                    {
                        var enemyobj = Instantiate(sen[i], spawnPoint[spawnCount].transform).GetComponentInChildren<Enemy>();
                        enemyobj.EnemyStats = DragonStats;
                    }
                    else if (sen[i].name == "Slime")
                    {
                        var enemyobj = Instantiate(sen[i], spawnPoint[spawnCount].transform).GetComponentInChildren<Enemy>();
                        enemyobj.EnemyStats = SlimeStats;
                    }
                    else if (sen[i].name == "Plant")
                    {
                        var enemyobj = Instantiate(sen[i], spawnPoint[spawnCount].transform).GetComponentInChildren<Enemy>();
                        enemyobj.EnemyStats = PlantStats;
                    }
                    else if (sen[i].name == "Spider")
                    {
                        var enemyobj = Instantiate(sen[i], spawnPoint[spawnCount].transform).GetComponentInChildren<Enemy>();
                        enemyobj.EnemyStats = SpiderStats;
                    }
                    else if (sen[i].name == "EvilMage")
                    {
                        var enemyobj = Instantiate(sen[i], spawnPoint[spawnCount].transform).GetComponentInChildren<Enemy>();
                        enemyobj.EnemyStats = EvilMageStats;
                    }
                }
                sensor.curmonster.AddRange(spawnPoint[spawnCount].GetComponentsInChildren<Enemy>());
                spawnCount++;
                sensor.Round = false;
            }
            spawnCount = 0;
        }
    }
}
