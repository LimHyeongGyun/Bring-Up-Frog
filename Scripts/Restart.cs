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
    public Vector3 respawnPosition = new Vector3(0, 1, 17); // ������ ��ġ

    

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

    // ������ �Լ�
    private void Respawn()
    {
        // �÷��̾� ĳ������ Ʈ�������� ������
        Transform playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        // �÷��̾� ĳ���͸� ������ ��ġ�� �̵���Ŵ
        playerTransform.position = respawnPosition;
    }
}
