using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "EquipStats", menuName = "ObjectAsset/EquipStats")]
public class EquipmentInformation : ScriptableObject
{
    public int star;
    public int level = 1;
    public int atk;
    public int health;
    public int price;

    public int strengthengold;
    public int basegold;

    private void Awake()
    {
        if (star == 1) { basegold = 250; strengthengold = 250; price = 500; }
        else if (star == 2) { basegold = 1500; strengthengold = 300; price = 1800; }
        else if (star == 3) { basegold = 3000; strengthengold = 350; price = 3350; }
        else if (star == 4) { basegold = 5500; strengthengold = 400; price = 590; }
        else if (star == 5) { basegold = 8000; strengthengold = 500; price = 8500; }
    }
}
