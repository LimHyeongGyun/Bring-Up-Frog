using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill : MonoBehaviour
{
    public int Skill_Damage;
    

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Monster"))
        {

            //Destroy(gameObject);
        }
    }
}
