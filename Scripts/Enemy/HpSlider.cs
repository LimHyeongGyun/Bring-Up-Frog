using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HpSlider : MonoBehaviour
{
    public Slider Hpbar;
    public Transform enemyT;
    public Enemy enemy;
    public int maxhp;

    public void Start()
    {
        maxhp = enemy.hp;
    }
    void Update()
    {
        transform.position = enemyT.position;
        Hpbar.value = (float)enemy.hp / (float)maxhp;
    }
}
