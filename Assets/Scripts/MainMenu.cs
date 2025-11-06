
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public GameObject Wombat;
    public Vector3 start;
    public Vector3 end;

    
    [Range(1f, 5f)]
    [SerializeField] private float _wombatStrollSpeed;

    public Transform Camera1;
    public Material Daybox;
    public Material Nightbox;
    public Light MainLight;
    public GameObject Torch;
    public GameObject Money;
    public GameObject Fire;

    private readonly Quaternion  _creditsRotation = Quaternion.Euler(-45f, 0f, 0f);
    private readonly Quaternion _mainMenuRotation = Quaternion.Euler(20f, 0f, 0f);
    private readonly Quaternion _loadMenuRotation = Quaternion.Euler(90f, 0f, 0f);
    private bool isRotating = false;

    public GameObject Player;
    public enum Focus
    {
        MainMenu,
        Credits,
        LoadMenu
    }

    public Focus _currentFocus;

    AudioSource AudioSource;
    public AudioClip Day;
    public AudioClip Night;

    public TMP_Text[] LoadTexts;

    public TMP_Text AutoSaveText;
    // Start is called before the first frame update
    void Start()
    {
        start = Wombat.transform.position;
        end = start + new Vector3(-90f, 0f, 0f);

        AudioSource = GetComponent<AudioSource>();
        StartCoroutine(StrollingWombat());
        _currentFocus = Focus.MainMenu;
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void ChangeMode(bool daytime)
    {
        if(daytime)
        {
            RenderSettings.skybox = Daybox;
            MainLight.intensity = 0.4f;
            Torch.SetActive(false);
            AudioSource.PlayOneShot(Day);
            Money.SetActive(true);
            Fire.SetActive(false); 

        }
        else
        {
            RenderSettings.skybox= Nightbox;
            MainLight.intensity = 0.1f;
            Torch.SetActive(true);
            Money.SetActive(false);
            Fire.SetActive(true);
            AudioSource.PlayOneShot(Night);
        }

    }

    public void NewGame(string scene)
    {
        Player.gameObject.SetActive(true);
        StartCoroutine(ChangeScene(scene));
    }
    private IEnumerator ChangeScene(string scene)
    {
        Player.gameObject.SetActive(true);
        var loadScene = SceneManager.LoadSceneAsync(scene);
        yield return new WaitUntil(() => loadScene.isDone);
        GameObject mb = MoneyBag.Instance.gameObject;
        Progress.Instance.MoneyBag = mb;
        GameManager.Instance.SetMoney();
    }
    public void Quit()
    {
        Application.Quit();
        //UnityEditor.EditorApplication.ExitPlaymode();
    }

    // Brand new function name, never seen before
    IEnumerator StrollingWombat()
    {
        //Wombat.transform.position = Vector3.MoveTowards(start, end, Time.deltaTime * 0.2f);
        while(true)
        {
            yield return new WaitForSecondsRealtime(Random.Range(1f, 10f));
            while (Vector3.Distance(Wombat.transform.position, end) > 1f)
            {
                Wombat.transform.position = Vector3.MoveTowards(Wombat.transform.position, end, Time.deltaTime * 8f);
                yield return null;
            }
            yield return new WaitForSecondsRealtime(Random.Range(1f, 10f));
            Wombat.transform.position = end;
            // Grab both the wombat and man to rotate them individually to give the impression the tables turned
            // and now the wombat chases the man
            GameObject child1 = Wombat.transform.GetChild(0).gameObject;
            GameObject child2 = Wombat.transform.GetChild(1).gameObject;

            child1.transform.localScale = new Vector3(-child1.transform.localScale.x, child1.transform.localScale.y, child1.transform.localScale.z);
            child2.transform.localScale = new Vector3(-child2.transform.localScale.x, child2.transform.localScale.y, child2.transform.localScale.z);


            while (Vector3.Distance(Wombat.transform.position, start) > 1f)
            {
                Wombat.transform.position = Vector3.MoveTowards(Wombat.transform.position, start, Time.deltaTime * 12f);
                yield return null;
            }

            child1.transform.localScale = new Vector3(child1.transform.localScale.x * -1f, child1.transform.localScale.y, child1.transform.localScale.z);
            child2.transform.localScale = new Vector3(child2.transform.localScale.x * -1f, child2.transform.localScale.y, child2.transform.localScale.z);
        }
        
    }

    public void ChooseCameraPan(int position)
    {
        // Can't seem to pass enum through Unity UI system
        _currentFocus = position switch
        {
            0 => Focus.MainMenu,
            1 => Focus.LoadMenu,
            2 => Focus.Credits,
            _ => Focus.MainMenu,
        };

        switch (_currentFocus)
        {
            case Focus.MainMenu:
                StartCoroutine(PanCamera(_mainMenuRotation));
                break;
            case Focus.Credits:
                StartCoroutine(PanCamera(_creditsRotation));
                break;
            case Focus.LoadMenu:
                StartCoroutine(PanCamera(_loadMenuRotation));
                break;
            default:
                Camera1.transform.rotation = _mainMenuRotation;
                break;
        }
    }
    private IEnumerator PanCamera(Quaternion target)
    {
        if (!isRotating)
        {
            //Debug.Log("rotating");
            isRotating = true;
            float elapsedTime = 0f;
            float rotationDuration = 1.0f;

            Quaternion startRotation = Camera1.transform.rotation;
            while (elapsedTime < rotationDuration)
            {
                Camera1.transform.rotation = Quaternion.Lerp(startRotation, target, elapsedTime / rotationDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            //Debug.Log("rotated");
            isRotating = false;
        }
    }

    public void UpdateLoadMenuUI()
    {
        for (int i = 0; i < GameManager.Instance.SaveFiles.Length ; i++)
        {
            // Check if the save data exists, UI should remain on empty
            if(!string.IsNullOrEmpty(GameManager.Instance.SaveFiles[i].DateTime))
                LoadTexts[i].text = $"{GameManager.Instance.SaveFiles[i].DateTime}\n${GameManager.Instance.SaveFiles[i].MoneyCollected} - {GameManager.Instance.SaveFiles[i].Scene} ";
        }
        if (!string.IsNullOrEmpty(GameManager.Instance.AutoSave.DateTime))
            AutoSaveText.text = $"{GameManager.Instance.AutoSave.DateTime}\n${GameManager.Instance.AutoSave.MoneyCollected} - {GameManager.Instance.AutoSave.Scene}";
    }
}
