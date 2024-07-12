using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static AudioManager;
using static Inventory;
//using static UnityEditor.Progress;

public class InventoryUI : MonoBehaviour
{
    System_Manager systemManager;
    Inventory inven;
    TestPlayer player;
    public GameObject inventoryPanel;
    bool activeInventory = false;

    public Slot slot;
    public Slot[] slots;
    public Transform slotHolder;

    public GameObject itemSelect;
    public Image selectItem;
    public GameObject[] itemStar;
    public Text itemInfo;
    public Text warining;
    public GameObject waringUI;
    public int info;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    private void Start()
    {
        inven = Inventory.instance;
        slots = slotHolder.GetComponentsInChildren<Slot>();
        player = FindObjectOfType<TestPlayer>();
        systemManager = FindObjectOfType<System_Manager>();
        inven.onSlotCountChange += SlotChange;
        inven.onChangeItem += RedrawSlotUI;//획득한 아이템 화면에 띄우기
        inventoryPanel.SetActive(activeInventory);
    }

    void SlotChange(int val) //정해진 슬롯 개수만 활성화
    {
        for(int i = 0; i < slots.Length; i++)
        {
            slots[i].slotnum = i; //슬롯의 slotnum을 차례로 번호부여
            if (i < inven.SlotCnt)
                slots[i].GetComponent<Button>().interactable = true;
            else
                slots[i].GetComponent<Button>().interactable = false;
        }
    }
    public void AddSlot() //슬롯추가버튼
    {
        if (inven.SlotCnt < 30)
        {
            inven.SlotCnt += 5;
            player.gold -= 1000;
            systemManager.playergold[0].text = player.gold.ToString();
        }
        else
        {
            waringUI.SetActive(true);
            warining.text = "이미 슬롯의 개수가 최대입니다.".ToString();
        }
    }

    public void ActiveInventory() //인벤토리UI ON/OFF
    {
        activeInventory = !activeInventory;
        inventoryPanel.SetActive(activeInventory);
    }

    void RedrawSlotUI() //슬롯을 초기화하고 items의 개수만큼 slot을 채워넣음
    {
        for(int i = 0; i < slots.Length; i++)
        {
            slots[i].RemoveSlot();
        }
        for(int i = 0; i < inven.items.Count; i++)
        {
            slots[i].item = inven.items[i];
            slots[i].UpdateSlotUI();
        }
    }

    public void PutOn()
    {
        slot.item.Equip(player);
        if (slot.item.equipType == EquipType.Weapon)
        {
            Equip.instance.EquipWeapon(slot.item);
        }
        if (slot.item.equipType == EquipType.Armor)
        {
            Equip.instance.EquipArmor(slot.item);
        }
        Inventory.instance.RemoveItem(slot.item);//아이템을 착용 했을때 인벤토리에서 제거
        itemSelect.SetActive(false);
    }
    public void Sell()
    {
        slot.item.SellItem(player);
        Inventory.instance.RemoveItem(slot.item);
        itemSelect.SetActive(false);
    }
    public void StrengthItem()
    {
        if(slot.item.level < 5)
        {
            Inventory.instance.Stregthen(slot.item);
        }
        inven.onChangeItem.Invoke();
        slot.SelectItem();
    }

    public void SetItem()
    {
        slots[0].item.Equip(player);
        if (slots[0].item.equipType == EquipType.Weapon)
        {
            Equip.instance.EquipWeapon(slots[0].item);
        }
        Inventory.instance.RemoveItem(slots[0].item);//아이템을 착용 했을때 인벤토리에서 제거
        itemSelect.SetActive(false);
    }
}
