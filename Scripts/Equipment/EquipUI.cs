using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipUI : MonoBehaviour
{
    Equip equip;

    public GameObject equipPanel;
    bool activeEquip = false;

    public EquipSlot weaponSlot;
    public EquipSlot armorSlot;
    public GameObject itemSelect; //아이템을 선택하고 띄워주는 패널
    public Image selectItem; //선택한 아이템이미지

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    void Start()
    {
        equip = Equip.instance;
        equip.onEquipItem += ReDrawSlotUI;
        equipPanel.SetActive(activeEquip);
    }

    public void ActiveEquipUI() //장비창UI ON/OFF
    {
        activeEquip = !activeEquip;
        equipPanel.SetActive(activeEquip);
    }

    void ReDrawSlotUI() //장착한 Item UI에 띄우기
    {
        weaponSlot.RemoveEQSlot();
        weaponSlot.item = equip.Weapon;
        weaponSlot.UpdateEQSlotUI();
        armorSlot.RemoveEQSlot();
        armorSlot.item = equip.Armor;
        armorSlot.UpdateEQSlotUI();
    }
}
