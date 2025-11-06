using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class CameraRotation : MonoBehaviour
{
    public Transform player; // Reference to the player's Transform
    
    private float direction;
    [Range(0f, 1f)]
    public float rotationSpeed;
    public float radius;
    private void Start()
    {
    }
    void LateUpdate()
    {
        Quaternion desired = Quaternion.Euler(35f, player.eulerAngles.y, 0f);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, desired, rotationSpeed);

        transform.position = player.position - (transform.forward * radius);
        //transform.RotateAround(player.position, Vector3.up, direction * rotationSpeed);
    }
}
