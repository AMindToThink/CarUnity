using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ItemInterface
{
    string Name { get; }

    Sprite Image { get; }

    void OnPickup();
    
}
public class InventoryEventArgs : System.EventArgs
{
    public InventoryEventArgs(ItemInterface item)
    {
        Item = item;
    }
    public ItemInterface Item;
}