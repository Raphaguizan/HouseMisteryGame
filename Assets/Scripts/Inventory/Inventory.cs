using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEditor.Search;
using UnityEngine;


namespace Guizan.Item
{
    [CreateAssetMenu(fileName ="NewInventory", menuName ="Inventory", order = 0)]
    public class Inventory : ScriptableObject
    {
        [SerializeField]
        private List<InventoryItem> itemsList;

        public void InitializeIventory()
        {
            // Load do inventário quando fizer o save
        }

        public void AddItem(ItemBase item, int qty = 1)
        {
            if (item == null)
            {
                Debug.LogWarning("Não é possível adicionar item Nulo");
                return;
            }
            if(qty <= 0)
            {
                Debug.LogWarning("Não é possível adicionar quantidade 0 ou negativa de itens");
                return;
            }

            var foundItem = itemsList.Find(i => i.item.ItemName.Equals(item.ItemName));
            if (foundItem != null)
            {
                foundItem.quantity += qty;
            }
            itemsList.Add(new(item, qty));
        }

        public void RemoveItem(string itemName, int qty = 1, bool fullRemove = false)
        {
            var foundItem = itemsList.Find(i => i.item.ItemName.Equals(itemName));
            if (foundItem == null)
            {
                Debug.LogWarning("você não tem o item "+itemName);
                return;
            }

            foundItem.quantity -= qty;

            if(foundItem.quantity <= 0 || fullRemove)
                itemsList.Remove(foundItem);
        }

        public void RemoveItem(ItemBase item, int qty = 1, bool fullRemove = false)
        {
            if (item == null)
            {
                Debug.LogWarning("Não é possível remover item Nulo");
                return;
            }

            RemoveItem(item.ItemName, qty, fullRemove);
        }

        public InventoryItem GetInventoryItem(string itemName)
        {
            var foundItem = itemsList.Find(i => i.item.ItemName.Equals(itemName));
            if(foundItem == null)
                Debug.LogWarning("você não tem o item " + itemName);
            
            return foundItem;
        }

        public InventoryItem GetInventoryItem(ItemBase item)
        {
            if (item == null)
            {
                Debug.LogWarning("Não é possível encontrar item Nulo");
                return null;
            }

            return GetInventoryItem(item.ItemName);
        }

        public ItemBase GetItem(string itemName)
        {
            return GetInventoryItem(itemName).item;
        }

        public int GetQty(string itemName)
        {
            return GetInventoryItem(itemName).quantity;
        }

        public int GetQty(ItemBase item)
        {
            if (item == null)
            {
                Debug.LogWarning("Não é possível encontrar item Nulo");
                return -1;
            }

            return GetInventoryItem(item).quantity;
        }

        public void UseItem(string itemName)
        {
            var myItem = GetInventoryItem(itemName);
            if (myItem == null)
            {
                Debug.LogWarning("Você não tem o item "+itemName);
                return;
            }

            myItem.UseItem();
            if (myItem.item.Consumable)
                myItem.quantity--;

            if (myItem.quantity <= 0)
                RemoveItem(itemName, fullRemove: true);
        }

        public void UseItem(ItemBase item)
        {
            if (item == null)
            {
                Debug.LogWarning("Não é possível usar item Nulo");
                return;
            }

            UseItem(item.ItemName);
        }
    }
}