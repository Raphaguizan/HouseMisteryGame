using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extensions
{
    public static void PrintList<T>(this List<T> myList, string listName = null)
    {
        listName ??= typeof(T).Name;
        string text = listName +":\n\n";

        foreach (var item in myList) 
        { 
            text += item.ToString()+"\n";
        }
        Debug.Log(text);
    }
}
