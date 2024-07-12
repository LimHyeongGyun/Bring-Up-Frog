using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class Slot : MonoBehaviour
{
    public Item item;
    InventoryUI invenui;

    public int slotnum;
    public Image itemIcon;
    public Text itemlevel;

    private void Start()
    {
        invenui = FindObjectOfType <InventoryUI>();
    }
    void Update()
    {
        
    }

    public void UpdateSlotUI() //획득한 아이템 이미지 활성화
    {
        itemIcon.sprite = item.itemImage;
        itemIcon.gameObject.SetActive(true);
        itemlevel.text = "+" + item.level.ToString();
    }
    public void RemoveSlot() //빈 Slot 아이템이미지 비활성화
    {
        item = null;
        itemIcon.gameObject.SetActive(false);
    }
    public void SelectItem() //선택창에서 선택한 아이템 이미지 활성화
    {
        invenui.itemInfo.text = "+" + item.level.ToString();
        if (item.equipType != EquipType.empty)
        {
            invenui.slot = gameObject.GetComponent<Slot>();
            invenui.selectItem.sprite = itemIcon.sprite;
            invenui.itemSelect.SetActive(true);
            for(int i = 0; i < invenui.itemStar.Length; i++)
            {
                invenui.itemStar[i].SetActive(false);
            }
            invenui.itemStar[item.star-1].SetActive(true);
        }
    }
}
