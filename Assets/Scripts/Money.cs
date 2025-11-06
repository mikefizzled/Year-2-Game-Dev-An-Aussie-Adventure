using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Money : MonoBehaviour
{
    // Start is called before the first frame update
    private const float RotationSpeed = 90f;
    [SerializeField]private int _value;
    public AudioClip Pickup;
    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up * RotationSpeed * Time.deltaTime, Space.World);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {            
            Progress.Instance.AddMoney(_value);
            AudioSource.PlayClipAtPoint(Pickup, transform.position);
            gameObject.SetActive(false);
        }
    }

    public int MoneyValue()
    {
        return _value;
    }
}
