using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class System_Manager : MonoBehaviour
{
    TestPlayer player;
    public Slider roundCounter;

    public static int roundCount;
    public int stageNum;
    public Text[] playergold;
    public Text[] playerticket;
    public Text playerlv;
    public Text playeratk;
    public Text playerhp;
    public Text playerexp;

    public GameObject InfoUI;
    public GameObject autoOn;
    public GameObject autoOff;
    public GameObject reGameOn;
    public GameObject reGameOff;
    public GameObject[] expressStage;

    public bool regame;
    public bool autoPlay;
    bool activeui;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        player = FindObjectOfType<TestPlayer>();
    }
    private void OnLevelWasLoaded(int level)
    {
        expressStage[0].SetActive(true);
    }
    private void Start()
    {
        autoPlay = true;
    }
    private void Update()
    {
        if(roundCount >= 1 && roundCounter.value == 0)
            roundCounter.value = 0.33f;
        if (roundCount >= 2 && roundCounter.value == 0.33f)
            roundCounter.value = 0.66f;
        if (roundCount == 3 && roundCounter.value == 0.66f)
            roundCounter.value = 1;
        else if(roundCount == 0)
        {
            roundCounter.value = 0;
        }
    }

    public void ActiveInfo()
    {
        activeui = !activeui;
        playerlv.text = player.level.ToString();
        playeratk.text = player.attackdmg.ToString();
        playerhp.text = player.maxhp.ToString();
        playerexp.text = player.exp + " / " + player.curneedexp.ToString();
        playergold[1].text = player.gold.ToString();
        playerticket[1].text = player.ticket.ToString();
        InfoUI.SetActive(activeui);

    }
    public void ExpressStage(int stagenum)
    {
        for(int i = 0; i < expressStage.Length; i++)
        {
            expressStage[i].SetActive(false);
        }
        expressStage[stagenum].SetActive(true);
    }
    public void Regame() //현재 스테이지 반복 판단
    {
        if (regame)
        {
            regame = false;
        }
        else if (!regame)
        {
            regame = true;
        }
        reGameOn.SetActive(regame);
        reGameOff.SetActive(!regame);
    }
    public void AutoPlay() //자동전투 판단
    {
        if (autoPlay)
        { 
            autoPlay = false;
        }
        else if (!autoPlay)
        {
            autoPlay = true;
        }
        autoOff.SetActive(!autoPlay);
        autoOn.SetActive(autoPlay);
    }
    public void OneWeaponPickBtn()
    {
        if(player.ticket > 0)
        {
            player.ticket -= 1;
            WeaponPick.one_Weapon = true;
        }
        playerticket[0].text = player.ticket.ToString();
    }
    public void TenWeaponPickBtn()
    {
        if (player.ticket > 9)
        {
            player.ticket -= 10;
            WeaponPick.ten_Weapon = true;
        }
        playerticket[0].text = player.ticket.ToString();
    }
    public void OneArmorPickBtn()
    {
        if (player.ticket > 0)
        {
            player.ticket -= 1;
            ArmorPick.one_armor = true;
        }
        playerticket[0].text = player.ticket.ToString();
    }
    public void TenArmorPickBtn()
    {
        if (player.ticket > 9)
        {
            player.ticket -= 10;
            ArmorPick.ten_armor = true;
        }
        playerticket[0].text = player.ticket.ToString();
    }
}