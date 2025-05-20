using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using LitJson;
using System.Collections;
using System.Collections.Generic;

public static class GameSaveManager {
    static string savePath = Application.persistentDataPath + "/game_SaveData/";
    static string filename = "inventory.json";

    public static void SaveGame() {
        if (!Directory.Exists(savePath)) {
            Directory.CreateDirectory(savePath);
        }
        var inventory = InventoryManager.instance.playerBag;
        var json = JsonMapper.ToJson(inventory.ToSaveData());
        File.WriteAllText(savePath + filename, json);
    }

    public static void LoadGame() {
        if (File.Exists(savePath + filename)) {
            string json = File.ReadAllText(savePath + filename);
            var dataList = JsonMapper.ToObject<List<ItemData>>(json);
            InventoryManager.LoadSaveData(dataList);
        }
    }
}
