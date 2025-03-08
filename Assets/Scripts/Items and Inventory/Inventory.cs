using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Inventory : MonoBehaviour, ISaveManager
{
    public static Inventory instance;

    public List<ItemData> startingItems;

    public List<InventoryItem> equipment;
    public Dictionary<ItemData_Equipment, InventoryItem> equipmentDictionary;

    public List<InventoryItem> inventory;
    public Dictionary<ItemData, InventoryItem> inventoryDictionary;

    public List<InventoryItem> stash;
    public Dictionary <ItemData, InventoryItem> stashDictionary;

    [Header("Inventory UI")]
    [SerializeField] private Transform inventorySlotParent;
    [SerializeField] private Transform stashSlotParent;
    [SerializeField] private Transform equipmentSlotParent;
    [SerializeField] private Transform statSlotParent;

    private UI_ItemSlot[] inventoryItemSlot;
    private UI_ItemSlot[] stashItemSlot;
    private UI_EquipmentSetSlot[] equipmentSlot;
    private UI_StatSlot[] statSlot;

    [Header("Items cooldown")]
    private float lastTimeUsedFlask;
    private float lastTimeUsedArmor;

    public float flaskCooldown { get; private set; }
    private float armorCooldown;

    [Header("Data base")]
    public List<ItemData> itemDataBase;
    public List<InventoryItem> loadedItems;
    public List<ItemData_Equipment> loadedEquipment;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        inventory = new List<InventoryItem>();
        inventoryDictionary = new Dictionary<ItemData, InventoryItem>();

        stash = new List<InventoryItem>();
        stashDictionary = new Dictionary<ItemData, InventoryItem>();

        equipment = new List<InventoryItem>();
        equipmentDictionary = new Dictionary<ItemData_Equipment, InventoryItem>();

        inventoryItemSlot = inventorySlotParent.GetComponentsInChildren<UI_ItemSlot>();
        stashItemSlot = stashSlotParent.GetComponentsInChildren<UI_ItemSlot>();
        equipmentSlot = equipmentSlotParent.GetComponentsInChildren<UI_EquipmentSetSlot>();
        statSlot = statSlotParent.GetComponentsInChildren<UI_StatSlot>();

        // デバッグ用にスロットの初期化状態を確認
        Debug.Log($"Inventory slots initialized: {inventoryItemSlot.Length}");
        Debug.Log($"Stash slots initialized: {stashItemSlot.Length}");
        Debug.Log($"Equipment slots initialized: {equipmentSlot.Length}");

        AddStartingItems();
    }

    private void AddStartingItems()
    {
        foreach (ItemData_Equipment item in loadedEquipment)
        {
            if (item != null)
            {
                EquipItem(item);
            }
            else
            {
                Debug.LogWarning("AddStartingItems: loadedEquipment contains null item");
            }
        }

        if (loadedItems.Count > 0)
        {
            foreach (InventoryItem item in loadedItems)
            {
                if (item != null && item.data != null)
                {
                    for (int i = 0; i < item.stackSize; i++)
                    {
                        AddItem(item.data);
                    }
                }
                else
                {
                    Debug.LogWarning("AddStartingItems: loadedItems contains null item or item.data");
                }
            }
            return;
        }

        for (int i = 0; i < startingItems.Count; i++)
        {
            if (startingItems[i] != null)
            {
                AddItem(startingItems[i]);
            }
            else
            {
                Debug.LogWarning($"AddStartingItems: startingItems[{i}] is null");
            }
        }
    }


    //public void EquipItem(ItemData _item)
    //{
    //    Debug.Log("[EquipItem] Called with item: " + _item.itemName);

    //    ItemData_Equipment newEquipment = _item as ItemData_Equipment;
    //    if (newEquipment == null)
    //    {
    //        Debug.LogWarning("[EquipItem] Item is not of type ItemData_Equipment.");
    //        return;
    //    }

    //    InventoryItem newItem = new InventoryItem(newEquipment);
    //    Debug.Log("[EquipItem] New InventoryItem created for: " + newEquipment.itemName);

    //    ItemData_Equipment oldEquipment = null;

    //    foreach (KeyValuePair<ItemData_Equipment, InventoryItem> item in equipmentDictionary)
    //    {
    //        if (item.Key.equipmentType == newEquipment.equipmentType)
    //        {
    //            oldEquipment = item.Key;
    //            break;
    //        }
    //    }

    //    if (oldEquipment != null)
    //    {
    //        Debug.Log("[EquipItem] Existing equipment found of same type: " + oldEquipment.itemName + ". Unequipping it.");
    //        UnequipItem(oldEquipment);
    //        AddItem(oldEquipment);
    //    }

    //    equipment.Add(newItem);
    //    equipmentDictionary.Add(newEquipment, newItem);
    //    Debug.Log("[EquipItem] Equipment added: " + newEquipment.itemName);

    //    newEquipment.AddModifiers();
    //    Debug.Log("[EquipItem] Modifiers applied for: " + newEquipment.itemName);

    //    RemoveItem(_item);
    //    Debug.Log("[EquipItem] Item removed from inventory: " + _item.itemName);

    //    UpdateSlotUI();
    //    Debug.Log("[EquipItem] UI slots updated.");
    //}

    public void EquipItem(ItemData _item)
    {
        Debug.Log("[EquipItem] Called with item: " + _item.itemName);

        // 装備用アイテムとしてキャスト
        ItemData_Equipment newEquipment = _item as ItemData_Equipment;
        if (newEquipment == null)
        {
            Debug.LogWarning("[EquipItem] Item is not of type ItemData_Equipment.");
            return;
        }

        // 新規装備用の InventoryItem を作成
        InventoryItem newItem = new InventoryItem(newEquipment);
        Debug.Log("[EquipItem] New InventoryItem created for: " + newEquipment.itemName);

        // 同じカテゴリー（EquipmentType）の既存装備を検索
        ItemData_Equipment oldEquipment = null;
        foreach (KeyValuePair<ItemData_Equipment, InventoryItem> pair in equipmentDictionary)
        {
            if (pair.Key.equipmentType == newEquipment.equipmentType)
            {
                oldEquipment = pair.Key;
                break;
            }
        }

        // 既に同カテゴリーの装備がセットされている場合は、アンイコップしてインベントリに戻す
        if (oldEquipment != null)
        {
            Debug.Log("[EquipItem] Existing equipment found of same type: " + oldEquipment.itemName + ". Unequipping it.");
            UnequipItem(oldEquipment);    // 装備解除処理（例：モディファイア解除）
            AddItem(oldEquipment);          // 古い装備をインベントリに戻す
        }

        // 新しい装備を装備リストに追加し、辞書に登録
        equipment.Add(newItem);
        equipmentDictionary.Add(newEquipment, newItem);
        Debug.Log("[EquipItem] Equipment added: " + newEquipment.itemName);

        // 新しい装備のステータスモディファイアをプレイヤーに適用
        newEquipment.AddModifiers();
        Debug.Log("[EquipItem] Modifiers applied for: " + newEquipment.itemName);

        // 新しい装備アイテムはインベントリから除外する
        RemoveItem(_item);
        Debug.Log("[EquipItem] Item removed from inventory: " + _item.itemName);

        // UIのスロット表示更新
        UpdateSlotUI();
        Debug.Log("[EquipItem] UI slots updated.");
    }



    public void UnequipItem(ItemData_Equipment itemToRemove)
    {
        if (equipmentDictionary.TryGetValue(itemToRemove, out InventoryItem value))
        {
            equipment.Remove(value);
            equipmentDictionary.Remove(itemToRemove);
            itemToRemove.RemoveModifiers();
        }
    }

    private void UpdateSlotUI()
    {
        // 装備スロットの更新
        for (int i = 0; i < equipmentSlot.Length; i++)
        {
            equipmentSlot[i].CleanUpSlot();

            foreach (var item in equipmentDictionary)
            {
                if (item.Key.equipmentType == equipmentSlot[i].slotType)
                {
                    equipmentSlot[i].UpdateSlot(item.Key, item.Value.stackSize);
                    break;
                }
            }
        }

        // インベントリスロットのクリーンアップ
        foreach (var slot in inventoryItemSlot)
            slot.CleanUpSlot();

        // インベントリスロットの更新
        int slotIndex = 0;
        foreach (var item in inventoryDictionary)
        {
            if (slotIndex < inventoryItemSlot.Length)
            {
                inventoryItemSlot[slotIndex].UpdateSlot(item.Key, item.Value.stackSize);
                slotIndex++;
            }
            else
            {
                Debug.LogWarning("Not enough slots to display all inventory items!");
                break;
            }
        }

        // スタッシュスロットのクリーンアップ
        foreach (var slot in stashItemSlot)
            slot.CleanUpSlot();

        // スタッシュスロットの更新
        slotIndex = 0;
        foreach (var item in stashDictionary)
        {
            if (slotIndex < stashItemSlot.Length)
            {
                stashItemSlot[slotIndex].UpdateSlot(item.Key, item.Value.stackSize);
                slotIndex++;
            }
            else
            {
                Debug.LogWarning("Not enough slots to display all stash items!");
                break;
            }
        }

        // ステータスUIの更新
        UpdateStatsUI();
    }




    public void UpdateStatsUI()
    {
        for (int i = 0; i < statSlot.Length; i++)
        {
            statSlot[i].UpdateStatValueUI();
        }
    }

    public void AddItem(ItemData _item)
    {
        if (_item.itemType == ItemType.Equipment && CanAddItem())
            AddToInventory(_item);
        
        else if (_item.itemType == ItemType.Material)
            AddToStash(_item);

        UpdateSlotUI();
    }

    private void AddToStash(ItemData _item)
    {
        if (stashDictionary.TryGetValue(_item, out InventoryItem value))
        {
            value.AddStack();
        }
        else
        {
            InventoryItem newItem = new InventoryItem(_item);
            stash.Add(newItem);
            stashDictionary.Add(_item, newItem);
        }
    }

    private void AddToInventory(ItemData _item)
    {
        if (inventoryDictionary.TryGetValue(_item, out InventoryItem value))
        {
            value.AddStack();
        }
        else
        {
            InventoryItem newItem = new InventoryItem(_item);
            inventory.Add(newItem);
            inventoryDictionary.Add(_item, newItem);
        }
    }

    public void RemoveItem(ItemData _item)
    {
        if (inventoryDictionary.TryGetValue(_item, out InventoryItem value))
        {
            if (value.stackSize <= 1)
            {
                inventory.Remove(value);
                inventoryDictionary.Remove(_item);
            }
            else
                value.RemoveStack();
        }

        if (stashDictionary.TryGetValue(_item, out InventoryItem stashValue))
        {
            if (stashValue.stackSize <= 1)
            {
                stash.Remove(stashValue);
                stashDictionary.Remove(_item);
            }
            else
                stashValue.RemoveStack();
        }

        UpdateSlotUI();
    }

    public bool CanAddItem()
    {
        if (inventory.Count >= inventoryItemSlot.Length)
        {
            Debug.Log("No more space");
            return false;
        }

        return true;
    }

    public bool CanCraft(ItemData_Equipment _itemToCraft, List<InventoryItem> _requiredMaterials)
    {
        // Check if all required materials are available with the required quantity

        foreach (var requiredItem in _requiredMaterials)
        {
            if (stashDictionary.TryGetValue(requiredItem.data, out InventoryItem stashItem))
            {
                if (stashItem.stackSize < requiredItem.stackSize)
                {
                    Debug.Log("Not enough materials: " + requiredItem.data.name);
                    return false;
                }
            }
            else
            {
                Debug.Log("Materials not found in stash: " + requiredItem.data.name);
                return false;
            }
        }

        // If all materials are available, remove them from stash

        foreach (var requiredMaterial in _requiredMaterials)
        {
            for (int i = 0; i < requiredMaterial.stackSize; i++)
            {
                RemoveItem(requiredMaterial.data);
            }
        }

        AddItem(_itemToCraft);
        Debug.Log("Craft is successful: " + _itemToCraft.name);
        return true;
    }

    public List<InventoryItem> GetEquipmentList() => equipment;

    public List<InventoryItem> GetStashList() => stash;

    public ItemData_Equipment GetEquipment(EquipmentType _type)
    {
        ItemData_Equipment equipedItem = null;

        foreach (KeyValuePair<ItemData_Equipment, InventoryItem> item in equipmentDictionary)
        {
            if (item.Key.equipmentType == _type)
                equipedItem = item.Key;
        }

        return equipedItem;
    }

    public void UseFlask()
    {
        ItemData_Equipment currentFlask = GetEquipment(EquipmentType.Flask);

        if (currentFlask == null)
            return;

        bool canUseFlask = Time.time > lastTimeUsedFlask + flaskCooldown;

        if (canUseFlask)
        {
            flaskCooldown = currentFlask.itemCooldown;
            currentFlask.Effect(null);
            lastTimeUsedFlask = Time.time;
        }
        else
            Debug.Log("Flask on cooldown;");
    }

    public bool CanUseArmor()
    {
        ItemData_Equipment currentArmor = GetEquipment(EquipmentType.Armor);

        if(Time.time > lastTimeUsedArmor + armorCooldown)
        {
            armorCooldown = currentArmor.itemCooldown;
            lastTimeUsedArmor = Time.time;
            return true;
        }

        Debug.Log("Armor on cooldown");
        return false;
    }

    public void LoadData(GameData _data)
    {
        foreach (KeyValuePair<string, int> pair in _data.inventory)
        {
            foreach (var item in itemDataBase)
            {
                if (item != null && item.itemId == pair.Key)
                {
                    InventoryItem itemToLoad = new InventoryItem(item);
                    itemToLoad.stackSize = pair.Value;

                    loadedItems.Add(itemToLoad);
                }
            }
        }

        foreach (string loadedItemId in _data.equipmentId)
        {
            foreach (var item in itemDataBase)
            {
                if (item != null && loadedItemId == item.itemId)
                {
                    loadedEquipment.Add(item as ItemData_Equipment);
                }
            }
        }
    }

    public void SaveData(ref GameData _data)
    {
        _data.inventory.Clear();
        _data.equipmentId.Clear();


        foreach (KeyValuePair<ItemData, InventoryItem> pair in inventoryDictionary)
        {
            _data.inventory.Add(pair.Key.itemId, pair.Value.stackSize);
        }

        foreach (KeyValuePair<ItemData, InventoryItem> pair in stashDictionary)
        {
            _data.inventory.Add(pair.Key.itemId, pair.Value.stackSize);
        }

        foreach (KeyValuePair<ItemData_Equipment, InventoryItem> pair in equipmentDictionary)
        {
            _data.equipmentId.Add(pair.Key.itemId);
        }
    }


#if UNITY_EDITOR
    [ContextMenu("Fill up item data base")]
    private void FillUpItemDataBase() => itemDataBase = new List<ItemData>(GetItemDataBase());

    private List<ItemData> GetItemDataBase()
    {
        List<ItemData> itemDataBase = new List<ItemData>();
        string[] assetNames = AssetDatabase.FindAssets("", new[] { "Assets/Data/Items" });

        foreach (string SOName in assetNames)
        {
            var SOpath = AssetDatabase.GUIDToAssetPath(SOName);
            var itemData = AssetDatabase.LoadAssetAtPath<ItemData>(SOpath);
            itemDataBase.Add(itemData);
        }

        return itemDataBase;
    }
#endif
}
