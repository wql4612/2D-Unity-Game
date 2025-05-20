using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Item {
    public string itemName;
    public Sprite itemImage;
    [TextArea]
    public string itemInformation;
    public int itemCount = 1;

    enum ItemType { Axe, Hammer, Magic, Stone };

    public Item() {
        this.itemImage = null;
        this.itemName = "name";
        this.itemInformation = "info";
        this.itemCount = 1;
    }

    public Item(string type): this() {
        this.itemName = type;
        if (type == "Axe") {
            this.itemImage = Resources.Load<Sprite>("axe");
            this.itemInformation = "This is an axe.";
        } else if (type == "Hammer") {
            this.itemImage = Resources.Load<Sprite>("hammer");
            this.itemInformation = "This is a hammer.";
        } else if (type == "Magic") {
            this.itemImage = Resources.Load<Sprite>("magic");
            this.itemInformation = "This is a magic skill.";
        } else if (type == "Stone") {
            this.itemImage = Resources.Load<Sprite>("stone");
            this.itemInformation = "This is a stone.";
        }
    }

    public Item(ItemData data): this(data.name) {
        this.itemCount = data.count;
    }
}

