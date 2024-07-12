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
    public GameObject itemSelect; //�������� �����ϰ� ����ִ� �г�
    public Image selectItem; //������ �������̹���

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

    public void ActiveEquipUI() //���âUI ON/OFF
    {
        activeEquip = !activeEquip;
        equipPanel.SetActive(activeEquip);
    }

    void ReDrawSlotUI() //������ Item UI�� ����
    {
        weaponSlot.RemoveEQSlot();
        weaponSlot.item = equip.Weapon;
        weaponSlot.UpdateEQSlotUI();
        armorSlot.RemoveEQSlot();
        armorSlot.item = equip.Armor;
        armorSlot.UpdateEQSlotUI();
    }
}
