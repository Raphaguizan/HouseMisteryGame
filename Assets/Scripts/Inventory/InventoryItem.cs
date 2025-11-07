using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Guizan.Item
{
    [Serializable]
    public class InventoryItem
    {
        [Expandable]
        public ItemBase item;
        public int quantity;

        public InventoryItem()
        {
            item = null;
            quantity = -1;
        }
        public InventoryItem(ItemBase newItem)
        {
            item = newItem;
            quantity = 1;
        }
        public InventoryItem(ItemBase newItem, int qty)
        {
            item = newItem;
            quantity = qty;
        }

        public void UseItem()
        {
            if (quantity <= 0)
            {
                Debug.Log("Você não tem o item "+item.ItemName);
                return ;
            }

            item.Use();
        }
    }
}