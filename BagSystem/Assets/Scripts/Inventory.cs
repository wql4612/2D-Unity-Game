using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory {
    public List<Item> itemList = new List<Item>();
    int itemCount = 0;

    public void AddItem(Item item) {
        bool needAddItem = true;
        for (int i = 0; i < itemList.Count; i++){
            if (itemList[i].itemName == item.itemName) {
                itemList[i].itemCount ++;
                needAddItem = false;
                break;
            }
        }
        if (needAddItem) {
            itemList.Add(item);
        }
    }

    public void RemoveItem(Item item) {
        for (int i = 0; i < itemList.Count; i++){
            if (itemList[i].itemName == item.itemName) {
                if (item.itemCount > 1) {
                    item.itemCount --;
                } else {
                    itemList.Remove(item);
                }
            }
        }
    }
    
    public List<ItemData> ToSaveData() {
        List<ItemData> itemDataList = new List<ItemData>();
        for (int i = 0; i < itemList.Count; i++) {
            itemDataList.Add(new ItemData(itemList[i]));
        }
        return itemDataList;
    }

    public void LoadSaveData(List<ItemData> itemDataList) {
        itemList.Clear();
        for (int i = 0; i < itemDataList.Count; i++) {
            itemList.Add(new Item(itemDataList[i]));
        }
    }
}
