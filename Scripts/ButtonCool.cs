using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonCool : MonoBehaviour
{
    public Button bt;
    private int coolTime = 10;
    private bool isCoolTime;

    private void Start()
    {
        this.bt = this.GetComponent<Button>();

        this.bt.onClick.AddListener(() => {
            Debug.Log("click");
            this.UseSkill();
        });
    }

    private void UseSkill()
    {
        if (this.isCoolTime)
        {
            return;
        }

        this.isCoolTime = true;

        this.StartCoroutine(this.WaitForCoolTime());
    }

    private IEnumerator WaitForCoolTime()
    {
        float delta = this.coolTime;

        while (true)
        {
            delta -= Time.deltaTime;
            Debug.Log(delta);
            if (delta <= 0)
            {
                break;
            }
            yield return null;
        }

        Debug.Log("스킬 사용가능");

        this.isCoolTime = false;
    }
}