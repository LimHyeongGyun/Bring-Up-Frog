using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class WeaponPick : MonoBehaviour
{
    public Inventory inven;
    ItemDataBase idb;
    [HideInInspector]
    public Item item;
    public SpriteRenderer image;

    public static bool one_Weapon;
    public static bool ten_Weapon;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        inven = inven.GetComponent<Inventory>();
        idb = FindObjectOfType<ItemDataBase>();
    }
    private void Update()
    {
        if (one_Weapon)
        {
            one_Weapon = false;
            OneWeapon();
        }
        else if (ten_Weapon)
        {
            ten_Weapon = false;
            TenWeapon();
        }
    }

    void OneWeapon()
    {
        /*1�� ����/�� : �̱�Ȯ�� 50%
        2�� ����/�� : �̱�Ȯ�� 30%
        3�� ����/�� : �̱�Ȯ�� 14%
        4�� ����/�� : �̱�Ȯ�� 5%
        5�� ����/�� : �̱�Ȯ�� 1%*/
        var w_Pickup = new Pickup.WeightedRandomPicker<Item>();

        // ������ �� ����ġ ��� ����
        w_Pickup.Add(

            (idb.WeaponDB[1], 50),
            (idb.WeaponDB[2], 15),
            (idb.WeaponDB[3], 15),
            (idb.WeaponDB[4], 7),
            (idb.WeaponDB[5], 7),
            (idb.WeaponDB[6], 5),
            (idb.WeaponDB[7], 1),
            (idb.WeaponDB[8], 1)
        );

        for (int i = 0; i < 1; i++) //���� ������
        {
            Item pick = w_Pickup.GetRandomPick();
            Item additem = new Item(pick);
            inven.AddItem(additem);
            Debug.Log(inven.items);
        }

        Debug.Log("");
        foreach (var item in w_Pickup.GetItemDictReadonly()) //�������� ����ġ
        {
            Debug.Log(item);
        }

        Debug.Log("");
        foreach (var item in w_Pickup.GetNormalizedItemDictReadonly()) //������ Ȯ��
        {
            Debug.Log(item);
        }
    }
    void TenWeapon()
    {
        /*1�� ����/�� : �̱�Ȯ�� 50%
        2�� ����/�� : �̱�Ȯ�� 30%
        3�� ����/�� : �̱�Ȯ�� 14%
        4�� ����/�� : �̱�Ȯ�� 5%
        5�� ����/�� : �̱�Ȯ�� 1%*/
        var w_Pickup = new Pickup.WeightedRandomPicker<Item>();

        // ������ �� ����ġ ��� ����
        w_Pickup.Add(

            (idb.WeaponDB[1], 50),
            (idb.WeaponDB[2], 15),
            (idb.WeaponDB[3], 15),
            (idb.WeaponDB[4], 7),
            (idb.WeaponDB[5], 7),
            (idb.WeaponDB[6], 5),
            (idb.WeaponDB[7], 1),
            (idb.WeaponDB[8], 1)
        );

        for (int i = 0; i < 10; i++) //���� ������
        {
            Item pick = w_Pickup.GetRandomPick();
            Item additem = new Item(pick);
            inven.AddItem(additem);
            Debug.Log(inven.items);
        }
    }
}
