using Guizan.Item;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Guizan.LLM.Agent
{
    [CreateAssetMenu(fileName ="NewInventoryInjector", menuName ="Injectors/Inventory")]
    public class TalkInjectorInventory : TalkInjectorBinder
    {
        [SerializeField, ResizableTextArea]
        private string prompt = "Você interpreta um personagem de RPG que possui um inventário próprio.\r\nO inventário representa os itens que o personagem carrega consigo — aquilo que ele pode usar, oferecer ou mencionar durante a conversa.\r\n\r\nO conteúdo do inventário é pessoal e não precisa ser compartilhado com outros personagens.\r\nSe desejar, o personagem pode omitir ou até mentir sobre seus itens, desde que isso faça sentido para sua personalidade e para o contexto da história.\r\n\r\nAbaixo estão os itens do inventário, com suas descrições e quantidades atuais:\r\n";
        [SerializeField]
        private Inventory npcInventory;

        public override string GetTextToInject()
        {
            string returnPrompt = prompt;
            var fullInventory = npcInventory.FullList;
            foreach (var item in fullInventory) 
            {
                returnPrompt += $"{item.item.ItemName} ({item.quantity}X) : {item.item.Description}\n";
            }
            return returnPrompt;
        }
    }
}