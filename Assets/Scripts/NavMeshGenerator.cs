using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class GenerateNavMesh : MonoBehaviour
{
    // The NavMeshSurface component
    private NavMeshSurface navMeshSurface;

    private void Awake()
    {
        // Get or add the NavMeshSurface component to the game object
        navMeshSurface = GetComponent<NavMeshSurface>();
        if (navMeshSurface == null)
        {
            navMeshSurface = gameObject.AddComponent<NavMeshSurface>();
        }

        // Generate the NavMesh
        navMeshSurface.BuildNavMesh();
    }
}