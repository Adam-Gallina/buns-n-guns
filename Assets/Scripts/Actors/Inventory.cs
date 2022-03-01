using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    #region ItemPrefabs
    public static InventoryItem BossKey;
    public static InventoryItem BasicKey;
    public static InventoryItem HealthPotion;

    private InventoryItem LoadItem(string itemName)
    {
        return Resources.Load<InventoryItem>("Prefabs/Items/" + itemName);
    }
    #endregion

    private void Awake()
    {
        BossKey = LoadItem("Boss Key");
        BasicKey = LoadItem("Key");
        HealthPotion = LoadItem("Potion");
    }

    [SerializeField] private List<InventoryItem> inventory = new List<InventoryItem>();


    public void AddItem(GameObject newItem)
    {
        if (newItem.GetComponent<InventoryItem>())
        {
            AddItem(newItem.GetComponent<InventoryItem>());
        }
        else
        {
            Debug.LogWarning($"Trying to add {newItem.name} to inventory, but it does not have an InventoryItem component");
        }
    }
    public void AddItem(InventoryItem newItem)
    {
        inventory.Add(newItem);
    }


    public void AddItems(InventoryItem[] newItems)
    {
        foreach (InventoryItem item in newItems)
            AddItem(item);
    }

    // Returns the similar item in the inventory
    public InventoryItem GetItem(InventoryItem targetItem)
    {
        foreach (InventoryItem item in inventory)
        {
            if (item.Equals(targetItem))
                return item;
        }
        return null;
    }

    /*public InventoryItem GetItemOfClass(ItemClass targetClass)
    {
        foreach (InventoryItem item in inventory)
        {
            if (item.itemClass == targetClass)
                return item;
        }
        
        return null;
    }*/

    // Returns the items not in the inventory
    public InventoryItem[] ContainsItems(InventoryItem[] targetItems)
    {
        List<InventoryItem> missing = new List<InventoryItem>();
        List<InventoryItem> remainingInventory = new List<InventoryItem>(inventory.ToArray());

        foreach (InventoryItem item in targetItems)
        {
            bool foundItem = false;
            foreach (InventoryItem currItem in remainingInventory)
            {
                if (currItem.Equals(item))
                {
                    remainingInventory.Remove(currItem);
                    foundItem = true;
                    break;
                }
            }
            if (!foundItem)
                missing.Add(item);
        }

        return missing.ToArray();
    }

    public int CountItems(InventoryItem targetItem)
    {
        int total = 0;

        foreach (InventoryItem item in inventory)
        {
            if (item.Equals(targetItem))
                total++;
        }

        return total;
    }

    public bool RemoveItem(InventoryItem targetItem)
    {
        InventoryItem item = GetItem(targetItem);
        if (item)
        {
            inventory.Remove(item);
            return true;
        }
        else
        {
            Debug.LogWarning("Trying to remove non-existant inventory item: " + targetItem);
            return false;
        }
    }

    // Returns items that failed to remove
    public InventoryItem[] RemoveItems(InventoryItem[] items)
    {
        List<InventoryItem> missing = new List<InventoryItem>();
        foreach (InventoryItem item in items)
        {
            if (!RemoveItem(item))
                missing.Add(item);
        }

        return missing.ToArray();
    }

    // Returns items that aren't in inventory, if all items are contained removes them
    public InventoryItem[] AttemptRemoveItems(InventoryItem[] items)
    {
        InventoryItem[] missing = ContainsItems(items);

        if (missing.Length == 0)
        {
            RemoveItems(items);
        }

        return missing;
    }
}
