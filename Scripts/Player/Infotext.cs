using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Infotext : MonoBehaviour
{
    TestPlayer player;
    public Text[] curatk;
    public Text[] nextatk;
    public Text[] curhp;
    public Text[] nexthp;
    bool activeui;
    
    void Start()
    {
        player = FindObjectOfType<TestPlayer>();
        player.infotxt = gameObject.GetComponent<Infotext>();
        gameObject.SetActive(activeui);
    }

    
    void Update()
    {
        
    }

    public void ActiveInfo()
    {
        activeui = !activeui;
        gameObject.SetActive(activeui);
        ChangeStats();
    }

    public void ChangeStats()
    {
        curatk[0].text = player.attackdmglevel.ToString();
        curatk[1].text = player.attackdmg.ToString();
        curatk[2].text = "-".ToString();

        nextatk[0].text = (player.attackdmglevel + 1).ToString();
        nextatk[1].text = (player.attackdmg += Mathf.RoundToInt(player.attackdmglevel * player.attackdmg * 0.5f)).ToString();
        nextatk[2].text = (Mathf.RoundToInt(30 + player.attackdmglevel * 30 * 0.2f)).ToString();

        curhp[0].text = player.hplevel.ToString();
        curhp[1].text = player.maxhp.ToString();
        curhp[2].text = "-".ToString();

        nexthp[0].text = (player.hplevel + 1).ToString();
        nexthp[1].text = (player.maxhp += player.hplevel * 5).ToString();
        nexthp[2].text = (Mathf.RoundToInt(30 + player.hplevel * 30 * 0.2f)).ToString();
    }
}
