using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDataBase : MonoBehaviour
{
    public static ItemDataBase instance;
    private void Awake()
    {
        instance = this;
    }
    public List<Item>WeaponDB = new List<Item>();
    public List<Item>ArmorDB = new List<Item>();

    private void Start()
    {
        
    }
}