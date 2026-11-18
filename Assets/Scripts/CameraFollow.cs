using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] Transform playerTransform;
    float height = 4f; 
    float distance = -5f;
    
    void LateUpdate()
    {
        transform.position = playerTransform.position + new Vector3(0, height, distance);
    }
}
