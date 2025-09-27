using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float smoothTime = 0.3f;
    private Vector3 velocity = Vector3.zero;
    private Transform targetRoom;

    private void Update()
    {
        if (targetRoom != null)
        {
            Vector3 targetPos = new Vector3(targetRoom.position.x, targetRoom.position.y, transform.position.z);
            transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, smoothTime);
        }
    }

    public void MoveToNewRoom(Transform newRoom)
    {
        targetRoom = newRoom;
    }
}
