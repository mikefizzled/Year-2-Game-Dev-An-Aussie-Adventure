using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animal : MonoBehaviour
{
    private string _name;
    [SerializeField] private float _walkSpeed;
    [SerializeField] private float _sprintSpeed;
    [SerializeField] private float _jumpHeight;
    [SerializeField] private List<AudioClip> _clips = new();

    public AudioSource AnimalAudioSource;
    public string Name
    { get => _name; set => _name = value; }
    public float WalkSpeed 
    { get => _walkSpeed;  set => _walkSpeed = value;}
    public float SprintSpeed
    { get => _sprintSpeed;  set => _sprintSpeed = value; }
    public float JumpHeight
    { get => _jumpHeight; set => _jumpHeight = value; }
    public List<AudioClip> Clips
    { get => _clips; set => _clips = value; }

    public void WalkSound()
    {
        if (AnimalAudioSource.isPlaying)
            return;
        int clipIndex = Random.Range(0, Clips.Count);
        AudioClip clip = Clips[clipIndex];

        AnimalAudioSource.pitch = Random.Range(0.85f, 1.15f);

        AnimalAudioSource.clip = clip;
        AnimalAudioSource.Play();
    }
}

