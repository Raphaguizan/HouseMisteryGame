using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MockDialog : MonoBehaviour
{
    [SerializeField]
    private AgentDialogManager agentDialogManager;

    [SerializeField]
    private Sprite image;

    [SerializeField]
    private List<string> texts;

    private void Start()
    {
        agentDialogManager.InitializeDialog(image);
    }

    [Button]
    public void SendText()
    {
        agentDialogManager.ReceiveAnswer(texts);
    }
}
