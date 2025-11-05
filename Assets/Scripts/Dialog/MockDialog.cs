using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Guizan.Dialog;
using Guizan.NPC;

public class MockDialog : MonoBehaviour
{

    [SerializeField]
    private NPCConfigs configs;


    [Button]
    public void StartDialog()
    {
        
        DialogManager.InitializeDialog(configs);
    }
}
