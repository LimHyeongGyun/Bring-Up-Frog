using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyWeapon : MonoBehaviour
{
    private HashSet<TestPlayer> enteredPlayer = new HashSet<TestPlayer>();
    public int enemyDMG;

    private void Update()
    {
        DestroyObj();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var player = other.GetComponentInChildren<TestPlayer>();
            if (!enteredPlayer.Contains(player)) //몬스터 중복 데미지 방지
            {
                enteredPlayer.Add(player);
                player._DecreaseHP(-enemyDMG);
            }
            Destroy(gameObject);
        }
    }
    private void DestroyObj()
    {
        Destroy(gameObject, 0.5f);
    }
}
