using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using static GameManager;

public class Progress : MonoBehaviour
{
    [SerializeField] private int _money;
    public TMP_Text MoneyText;


    // exposing this in inspector seems awful, may resort to dumb bools instead?

    public Dictionary<string, bool> UnlockedAnimals = new Dictionary<string, bool> 
    {
        {"Human", true},
        {"Emu", false},
        {"Bandicoot", false},
        {"Kangaroo", false}
    };

    // Using singleton - this is forced by Awake
    public static Progress Instance { get; private set; }

    public GameObject MoneyBag;

    // SET TO FALSE WHEN THE MONEY UNLOCK IS INCORPORATED
    public bool Shapeshifter = false;

    public bool Cave = false;

    public bool Thylacine = false;
    // Start is called before the first frame update
    void Start()
    {

    }


    private void Awake()
    {
        // Force only a single instance

        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        UpdateMoneyUI();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddMoney(int value)
    {
        _money += value;
        UpdateMoneyUI();
    }
    public int CurrentMoney()
    {
        return _money;
    }
    public void UpdateMoneyUI()
    {
        MoneyText.text = "$" + _money.ToString();
    }
    public void UnlockAnimal(string name)
    {
        if (UnlockedAnimals[name] == false)
        {
            UnlockedAnimals[name] = true;
            Debug.Log("new unlock" + name);
        }
        else
        {
            Debug.Log("Already unlocked");
        }
    }

    public void LoadAnimalStatus(string animal, bool unlock){
        Debug.Log(animal  + " " + unlock);
        UnlockedAnimals[animal] = unlock;
    }
    public bool CheckUnlockedStatus(string name)
    {
        //Debug.Log("CheckUnlockedStatus: " + name + " " +unlockedAnimals[name]);
        return UnlockedAnimals[name];
    }

    public void SetActiveMoney()
    {
        GameObject mb = GameObject.FindGameObjectWithTag("MoneyBag");
        if (GameObject.FindGameObjectWithTag("MoneyBag") != null)
        {
            Debug.Log("MoneyBag found!");
        }

        else
        {
            Debug.Log("mb not found");
        }
        MoneyBag = mb;
        foreach (MoneyStatus m in GameManager.Instance.CurrentSave.MoneyState)
        {
            if (!m.active)
                MoneyBag.transform.Find(m.name).gameObject.SetActive(false);
        }
    }
    public void SetMoney(int money){
        _money = money;
    }
}
