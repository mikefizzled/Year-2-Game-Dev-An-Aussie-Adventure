using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{
    public Transform player;
    [SerializeField]private Vector3 offset;
    public float chaseSpeed;


    private float direction;
    [Range(0f, 1f)]
    public float rotationSpeed;
    public float radius;
    // Start is called before the first frame update
    void Start()
    {
        offset= transform.position - player.position;
    }
    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, player.position + offset, chaseSpeed * Time.deltaTime);
    }

    void LateUpdate()
    {

        if (Input.GetKey(KeyCode.Q))
        {
            direction = 1;
        }
        else if (Input.GetKey(KeyCode.E))
        {
            direction = -1;
        }
        else
        {
            direction = 0;
        }
        transform.position = player.position - (transform.forward * radius);

        transform.RotateAround(player.position, Vector3.up, direction * rotationSpeed);
    }
}
