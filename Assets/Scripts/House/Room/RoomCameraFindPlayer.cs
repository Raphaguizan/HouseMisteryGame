using Cinemachine;
using Guizan.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Guizan.House.Room
{
    [RequireComponent(typeof(CinemachineVirtualCamera))]
    public class RoomCameraFindPlayer : MonoBehaviour
    {
        private CinemachineVirtualCamera m_Camera;
        private void Awake()
        {
            m_Camera = GetComponent<CinemachineVirtualCamera>();
        }
        private void OnEnable()
        {
            var player = FindAnyObjectByType<PlayerMovement>();
            if (player != null)
                m_Camera.Follow = player.transform;
        }
    }
}