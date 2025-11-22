using Guizan.LLM.Agent;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Guizan.Dialog
{
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

        private List<Sprite> emoticonsList;
        private List<AgentEmoticons> currentEmoticonsList;
        private List<string> pages;
        private int currentPage = 0;

        public bool InLastPage => currentPage == pages.Count - 1;

        public void InitializeDialog(List<Sprite> emoticons)
        {
            emoticonsList = emoticons;
            pages = new();
            currentPage = 0;
            WaitingForAnswer();
        }

        public void ReceiveAnswer(List<string> pages, List<AgentEmoticons> emoticons)
        {
            bool activePagination = pages.Count > 1;
            this.pages = pages;
            this.currentEmoticonsList = emoticons;
            currentPage = 0;

            leftArrow.gameObject.SetActive(activePagination);
            rightArrow.gameObject.SetActive(activePagination);

            leftArrow.interactable = false;
            rightArrow.interactable = true;

            AdjustPageIndex();
            ShowTextByIndex();
        }

        public void WaitingForAnswer()
        {
            textBox.text = "...";
            SetEmoticonSprite(AgentEmoticons.thinking);
        }
        private void SetEmoticonSprite(AgentEmoticons emoticon)
        {
            Sprite newSprite = emoticon switch
            {
                AgentEmoticons.Happy => emoticonsList[1],
                AgentEmoticons.Sad => emoticonsList[2],
                AgentEmoticons.Angry => emoticonsList[3],
                AgentEmoticons.Surprised => emoticonsList[4],
                AgentEmoticons.thinking => emoticonsList[5],
                _ => emoticonsList[0],
            };

            npcPhoto.sprite = newSprite;
        }

        private void ShowTextByIndex()
        {
            textBox.text = pages[currentPage];
            AgentEmoticons emot = AgentEmoticons.Default;
            if (currentEmoticonsList.Count > 0)
                emot = currentEmoticonsList[currentPage];

            SetEmoticonSprite(emot);
        }
        private void AdjustPageIndex()
        {
            pageIndex.text = $"{currentPage + 1}/{pages.Count}";
        }
        
        void OnEnable()
        {
            leftArrow.onClick.AddListener(LastPage);
            rightArrow.onClick.AddListener(NextPage);
        }


        public void NextPage()
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
}