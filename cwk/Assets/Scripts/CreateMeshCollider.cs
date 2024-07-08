using UnityEngine;

public class CreateMeshCollider : MonoBehaviour
{
    void Start()
    {
        // Check if the GameObject already has a MeshCollider
        MeshCollider meshCollider = gameObject.GetComponent<MeshCollider>();

        if (meshCollider == null)
        {
            // Add a MeshCollider if it doesn't exist
            meshCollider = gameObject.AddComponent<MeshCollider>();
        }
    }
}
