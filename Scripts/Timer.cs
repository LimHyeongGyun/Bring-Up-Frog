using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public static float game_time = 60f; //게임 플레이 시간
    private GameObject text_child_object;

    public bool GameOver = false;
    public GameObject Canvas_GameTime;
    public GameObject Canvas_Restart;

    public Player player;

    void Start()
    {
        Canvas_Restart.SetActive(false);
        game_time = 60f;
        text_child_object = transform.GetChild(0).gameObject;
    }

    void Update()
    {
        game_time -= Time.deltaTime;
        int int_game_time = (int)Mathf.Round(game_time); //float -> int
        text_child_object.GetComponent<Text>().text = "Time : " + int_game_time;

        if (int_game_time < 0)
        {
            GameOver = true;
            Debug.Log("게 임 종 료");
            Canvas_GameTime.SetActive(false);
            text_child_object.GetComponent<Text>().text = "GAME OVER";
        }
        
        if (GameOver)
        {
            Canvas_Restart.SetActive(true);
        }
        else
            Canvas_Restart.SetActive(false);
    }
}
