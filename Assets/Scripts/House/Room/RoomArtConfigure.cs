using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Guizan.House.Room
{
    [RequireComponent(typeof(RoomController))]
    public class RoomArtConfigure : MonoBehaviour
    {
        private RoomType myType;
        private RoomController controller;
        private void Awake()
        {
            controller = GetComponent<RoomController>();
        }

        private void Start()
        {
            myType = controller.RoomType;
            ChangeArtType();
        }

        private void ChangeArtType()
        {
            //Debug.Log($"Ajustando o quarto{gameObject.name} para o estilo : {myType}");
        }
    }
}