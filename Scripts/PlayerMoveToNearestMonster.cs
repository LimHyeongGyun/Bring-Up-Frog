using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveToNearestMonster : MonoBehaviour
{
    public Transform playerTransform;
    public List<Transform> monsterTransforms;

    // Update is called once per frame.
    void Update()
    {
        // 가까운 적 찾기
        Transform nearestMonster = null;
        float distance = float.MaxValue;
        for (int i = 0; i < monsterTransforms.Count; i++)
        {
            float currentDistance = Vector3.Distance(playerTransform.position, monsterTransforms[i].position);
            if (currentDistance < distance)
            {
                distance = currentDistance;
            }
        }

        // 가까운 적에게 이동
        if (nearestMonster != null)
        {
            Vector3 direction = nearestMonster.position - playerTransform.position;
            playerTransform.position += direction * Time.deltaTime;
        }

        // playerTransform 변수 초기화
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        // monsterTransforms 변수 초기화
        monsterTransforms = new List<Transform>();
        for (int i = 0; i < 10; i++)
        {
            Transform monsterTransform = GameObject.FindGameObjectWithTag("Monster").transform;
            monsterTransforms.Add(monsterTransform);
        }
    }
}
