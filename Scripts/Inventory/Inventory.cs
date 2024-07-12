using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory instance;

    public delegate void OnSlotCountChange(int val); //�븮������
    public OnSlotCountChange onSlotCountChange; //�븮�� �ν��Ͻ�ȭ

    public delegate void OnChangeItem(); //�������� �߰��Ǹ� ���� UI���� �߰��ǰ��ϴ� �븮��
    public OnChangeItem onChangeItem;

    public List<Item> items = new List<Item>();
    ItemDataBase idb;
    InventoryUI invenui;
    public TestPlayer player;

    static bool baseset;
    int slotCnt; //������ ������ ����
    public int SlotCnt
    {
        get => slotCnt;
        set
        {
            slotCnt = value;
            onSlotCountChange.Invoke(slotCnt);
        }
    }

    #region Singleton
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }
    #endregion
    private void Start()
    {
        player = FindObjectOfType<TestPlayer>();
        idb = FindObjectOfType<ItemDataBase>();
        invenui = FindObjectOfType<InventoryUI>();

        SlotCnt = 5; //���԰��� �ʱ�ȭ
        if (!baseset) //�⺻ ������ �ο�
        {
            baseset = true;
            Item baseitem = idb.WeaponDB[0];
            Item woodenstick = new Item(baseitem);
            AddItem(woodenstick);
            invenui.SetItem();
        }
    }

    public bool AddItem(Item _item)
    {
        if(items.Count < SlotCnt) //������ ���������� ������ �߰�
        {
            items.Add(_item);
            if(onChangeItem != null)
                onChangeItem.Invoke();
            return true;
        }
        return false;
    }
    public void RemoveItem(Item item)
    {
        items.Remove(item);
        onChangeItem.Invoke();
    }
    public void Stregthen(Item _item)
    {
        player.gold -= _item.basegold + _item.strengthengold * _item.level; //��ȭ��� ����
        if(_item.equipType == EquipType.Weapon)
        {
            if (_item.star == 1) _item.atk += 1;
            else if (_item.star == 2) _item.atk += 2;
            else if (_item.star == 3) _item.atk += 3;
            else if (_item.star == 4) _item.atk += 4;
            else if (_item.star == 5) _item.atk += 5;
        }
        else if (_item.equipType == EquipType.Armor)
        {
            if (_item.star == 1) _item.health += 25;
            else if (_item.star == 2) _item.health += 180;
            else if (_item.star == 3) _item.health += 800;
            else if (_item.star == 4) _item.health += 1200;
            else if (_item.star == 5) _item.health += 2000;
        }
        _item.level += 1;
    }
}