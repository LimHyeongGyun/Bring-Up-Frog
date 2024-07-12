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
        /*1성 무기/방어구 : 뽑기확률 50%
        2성 무기/방어구 : 뽑기확률 30%
        3성 무기/방어구 : 뽑기확률 14%
        4성 무기/방어구 : 뽑기확률 5%
        5성 무기/방어구 : 뽑기확률 1%*/
        var w_Pickup = new Pickup.WeightedRandomPicker<Item>();

        // 아이템 및 가중치 목록 전달
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

        for (int i = 0; i < 1; i++) //뽑은 아이템
        {
            Item pick = w_Pickup.GetRandomPick();
            Item additem = new Item(pick);
            inven.AddItem(additem);
            Debug.Log(inven.items);
        }

        Debug.Log("");
        foreach (var item in w_Pickup.GetItemDictReadonly()) //아이템의 가중치
        {
            Debug.Log(item);
        }

        Debug.Log("");
        foreach (var item in w_Pickup.GetNormalizedItemDictReadonly()) //아이템 확률
        {
            Debug.Log(item);
        }
    }
    void TenWeapon()
    {
        /*1성 무기/방어구 : 뽑기확률 50%
        2성 무기/방어구 : 뽑기확률 30%
        3성 무기/방어구 : 뽑기확률 14%
        4성 무기/방어구 : 뽑기확률 5%
        5성 무기/방어구 : 뽑기확률 1%*/
        var w_Pickup = new Pickup.WeightedRandomPicker<Item>();

        // 아이템 및 가중치 목록 전달
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

        for (int i = 0; i < 10; i++) //뽑은 아이템
        {
            Item pick = w_Pickup.GetRandomPick();
            Item additem = new Item(pick);
            inven.AddItem(additem);
            Debug.Log(inven.items);
        }
    }
}
