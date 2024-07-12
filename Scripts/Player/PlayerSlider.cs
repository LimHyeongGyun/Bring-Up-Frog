using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSlider : MonoBehaviour
{
    TestPlayer player;
    public Slider hpSlider;
    public Slider expSlider;

    void Start()
    {
        player = FindObjectOfType<TestPlayer>();
    }

    void Update()
    {
        hpSlider.value = (float)player.hp / (float)player.maxhp;
        expSlider.value = (float)player.exp / (float)player.curneedexp;
    }
}
