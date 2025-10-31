using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Guizan
{
    public interface IInteractable
    {
        public void OnPointerOver(bool val);
        public void Interact();
    }
}