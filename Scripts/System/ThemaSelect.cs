using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThemaSelect : MonoBehaviour
{
    StageMoveUI smui;

    public StageMove[] stage;
    public Transform stageHolder;

    public int themaNum;

    void Start()
    {
        smui = FindObjectOfType<StageMoveUI>();
        stage = stageHolder.GetComponentsInChildren<StageMove>();
        StageNum();
    }
    void Update()
    {

    }

    public void ActiveThemaSelectUI()
    {
        for (int i = 0; i < smui.themaStage.Length; i++)
            smui.themaStage[i].SetActive(false);
        smui.themaStage[themaNum].SetActive(true);
    }
    void StageNum()
    {
        for(int i = 0; i < stage.Length; i++)
        {
            stage[i].stageNum = i;
            stage[i].themaPos = themaNum;
        }
    }
}
