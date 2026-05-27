using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject player;
    private Vector3 offset;

    void Start()
    {
 
        offset = player.transform.InverseTransformPoint(transform.position); 
    }

    void LateUpdate()
    {

        transform.position = player.transform.TransformPoint(offset); 


        transform.LookAt(player.transform); 
    }
}