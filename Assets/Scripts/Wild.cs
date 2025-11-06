using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;
using Quaternion = UnityEngine.Quaternion;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;

public class Wild : MonoBehaviour
{
    public float RoamDistance;
    public Vector3 StartingLocation;
    public Vector3 TargetDestination;
    public Vector3 EscapeDestination;
 
    private Transform _player;
    private Animal a;
    private Rigidbody rb;
    public LayerMask WorldLayer;
    
    public float WaitTimer = 1f;
    public float CurrentWait = 0f;

    private Animator _animator;
    public enum Status
    {
        Idle,
        Exploring,
        Returning,
        Escaping
    }

    public Status CurrentStatus;
private void Start()
    {
        _player = Player.Instance.transform;
        a = GetComponentInParent<Animal>();

        StartingLocation = transform.position;

        rb = GetComponent<Rigidbody>();
        NewTargetDestination();
        if (GetComponent<Animator>())
        {
            _animator = GetComponent<Animator>();
        }
    }

    // Update is called once per frame
private void Update()
{  
    switch (CurrentStatus)
    {
        case Status.Idle:
            CurrentWait += Time.deltaTime;
            if (_animator)
            {
                _animator.SetTrigger("Idle");
            }
            if (CurrentWait > WaitTimer)
            {
                CurrentWait = 0f;
                WaitTimer = Random.Range(1f, 3f);
                float playerDistance = Vector3.Distance(transform.position,Player.Instance.transform.position);
                
                // Begin Escape
                if (playerDistance < 15f 
                    && Player.Instance.Current().GetType() != a.GetType())
                {
                    CurrentStatus = Status.Escaping;
                    Escape();
                }
                
                // Begin Return
                else if (Vector3.Distance(StartingLocation, transform.position) > RoamDistance)
                {
                    CurrentStatus = Status.Returning;
                    Return();
                }
                
                // Begin Explore
                else
                {
                    CurrentStatus = Status.Exploring;
                    Explore();
                }
            }

            break;

            case Status.Escaping:
                Escape();
                break;

            case Status.Returning:
                Return();
                break;

            case Status.Exploring:
                Explore();
                break;
    }
}

private void NewTargetDestination()
{
    Vector3 random = new(Random.Range(-10f, 10f), 0f, Random.Range(-10f, 10f));
    TargetDestination = transform.position + random;
    RayCastDestination(true);
}

private void RayCastDestination(bool explore)
{
    float distance = Vector3.Distance(transform.position, TargetDestination);

    Vector3 direction = Vector3.zero;
    if(explore) 
        direction = (TargetDestination - transform.position).normalized;
    else
    {
        direction = (EscapeDestination - transform.position).normalized;
    }

    RaycastHit[] hits = Physics.RaycastAll(transform.position, direction, distance, WorldLayer);
    
    
    if (hits.Length > 0)
    {
        CurrentStatus = Status.Returning;
        Debug.DrawRay(transform.position, direction * distance, Color.red, 3f);
    }
    else
    {
        if (explore)
            Debug.DrawRay(transform.position, direction * distance, Color.green, 3f);
        else
            Debug.DrawRay(transform.position, direction * distance, Color.yellow, 3f);
        }
}
private void NewEscapeDestination()
{
    Vector3 escape = transform.position - _player.position;
    escape.y = 0;
    escape.Normalize();
    const float escapeDistance = 10f;
    EscapeDestination = transform.position + escape * escapeDistance;
    RayCastDestination(false);
}


private void Explore()
{
    if (Vector3.Distance(transform.position, TargetDestination) < 1f)
    {
        CurrentStatus = Status.Idle;
        
        ResetRB();
        NewTargetDestination();

    }
    else if(TargetDestination == Vector3.zero)
        NewTargetDestination();

    if (_animator)
    {
        _animator.SetTrigger("Still");
    }

    a.WalkSound();
    Vector3 directionToTarget = (TargetDestination - transform.position).normalized;
    Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
    transform.rotation = targetRotation;

    transform.position = Vector3.MoveTowards(transform.position, TargetDestination, a.WalkSpeed/2 * Time.deltaTime);
}
private void Escape()
{
    CurrentStatus = Status.Escaping;
    a.WalkSound();
    if (EscapeDestination == Vector3.zero)
    {
        NewEscapeDestination();
        if (_animator)
        {
            _animator.SetTrigger("Still");
        }
        }
    Quaternion rotationFromPlayer = Quaternion.LookRotation(EscapeDestination);
    transform.rotation = rotationFromPlayer;

    // Move towards the escape destination
    float step = a.SprintSpeed * Time.deltaTime;
    transform.position = Vector3.MoveTowards(transform.position, EscapeDestination, step);
    if (Vector3.Distance(transform.position, EscapeDestination) < 1f)
    {
        NewEscapeDestination();
        CurrentStatus = Status.Idle;
        ResetRB();
    }
}
private void Return()
{
    CurrentStatus = Status.Returning;
    a.WalkSound();
    Vector3 directionToTarget = (StartingLocation - transform.position).normalized;
    Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
    
    transform.rotation = targetRotation;
    transform.position = Vector3.MoveTowards(transform.position, StartingLocation, a.SprintSpeed * Time.deltaTime);
    if (Vector3.Distance(transform.position, StartingLocation) < 2f)
    {
        CurrentStatus = Status.Idle;
        ResetRB();
    }
}

private void ResetRB()
{
    rb.velocity = Vector3.zero;
    rb.angularVelocity = Vector3.zero;
    }
    
}
