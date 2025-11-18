using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Guizan.Item
{
    public enum ItemType
    {
        Default,
        Key,
        Cloth
    }

    [CreateAssetMenu(fileName ="NewItem", menuName ="Item/ItemBase")]
    public class ItemBase : ScriptableObject
    {
        [SerializeField]
        private string myName = "item";
        [SerializeField, ResizableTextArea]
        private string myDescription = "É um item comum";
        [SerializeField]
        private ItemType type = ItemType.Default;
        [SerializeField]
        private bool consumable = false;
        [SerializeField]
        private Sprite image;

        public string ItemName => myName;
        public string Description => myDescription;
        public ItemType Type => type;
        public bool Consumable => consumable;
        public Sprite Image => image;

        public virtual void Use() { Debug.Log("Usou o Item " + ItemName); }
    }
}