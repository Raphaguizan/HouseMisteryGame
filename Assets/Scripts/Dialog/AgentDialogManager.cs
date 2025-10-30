using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AgentDialogManager : MonoBehaviour
{
    [SerializeField]
    private Image npcPhoto;
    [SerializeField]
    private TextMeshProUGUI textBox;
    [SerializeField]
    private TextMeshProUGUI pageIndex;
    [SerializeField]
    private Button leftArrow;
    [SerializeField]
    private Button rightArrow;

    private List<string> pages;
    private int currentPage = 0;

    public void InitializeDialog(Sprite photo)
    {
        pages = new();
        currentPage = 0;
        npcPhoto.sprite = photo;
    }

    public void ReceiveAnswer(List<string> pages)
    {
        this.pages = pages;
        currentPage = 0;

        leftArrow.interactable = false;
        rightArrow.interactable = true;

        AdjustPageIndex();
        ShowTextByIndex();
    }

    private void ShowTextByIndex()
    {
        textBox.text = pages[currentPage];
    }
    private void AdjustPageIndex()
    {
        pageIndex.text = $"{currentPage+1}/{pages.Count}";
    }

    void OnEnable()
    {
        leftArrow.onClick.AddListener(LastPage);
        rightArrow.onClick.AddListener(NextPage);
    }


    private void NextPage()
    {
        if (currentPage >= pages.Count - 2)
            rightArrow.interactable = false;

        leftArrow.interactable = true;
        currentPage++;
        AdjustPageIndex();
        ShowTextByIndex();
    }
    private void LastPage()
    {
        if (currentPage <= 1)
            leftArrow.interactable = false;

        rightArrow.interactable = true;
        currentPage--;
        AdjustPageIndex();
        ShowTextByIndex();
    }

    private void OnDisable()
    {
        leftArrow.onClick.RemoveListener(LastPage);
        rightArrow.onClick.RemoveListener(NextPage);
    }
}
