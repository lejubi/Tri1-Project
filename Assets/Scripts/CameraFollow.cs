using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;
    public float smoothSpeed = 0.125f;
    public Vector3 offset;

    void LateUpdate()
    {
        if (player == null) return;

        Vector3 newPos = player.position + offset;
        Vector3 smoothPos = Vector3.Lerp(transform.position, newPos, smoothSpeed);
        transform.position = new Vector3(smoothPos.x, smoothPos.y, transform.position.z);
    }
}