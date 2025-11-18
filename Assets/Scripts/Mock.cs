using Guizan.House.Room;
using Guizan.Player.Inventory;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mock : MonoBehaviour
{
    public PlayerInventoryBinder my;
    public string itemName = "Item1";
    public int itemQty = 1;
    // Start is called before the first frame update
    IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();
    }

    [Button]
    public void DoIt()
    {
        my.Inventory.UseItem(itemName);
    }
}
