using Guizan.Item;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Guizan.Player.Inventory
{
    public class PlayerInventoryController : MonoBehaviour
    {
        [SerializeField, Expandable]
        private Item.Inventory myInventory;

        public Item.Inventory Inventory => myInventory;

        // Start is called before the first frame update
        void Start()
        {
            myInventory.InitializeIventory();
        }
    }
}