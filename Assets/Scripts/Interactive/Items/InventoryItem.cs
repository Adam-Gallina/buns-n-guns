using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public enum ItemClass { Key, Potion }
public class InventoryItem : DirectInteraction
{
    public string itemName;
    //public ItemClass itemClass;

    protected override bool DoInteraction(Transform source)
    {
        if (source.gameObject.layer == GameController.PLAYER_INTERACTION_LAYER)
        {
            source.GetComponent<Inventory>().AddItem(this);
            gameObject.SetActive(false);
            FloatingText.instance.CreateText(transform.position, "+" + itemName);
            return true;
        }
        return false;
    }

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        return ((InventoryItem)obj).itemName == itemName;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}
