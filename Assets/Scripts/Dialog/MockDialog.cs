using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Guizan.Dialog;
using Guizan.NPC;

public class MockDialog : MonoBehaviour
{
    [SerializeField]
    private DialogManager dialogManager;

    [SerializeField]
    private NPCConfigs configs;


    [Button]
    public void StartDialog()
    {
        dialogManager.gameObject.SetActive(true);
        dialogManager.InitializeDialog(configs);
    }
}
