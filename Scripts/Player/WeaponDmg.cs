using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponDmg : MonoBehaviour
{
    private HashSet<Enemy> enteredEnemy = new HashSet<Enemy>();
    public int playerDMG;

    private void Update()
    {
        DestroyObj();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            var enemy = other.GetComponentInChildren<Enemy>();
            if (!enteredEnemy.Contains(enemy)) //몬스터 중복 데미지 방지
            {
                enteredEnemy.Add(enemy);
                enemy._DecreaseHP(-playerDMG);
            }
            Destroy(gameObject);
        }
    }
    private void DestroyObj()
    {
        Destroy(gameObject, 0.5f);
    }
}
