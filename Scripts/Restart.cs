using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Restart : MonoBehaviour
{
    Button button;
    public bool Select = false;
    private float DelTime = 0.5f;
    public Vector3 respawnPosition = new Vector3(0, 1, 17); // 리스폰 위치

    

    private void Start()
    {
        button = GetComponent<Button>();
        //button.onClick.AddListener(RestartScene);
    }

    void Update()
    {
        if (Select)
        {
            DelTime -= Time.deltaTime;
        }
        else
        {
            DelTime = 0.5f;
        }
        
        if (DelTime <= 0.0f)
        {
            Invoke("Respawn", 3.0f);
        }
    }
    /*
    private void RestartScene()
    {
        SceneManager.LoadScene(0);
    }*/

    // 리스폰 함수
    private void Respawn()
    {
        // 플레이어 캐릭터의 트랜스폼을 가져옴
        Transform playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        // 플레이어 캐릭터를 리스폰 위치로 이동시킴
        playerTransform.position = respawnPosition;
    }
}
