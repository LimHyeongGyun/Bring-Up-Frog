using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Equip : MonoBehaviour
{
    public static Equip instance;

    #region
    private void Awake()
    {
        if(instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }
    #endregion

    private TestPlayer player;

    public delegate void OnEquipItem();
    public OnEquipItem onEquipItem;

    public Item Weapon;
    public Item Armor;
    Item item;

    void Start()
    {
        player = FindObjectOfType<TestPlayer>();
        player.equip = GetComponent<Equip>();
    }

    void Update()
    {
        
    }

    public void EquipWeapon(Item weapon)
    {
        if(Weapon.equipType != EquipType.empty)
        {
            UnEquipWeapon();
        }
        Weapon = weapon;
        onEquipItem.Invoke();
        player.ChangeWeapon(Weapon.itemName);
    }
    public void UnEquipWeapon()
    {
        Weapon.UnEquip(player);
        Inventory.instance.AddItem(Weapon);
        onEquipItem.Invoke();
    }
    public void EquipArmor(Item armor)
    {
        if (Armor.equipType != EquipType.empty)
        {
            UnEquipArmor();
        }
        Armor = armor;
        onEquipItem.Invoke();
        player.ChangeArmor(Armor.itemName);
    }
    public void UnEquipArmor()
    {
        Armor.UnEquip(player);
        Inventory.instance.AddItem(Armor);
        onEquipItem.Invoke();
    }
}
