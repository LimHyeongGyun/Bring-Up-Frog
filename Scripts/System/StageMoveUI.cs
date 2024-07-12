using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageMoveUI : MonoBehaviour
{
    public GameObject stageMoveUIPanel;
    public ThemaSelect[] themas;
    public Transform themaHolder;
    public GameObject[] themaStage;
    bool activeui;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    private void OnLevelWasLoaded(int level)
    {
        activeui = !activeui;
        stageMoveUIPanel.SetActive(activeui);
    }
    void Start()
    {
        stageMoveUIPanel = transform.GetChild(0).gameObject;
        stageMoveUIPanel.SetActive(activeui);
        themas = themaHolder.GetComponentsInChildren<ThemaSelect>();
        ThemaNum();
    }
    void Update()
    {

    }
    void ThemaNum()
    {
        for (int i = 0; i < themas.Length; i++)
        {
            themas[i].themaNum = i;
        }
    }

    public void ActiveStageMoveUI()
    {
        activeui = !activeui;
        stageMoveUIPanel.SetActive(activeui);
    }
}
