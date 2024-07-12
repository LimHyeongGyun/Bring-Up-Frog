using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using static AudioManager;
using static UnityEditor.Progress;

public enum EquipType
{
    empty,
    Weapon,
    Armor
}
public enum ItemType
{
    Equipment,
    Etc
}

[System.Serializable]
public class Item
{
    public EquipType equipType;
    public ItemType itemType;
    public string itemName;
    public Sprite itemImage;
    public GameObject equipMent;
    public EquipmentInformation eqinfo;

    InventoryUI invenui;

    [HideInInspector]
    public int star;
    [HideInInspector]
    public int level;
    [HideInInspector]
    public int atk;
    [HideInInspector]
    public int health;
    [HideInInspector]
    public int price;
    [HideInInspector]
    public int strengthengold;
    [HideInInspector]
    public int basegold;

    public Item(Item _item)
    {
        equipType = _item.equipType;
        itemType = _item.itemType;
        itemName = _item.itemName;
        itemImage = _item.itemImage;
        equipMent = _item.equipMent;
        star = _item.eqinfo.star;
        level = _item.eqinfo.level;
        atk = _item.eqinfo.atk;
        health = _item.eqinfo.health;
        price = _item.eqinfo.price;
        strengthengold = _item.eqinfo.strengthengold;
        basegold = _item.eqinfo.basegold;
    }

    public void Equip(TestPlayer player)
    {
        player.attackdmg += atk;
        player.hp += health;
    }
    public void UnEquip(TestPlayer player)
    {
        player.attackdmg -= atk;
        player.hp -= health;
    }
    public void SellItem(TestPlayer player)
    {
        player.gold += price;
    }
}
