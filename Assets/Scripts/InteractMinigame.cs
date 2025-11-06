using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InteractMinigame : MonoBehaviour
{

    public GameObject UnlockUI;
    public TextMeshProUGUI UnlockPrompt;
    public TextMeshProUGUI UnlockTimer;
    public TextMeshProUGUI ReponseText;

    public AudioClip CorrectInput;
    public AudioClip WrongInput;

    private string _collidingAnimal;
    [SerializeField] private string _requiredInput;

    private static readonly string[] RandomInputs = { "W", "A", "S", "D", "Q", "E" };
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if((_collidingAnimal != null) & Player.Instance.IsInMinigame() == false & Progress.Instance.Shapeshifter)
            {
                Debug.Log("start coroutine");
                Player.Instance.SetMinigameStatus(true);
                _requiredInput = RandomInputLetter();
                StartCoroutine(AnimalInteraction(_collidingAnimal, _requiredInput));
            }
        }

        if (Player.Instance.IsInMinigame())
        {
            bool minigameUnlock = CheckInput(_requiredInput);
            if (minigameUnlock)
            {
                Progress.Instance.UnlockAnimal(_collidingAnimal);
                StopCoroutine(AnimalInteraction(_collidingAnimal, _requiredInput));
                UnlockUI.SetActive(false);
                StartCoroutine(DisplayTextResponse(true, _collidingAnimal));
                Time.timeScale = 1f;
                AudioSource.PlayClipAtPoint(CorrectInput, transform.position);
                Player.Instance.SetMinigameStatus(false);
            }
        }
    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Animal"))
        {
            _collidingAnimal = other.GetComponentInParent<Animal>().Name;
        }
    }
    public void OnTriggerExit(Collider other)
    {
        _collidingAnimal = null;
    }

    public IEnumerator AnimalInteraction(string animal, string requiredInput)
    {
        
        if (Progress.Instance.CheckUnlockedStatus(animal) == false)
        {
            Time.timeScale = 0f;
            UnlockUI.SetActive(true);
            float promptCountdown = 5f;

            UnlockPrompt.text = requiredInput;
            Debug.Log("Required: " + requiredInput);
            while (promptCountdown >= 0f)
            {
                UnlockTimer.text = promptCountdown.ToString();
                yield return new WaitForSecondsRealtime(1f);
                promptCountdown--;
            }
            UnlockUI.SetActive(false);
            yield return null;
        }
        else
        {
            Debug.Log("Already unlocked");
            StartCoroutine(DisplayTextResponse(false, animal));
            yield return null;
        }
    }

    public string RandomInputLetter()
    {
        int random = UnityEngine.Random.Range(0, RandomInputs.Length - 1);

        return RandomInputs[random];
    }
    public bool CheckInput(string randomPick)
    {
        string userInput = Input.inputString.ToUpper();
        //Debug.Log($"User Entered: {userInput} to match {randomPick}");
        if (userInput == randomPick)
        {
            return true;
        }
        else 
        {
            return false;
        }
    }

    public IEnumerator DisplayTextResponse(bool unlocked, string animal)
    {
        if (unlocked)
        {
            ReponseText.text = $"{animal} was unlocked!";
            yield return new WaitForSecondsRealtime(3f);
            ReponseText.text = "";

        }
        else
        {
            ReponseText.text = $"{animal} is already unlocked!";
            yield return new WaitForSecondsRealtime(3f);
            ReponseText.text = "";
        }
    }
}
