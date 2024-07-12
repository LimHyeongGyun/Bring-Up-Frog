using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmorPick : MonoBehaviour
{
    public Inventory inven;
    ItemDataBase idb;

    [HideInInspector]
    public Item item;

    public SpriteRenderer image;

    public static bool one_armor;
    public static bool ten_armor;

    private void Awake()
    {
        inven = inven.GetComponent<Inventory>();
        idb = FindObjectOfType<ItemDataBase>();
    }
    private void Update()
    {
        if (one_armor)
        {
            one_armor = false;
            OneArmor();
        }
        else if (ten_armor)
        {
            ten_armor = false;
            TenArmor();
        }
    }

    void OneArmor()
    {
        /*1성 무기/방어구 : 뽑기확률 50%
        2성 무기/방어구 : 뽑기확률 30%
        3성 무기/방어구 : 뽑기확률 14%
        4성 무기/방어구 : 뽑기확률 5%
        5성 무기/방어구 : 뽑기확률 1%*/
        var a_Pickup = new Pickup.WeightedRandomPicker<Item>();

        // 아이템 및 가중치 목록 전달
        a_Pickup.Add(

            (idb.ArmorDB[0], 50),
            (idb.ArmorDB[1], 15),
            (idb.ArmorDB[2], 15),
            (idb.ArmorDB[3], 7),
            (idb.ArmorDB[4], 7),
            (idb.ArmorDB[5], 5),
            (idb.ArmorDB[6], 1)
        );

        for (int i = 0; i < 1; i++)
        {
            Item pick = a_Pickup.GetRandomPick();
            Item additem = new Item(pick);
            inven.AddItem(additem);
            Debug.Log(inven.items);
        }
    }
    void TenArmor()
    {
        /*1성 무기/방어구 : 뽑기확률 50%
        2성 무기/방어구 : 뽑기확률 30%
        3성 무기/방어구 : 뽑기확률 14%
        4성 무기/방어구 : 뽑기확률 5%
        5성 무기/방어구 : 뽑기확률 1%*/
        var a_Pickup = new Pickup.WeightedRandomPicker<Item>();

        // 아이템 및 가중치 목록 전달
        a_Pickup.Add(

            (idb.ArmorDB[0], 50),
            (idb.ArmorDB[1], 15),
            (idb.ArmorDB[2], 15),
            (idb.ArmorDB[3], 7),
            (idb.ArmorDB[4], 7),
            (idb.ArmorDB[5], 5),
            (idb.ArmorDB[6], 1)
        );

        for (int i = 0; i < 10; i++)
        {
            Item pick = a_Pickup.GetRandomPick();
            Item additem = new Item(pick);
            inven.AddItem(additem);
            Debug.Log(inven.items);
        }
    }
}
