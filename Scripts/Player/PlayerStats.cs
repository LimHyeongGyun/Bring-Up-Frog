using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct PlayerStats
{
    public int maxHp;
    public int hpLevel;
    public int attack;
    public int attackLevel;
    public float genHp;
    public int level;
    public int ticket;
    public int gold;
    public int exp;
    public PlayerStats(int MaxHp, int HPLevel, int Attack, int AttackLevel, float GenHP, int Ticket, int Gold, int Exp, int Level)
    {
        maxHp = MaxHp;
        hpLevel = HPLevel;
        attack = Attack;
        attackLevel = AttackLevel;
        genHp = GenHP;
        ticket = Ticket;
        gold = Gold;
        exp = Exp;
        level = Level;
    }
}

public struct NeedExp
{
    public int needExp;

    public NeedExp(int NeedExp)
    {
        needExp = NeedExp;
    }
}
public enum PlayerState { Live, Die}
public enum LiveState { Idle, Walk, Attack }
public enum AttackState { None, Attack, Pierce, Windmill, Channeling, Landslash }