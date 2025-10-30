using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using Guizan.LLM;
using NaughtyAttributes;
using Guizan.LLM.Agent;
using System.Collections.Generic;

public class UICallLLM : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField inputField;
    [SerializeField]
    private TextMeshProUGUI textBox;
    [SerializeField]
    private Button sendButton;
    [SerializeField]
    private Button startButton;

    [Space]
    [SerializeField]
    private AgentTalkManager agentTalk;


    private bool WaitResponse = false;

    private void OnEnable()
    {
        sendButton.onClick.AddListener(CallLLM);
        startButton.onClick.AddListener(ConversationToggle);
    }

    private void ConversationToggle()
    {
        if (agentTalk.InConversation)
        {
            agentTalk.EndConversation();
            StartCoroutine(SetStartBtnArt(false));
        }
        else
        {
            agentTalk.StartConversation();
            StartCoroutine(SetStartBtnArt(true));
        }
    }

    private IEnumerator SetStartBtnArt(bool status)
    {
        yield return new WaitUntil(()=> agentTalk.InConversation == status);
        ColorBlock cb = startButton.colors;
        TextMeshProUGUI buttonText = startButton.GetComponentInChildren<TextMeshProUGUI>();
        if (!status)
        {
            cb.normalColor = Color.white;
            cb.selectedColor = Color.white;
            buttonText.color = Color.black;
            buttonText.text = "Start";
        }
        else
        {
            cb.normalColor = Color.gray;
            cb.selectedColor = Color.gray;
            buttonText.color = Color.white;
            buttonText.text = "End";
        }
        startButton.colors = cb;
    }

    void Start()
    {
        textBox.text = "";
        inputField.text = "";
    }

    private void OnSubmit()
    {
        if (!WaitResponse)
        {
            CallLLM();
        }
    }

    private void CallLLM()
    {
        string text = inputField.text;
        if (text.Equals(string.Empty))
            return;

        // send data to script
        agentTalk.SendMessage(text, callback: PrintLLMAnswer);
        ChangeWaitResponse(true);
    }

    private void ChangeWaitResponse(bool val)
    {
        WaitResponse = val;
        sendButton.interactable = !val;
        if (!val) inputField.text = "";
    }

    private void PrintLLMAnswer(List<string> resp)
    {
        Debug.Log("recebi:\n" + resp[0]);
        string text = "";
        for (int i = 0; i < resp.Count; i++)
        {
            text += resp[i]+"\n";
        }
        textBox.text = text;
        ChangeWaitResponse(false);
    }
    private void OnDisable()
    {
        sendButton.onClick.RemoveListener(CallLLM);
        startButton.onClick.RemoveListener(ConversationToggle);
    }
}
