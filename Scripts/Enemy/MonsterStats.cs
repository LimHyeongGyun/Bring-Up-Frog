using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct EnemyStats
{
    public string name;
    public int maxHp;
    public int damage;

    public EnemyStats(string Name, int MaxHp, int Damage)
    {
        name = Name;
        maxHp = MaxHp;
        damage = Damage;
    }
}
public enum EnemyType { Melee, Range }
public enum EnemyState { Idle, Walk, Attack, Die, Unbeatable }