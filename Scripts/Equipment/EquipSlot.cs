using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;

public class EquipSlot : MonoBehaviour
{
    public enum EquipSlotType
    {
        Weapon,
        Armor
    }
    public EquipSlotType eqslottype;
    EquipUI equi;
    public Item item;

    public Image itemIcon;
    public Text leveltext;

    void Start()
    {
        equi = GetComponent<EquipUI>();
    }

    public void UpdateEQSlotUI()
    {
        if(item.equipType != EquipType.empty)
            leveltext.text = "+" + item.level.ToString();
        itemIcon.sprite = item.itemImage;
        itemIcon.gameObject.SetActive(true);
    }
    public void RemoveEQSlot()
    {
        item = null;
        itemIcon.gameObject.SetActive(false);
    }
    public void PutonSelectItem() //장비 선택
    {
        equi.weaponSlot = gameObject.GetComponent<EquipSlot>();
        equi.selectItem.sprite = itemIcon.sprite;
        equi.itemSelect.SetActive(true);
    }
}