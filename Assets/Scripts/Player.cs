using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class Player : MonoBehaviour
{
    // Movement
    [Header("Current Attributes")]
    [SerializeField] private float _playerWalk;
    [SerializeField] private float _playerSprint;
    [SerializeField] private float _playerJump;


    private float horizontalInput;
    private float verticalInput;

    private Animal currentAnimal;

    // Interaction
    private bool _interacting = false;


    // Player model
    [Header("Player Model")]
    public GameObject playerModel;
    private MeshFilter currentMesh;
    public Renderer pm;
    private SphereCollider _sphereCollider;


    private float _rotationSpeed = 360f;

    // Audio
    [Header("Audio Parts")]
    public AudioClip EvilLaugh;
    public AudioClip PickupClip;
    public AudioSource audioSource;
    public AudioMixerSnapshot Shop, OutOfShop;
    
    // UI
    [Header("UI")]
    public GameObject InteractUI;
    public TextMeshProUGUI InteractText;
    public Button FirstButtonToSelect;
    public GameObject PauseMenu;
    public GameObject transformPanel;

    public Transform SaveIcon;


    // Singleton instance
    public static Player Instance { get; private set; }

    // Interface 
    private IInteractable interactable;

    // Player State
    public enum PlayerState
    {
        Idle,
        Walking,
        Sprinting,
        Jumping,
        Menu
    }
    public PlayerState State;

    // Rigidbody
    Rigidbody rb;
    Vector3 moveDirection;

    [System.Serializable]
    public class Models
    {
        // Keep public for nice name inspector
        [HideInInspector] public string Name;
        [SerializeField] private Mesh _model;
        [SerializeField] private float _colliderScale;
        [SerializeField] private Animal _animal;
        [SerializeField] private Material _material;

        public Models(string name)
        {
            Name = name;
        }
        public Animal GetAnimal()
        {
            return _animal;
        }
        public float GetRadius()
        {
            return _colliderScale;
        }
        public Mesh GetModel()
        {
            return _model;
        }
        public Material GetMaterial()
        {
            return _material;
        }
    }

    // Transformations
    public List<Models> Transformations = new()
    {
        new Models("Human"),
        new Models("Emu"),
        new Models("Bandicoot"),
        new Models("Kangaroo")
    };
    public float PlayerWalk
    {
        get => _playerWalk; 
        set => _playerWalk = value;
    }
    public float PlayerSprint
    {
        get => _playerSprint;
        set => _playerSprint = value;
    }
    public float PlayerJump
    {
        get=> _playerJump;
        set => _playerJump = value;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        currentMesh = playerModel.GetComponent<MeshFilter>();
        _sphereCollider = GetComponent<SphereCollider>();
        currentAnimal = Transformations[0].GetAnimal();

        PlayerWalk = currentAnimal.WalkSpeed;
        PlayerSprint = currentAnimal.SprintSpeed;
        PlayerJump = currentAnimal.JumpHeight;
        audioSource = GetComponent<AudioSource>();
        
    }
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    // Update is called once per frame
    void Update()
    {

        Ray ray = new Ray(transform.position, -transform.up);
        bool grounded = Physics.Raycast(ray, _sphereCollider.radius + 0.5f);

        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        MenuHandler();

        if (_interacting) 
        {
            if (Input.GetKeyDown(KeyCode.F)){
                CancelInteraction();
            }
        }

        if (Input.GetKeyDown(KeyCode.F) && interactable != null)
            interactable.Interaction();
        

        if (Input.GetKeyDown(KeyCode.Space) && State != PlayerState.Menu)
        {
            if (grounded)
            {
                rb.AddForce(Vector3.up * PlayerJump, ForceMode.Impulse);
            }
        }
    }
    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MenuHandler()
    {
        if (State != PlayerState.Menu)
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                State = PlayerState.Menu;
                transformPanel.SetActive(true);
                Time.timeScale = 0f;
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                State = PlayerState.Menu;
                PauseMenu.SetActive(true);
                Time.timeScale = 0f;
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                State = PlayerState.Idle;
                transformPanel.SetActive(false);
                Time.timeScale = 1f;
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                State = PlayerState.Idle;
                PauseMenu.SetActive(false);
                Time.timeScale = 1f;
            }
        }

    }
    private void MovePlayer()
    {
        // calculating direction
        // flip input for inverted negative movement - feels too weird
        //if(verticalInput < 0)
        //{
        //    horizontalInput *= -1;
        //}

        float speed = Input.GetKey(KeyCode.LeftShift) ? PlayerSprint : PlayerWalk;
        moveDirection = transform.forward * verticalInput + transform.right * horizontalInput;

        rb.AddForce(moveDirection.normalized * speed *5f, ForceMode.Force);
        
        if (moveDirection != Vector3.zero)
        {
            Quaternion playerRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
            playerModel.transform.rotation = Quaternion.RotateTowards(transform.rotation, playerRotation, _rotationSpeed);
            currentAnimal.WalkSound();
        }
        else
        {
            currentAnimal.AnimalAudioSource.Stop();
        }

    }

    public void SetAnimal(string animal)
    {
        int index = GetKeyIndex(animal);
        ChangeAnimal(index);
    }

    public int GetKeyIndex(string key)
    {
        var keysList = new List<string>
            (Progress.Instance.UnlockedAnimals.Keys);
        return keysList.IndexOf(key);
    }
    public bool ChangeAnimal(int selection)
    {
        bool swapped = false;
        if(currentAnimal != Transformations[selection].GetAnimal())
        {
            if (Progress.Instance.CheckUnlockedStatus(Transformations[selection].Name))
            {
                SwapToSelected(selection);
                swapped = true;
            }
        }
        PlayerWalk = currentAnimal.WalkSpeed;
        PlayerJump = currentAnimal.JumpHeight;
        PlayerSprint = currentAnimal.SprintSpeed;
        return swapped;
    }

    private void SwapToSelected(int selection)
    {
        currentAnimal = Transformations[selection].GetAnimal();
        currentMesh.mesh = Transformations[selection].GetModel();
        _sphereCollider.radius = Transformations[selection].GetRadius();
        pm.material = Transformations[selection].GetMaterial();
    }

    public Animal Current()
    {
        return currentAnimal;
    }

    public string CheckCurrent(int animal)
    {
        return animal switch
        {
            0 => "Human",
            1 => "Emu",
            2 => "Bandicoot",
            3 => "Kangaroo",
            _ => throw new ArgumentException("Invalid animal type."),
        };
    }

    public void OnTriggerEnter(Collider other)
    {
        switch (other.name)
        {
            case "ShopCollider":
                Shop.TransitionTo(1f);
                break;
            case "CaveCollider":
                Interaction("Shaman's Cave");
                break;
        }

    }
    public void OnTriggerExit(Collider collider)
    {
        switch(collider.name)
        {
            case "ShopCollider":
                OutOfShop.TransitionTo(1f);
                break;
            case "CaveCollider":
                AudioSource.PlayClipAtPoint(EvilLaugh, transform.position, 1f);
                break;
            case "Money":
                audioSource.PlayOneShot(PickupClip, 1f);
                break;
        }
    }

    public bool IsInMinigame()
    {
        return _interacting;
    }
    public void SetMinigameStatus(bool status)
    {
        _interacting = status;
    }
    private void Interaction(string type)
    {
        InteractUI.SetActive(true);
        FirstButtonToSelect.Select();
        switch (type)
        {
            case "Shaman's Cave":
                InteractText.text = "Enter the spooky cave?";
                _interacting = true;
                Time.timeScale = 0f;
                break;
        }
    }

    public void CancelInteraction()
    {
        InteractUI.SetActive(false);
        _interacting = false;
        Time.timeScale = 1f;
        Player.Instance.State = PlayerState.Idle;
    }
    
    public void ChangeScene()
    {
        string current = SceneManager.GetActiveScene().name;
        if(current == "Overworld"){
            StartCoroutine(SwapScene("Shaman's Cave"));
        }
        else
        {
            StartCoroutine(SwapScene("Overworld"));
        }
        
        CancelInteraction();
    }

    private IEnumerator SwapScene(string scene)
    {
        var loadScene = SceneManager.LoadSceneAsync(scene);
        yield return new WaitUntil(() => loadScene.isDone);
        transform.position = new Vector3(0f, 2f, 0f);
        CancelInteraction();
    }

    // Following a tutorial on YouTube as I know how powerful interfaces are but no idea where to start
    // https://youtu.be/lzE4tvdfTM8

    public void InteractionInstance(IInteractable other)
    {
        interactable = other;
    }

    public void ResetInteractionInstance()
    {
        interactable = null;
    }
}
