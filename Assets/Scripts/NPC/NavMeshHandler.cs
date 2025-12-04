using Game.Initialization;
using Guizan.House;
using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

[RequireComponent(typeof(NavMeshSurface))]
public class NavMeshHandler : MonoBehaviour
{
    private NavMeshSurface navMeshSurface;
    private void Awake()
    {
        InitializeHandler.SubscribeInitialization(this.GetType().Name);
        navMeshSurface = GetComponent<NavMeshSurface>();
    }
    private IEnumerator Start()
    {
        yield return new WaitUntil(()=> InitializeHandler.IsInitialized(typeof(RoomsTypeHandler).Name));
        navMeshSurface.BuildNavMesh();
        yield return new WaitForEndOfFrame();
        InitializeHandler.SetInitialized(this.GetType().Name);
    }
}
