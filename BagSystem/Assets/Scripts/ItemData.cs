using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemData {
    public string name;
    public int count;

    public ItemData() {
        this.name = "";
        this.count = 1;
    }

    public ItemData(string name, int count) {
        this.name = name;
        this.count = count;
    }

    public ItemData(Item item) {
        name = item.itemName;
        count = item.itemCount;
    }
}
