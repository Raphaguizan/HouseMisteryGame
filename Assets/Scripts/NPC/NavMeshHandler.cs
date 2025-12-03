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
        navMeshSurface = GetComponent<NavMeshSurface>();
    }
    private IEnumerator Start()
    {
        yield return new WaitUntil(()=>InitializeHandler.Initialized);
        navMeshSurface.BuildNavMesh();
    }
}
