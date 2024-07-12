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

    public void UpdateSlotUI() //ȹ���� ������ �̹��� Ȱ��ȭ
    {
        itemIcon.sprite = item.itemImage;
        itemIcon.gameObject.SetActive(true);
        itemlevel.text = "+" + item.level.ToString();
    }
    public void RemoveSlot() //�� Slot �������̹��� ��Ȱ��ȭ
    {
        item = null;
        itemIcon.gameObject.SetActive(false);
    }
    public void SelectItem() //����â���� ������ ������ �̹��� Ȱ��ȭ
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
