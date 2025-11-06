using Guizan.House.Room;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mock : MonoBehaviour
{
    public RoomController my;
    public RoomController other;
    // Start is called before the first frame update
    IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();
        my.AdaptColliderPointsToRight(other);
    }
}
