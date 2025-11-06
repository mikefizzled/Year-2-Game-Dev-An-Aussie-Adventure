//using System;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class TransformationMenu : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject Menu;
    public GameObject playerObject;

    public List<GameObject> list = new List<GameObject>();

    private Emu emu;
    private Kangaroo kangaroo;
    private Bandicoot bandicoot;

    private int selection;

    private readonly Color  _selectionRed = new Color(0.8f, 0.133f, 0.161f, 1f);
    private readonly Color _notSelectedBlue = new Color(0.13f, 0.3f, 0.55f, 1f);

    public AudioClip ChooseEffect;
    public AudioClip BadChoice;
    [Header("Emu")]
    public RawImage emuImage;
    public TMP_Text emuText;

    [Header("Bandicoot")]
    public RawImage bandicootImage;
    public TMP_Text bandicootText;

    [Header("Kangaroo")]
    public RawImage kangarooImage;
    public TMP_Text kangarooText;

    [Header("Response Panels")]
    public GameObject AlreadyInForm;
    public GameObject FormNotUnlocked;
    public AudioSource audioSource;
    void Start()
    {
        emu = Player.Instance.GetComponent<Emu>();
        kangaroo = Player.Instance.GetComponent<Kangaroo>();
        bandicoot = Player.Instance.GetComponent<Bandicoot>();
        audioSource = GetComponent<AudioSource>();
        EmuCheck();
        BandicootCheck();
        KangarooCheck();
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.anyKey)
        {
            ResetWarning();
        }
        if(Input.GetKeyUp(KeyCode.W)||Input.GetKeyUp(KeyCode.D)||Input.GetKeyUp(KeyCode.S)||Input.GetKeyUp(KeyCode.A)){
            
            //AudioSource.PlayClipAtPoint(ChooseEffect, _player.transform.position);
            audioSource.pitch = Random.Range(0.8f, 1.2f);
            audioSource.clip = ChooseEffect;
            audioSource.Play();
        }
        if (Input.GetKeyUp(KeyCode.W)){
            selection = 0;
        }

        if (Input.GetKeyUp(KeyCode.D)){
            selection = 1;
        }
            
        if (Input.GetKeyUp(KeyCode.S))
            selection = 2;
        if (Input.GetKeyUp(KeyCode.A)){
            selection = 3;
        }

        if (selection > -1)
            
            PanelChange(selection);
        if (Input.GetKeyUp(KeyCode.Space))
        {
            if (Player.Instance.ChangeAnimal(selection))
            {
                CloseMenu();
            }
            else
            {
                audioSource.clip = BadChoice;
                audioSource.Play();
                if (Player.Instance.Current().Name == Player.Instance.CheckCurrent(selection))
                {
                    AlreadyInForm.SetActive(true);
                }
                else
                {
                    FormNotUnlocked.SetActive(true);
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            // reset bool for updating menu
            CloseMenu();
        }
    }

    private void OnEnable()
    {
        EmuCheck();
        BandicootCheck();
        KangarooCheck();

    }

    
    public void EmuCheck()
    {
        if (Progress.Instance.CheckUnlockedStatus("Emu"))
        {
            emuImage.color = Color.white;
            emuText.text = emu.Name;
        }
        else
        {
            emuImage.color = Color.black;
            emuText.text = "???";
        }
    }
    public void BandicootCheck()
    {
        if (Progress.Instance.CheckUnlockedStatus("Bandicoot"))
        {
            bandicootImage.color = Color.white;
            bandicootText.text = bandicoot.Name;
        }
        else
        {
            bandicootImage.color = Color.black;
            bandicootText.text = "???";
        }
    }
    public void KangarooCheck()
    {
        if (Progress.Instance.CheckUnlockedStatus("Kangaroo"))
        {
            kangarooImage.color = Color.white;
            kangarooText.text = kangaroo.Name;
        }   
        else
        {   
            kangarooImage.color = Color.black;
            kangarooText.text = "???";
        }
    }

    public void PanelChange(int selection)
    {
        foreach (var item in list)
        {
            if (list[selection] == item)
            {
                item.transform.GetComponent<UnityEngine.UI.Image>().color = _selectionRed;
            }
            else
            {
                item.transform.GetComponent<UnityEngine.UI.Image>().color = _notSelectedBlue;
            }
        }
    }
    public void CloseMenu()
    {
        AlreadyInForm.SetActive(false);
        FormNotUnlocked.SetActive(false);
        Menu.SetActive(false);
        Player.Instance.State = Player.PlayerState.Idle;
        Time.timeScale = 1f;
    }
    public void ResetWarning()
    {
        AlreadyInForm.SetActive(false);
        FormNotUnlocked.SetActive(false);
    }
}
