using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public SaveFile[] SaveFiles;

    public SaveFile AutoSave;

    public SaveFile CurrentSave;

    public GameObject PlayerObject;

    public GameObject PlayerCamera;

    [System.Serializable]
    public class SaveFile
    {
        
        public string DateTime;
        public string Scene;
        public Vector3 Location;
        public int MoneyCollected;
        public string CurrentForm;
        public bool ShapeshifterUnlocked;
        public bool HumanUnlock;
        public bool KangarooUnlock;
        public bool EmuUnlock;
        public bool BandicootUnlock;
        public List<MoneyStatus> MoneyState;

        
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void Awake()
    {
        DontDestroyOnLoad(PlayerObject);
        DontDestroyOnLoad(PlayerCamera);

        DontDestroyOnLoad(this);
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
        SaveFiles = new SaveFile[] { new(), new()};
        AutoSave = new SaveFile();
    }

    public void SaveGame(int chosenSlot)
    {
        CreateSaveFile(CollectSaveData(), chosenSlot);
        StartCoroutine(DisplaySaveIcon());
    }

    private string CollectSaveData()
    {
        List<MoneyStatus> moneyList = new List<MoneyStatus>();
        Progress.Instance.MoneyBag = GameObject.FindGameObjectWithTag("MoneyBag");
        foreach (Transform moneyObject in Progress.Instance.MoneyBag.transform)
        {
            var moneyStatus = new MoneyStatus
            {
                name = moneyObject.gameObject.name,
                active = moneyObject.gameObject.activeSelf
            };
            moneyList.Add(moneyStatus);
        }

        var saveData = new SaveFile()
        {
            DateTime = DateTime.Now.ToString("dd-MM-yyyy HH:mm"),
            ShapeshifterUnlocked = Progress.Instance.Shapeshifter,
            MoneyCollected = Progress.Instance.CurrentMoney(),
            MoneyState = moneyList,
            CurrentForm = Player.Instance.Current().Name,
            HumanUnlock = Progress.Instance.CheckUnlockedStatus("Human"),
            BandicootUnlock = Progress.Instance.CheckUnlockedStatus("Bandicoot"),
            EmuUnlock = Progress.Instance.CheckUnlockedStatus("Emu"),
            KangarooUnlock = Progress.Instance.CheckUnlockedStatus("Kangaroo"),
            Location = Progress.Instance.transform.position,
            Scene = SceneManager.GetActiveScene().name
        };
        var jsonSave = JsonUtility.ToJson(saveData, true);
        return jsonSave;
    }

    private void CreateSaveFile(string saveData, int saveSlot)
    {
        // Using the concept of indexes >= 0 to refer to the array of manual saves, otherwise use autosave (-1)
        var filename = (saveSlot >= 0) ? $"AnAussieAdventure{saveSlot}.txt" : "AnAussieAdventure-Autosave.txt";
        var saveLocation = SaveLocation();
        // Gives the full filename and directory
        // Example: c:/users/Example/Appdata/LocalLow/MikefizzledGames/Save/AnAussieAdventure1.txt
        var filePath = Path.Combine(saveLocation, filename);

        using var writer = new StreamWriter(filePath);
        writer.Write(saveData);
    }

    private SaveFile ChooseSaveFile(int saveSlot)
    {
        // Return the corresponding array file if >0, else return the autosave (-1)
        return (saveSlot >= 0) ? SaveFiles[saveSlot] : AutoSave;
    }
    public string SaveLocation()
    {
        var savePath = Application.persistentDataPath;
        var subFolder = Path.Combine(savePath, "Save");
        if (!Directory.Exists(subFolder))
        {
            Directory.CreateDirectory(subFolder);
            Debug.Log("Created Folder");
        }
        return subFolder;
    }


    public void LoadSaveData(SaveFile save)
    {
        CurrentSave = save;
        // Set Scene
        if (SceneManager.GetActiveScene().name != save.Scene)
        {
            StartCoroutine(SwapScene(save.Scene));
            Debug.Log("NExt");
        }
        else
        {
            Debug.Log($"Already on scene: {save.Scene}");
            GameManager.Instance.SetMoney();
        }


        PlayerObject.SetActive(true);
        PlayerCamera.SetActive(true);
        // Set Location of Player
        PlayerObject.transform.position = save.Location;

        // Set Player's previous transformation
        Player.Instance.SetAnimal(save.CurrentForm);

        // Unlock the shaftershifting power
        Progress.Instance.Shapeshifter = save.ShapeshifterUnlocked;

        // Unlock all previously unlocked animals         
        Progress.Instance.LoadAnimalStatus("Human", save.HumanUnlock);
        Progress.Instance.LoadAnimalStatus("Kangaroo", save.KangarooUnlock);
        Progress.Instance.LoadAnimalStatus("Bandicoot", save.BandicootUnlock);
        Progress.Instance.LoadAnimalStatus("Emu", save.EmuUnlock);

        Progress.Instance.SetMoney(save.MoneyCollected);

        Debug.Log(save.Location);
        PlayerObject.transform.position = save.Location;
        Progress.Instance.UpdateMoneyUI();
    }
    public void LoadFile(int chosen)
    {
        SaveFile save = ChooseSaveFile(chosen);

        if (save.DateTime != null)
        {
            GameManager.Instance.LoadSaveData(save);
        }
        else
        {
            Debug.Log("Save does not exist");
        }
    }

    public void CheckPreExistingFiles()
    {
        string path = GameManager.Instance.SaveLocation();
        var directoryInfo = new DirectoryInfo(path);
        FileInfo[] files = directoryInfo.GetFiles();

        List<FileInfo> manualFiles = new List<FileInfo>();
        FileInfo autosave = null;
        foreach (FileInfo file in files)
        {
            if (file.Name.Contains("Autosave"))
            {
                autosave = file;
            }
            else
            {
                manualFiles.Add(file);
            }
        }

        for (int i = 0; i < manualFiles.Count; i++)
        {
            using (StreamReader reader = new StreamReader(manualFiles[i].FullName))
            {
                string json = reader.ReadToEnd();

                SaveFile file = JsonUtility.FromJson<SaveFile>(json);
                GameManager.Instance.SaveFiles[i] = file;
            }
        }
        if (autosave != null)
        {
            using (StreamReader reader = new StreamReader(autosave.FullName))
            {
                string json = reader.ReadToEnd();
                SaveFile file = JsonUtility.FromJson<SaveFile>(json);
                GameManager.Instance.AutoSave = file;
            }
        }
    }

    [System.Serializable]
    public struct MoneyStatus
    {
        public string name;
        public bool active;
    }

    private static IEnumerator SwapScene(string scene)
    {
        AsyncOperation loadScene = SceneManager.LoadSceneAsync(scene);
        yield return new WaitUntil(() => loadScene.isDone);
        Debug.Log(MoneyBag.Instance.enabled);
        //Progress.Instance.MoneyBag = GameObject.FindwGameObjectWithTag("MoneyBag");
        GameObject mb = MoneyBag.Instance.gameObject;
        Progress.Instance.MoneyBag = mb;
        GameManager.Instance.SetMoney();
    }

    public void SetMoney()
    {
        if (GameManager.Instance.CurrentSave != null)
        {
            foreach (MoneyStatus m in GameManager.Instance.CurrentSave.MoneyState)
            {
                Progress.Instance.MoneyBag.transform.Find(m.name).gameObject.SetActive(m.active);
            }
        }

    }

    public static IEnumerator DisplaySaveIcon()
    {
        Player.Instance.SaveIcon.gameObject.SetActive(true);
        Image icon = Player.Instance.SaveIcon.GetComponent<Image>();
       
        const float  displayTime = 4f;
        float elapsedTime = 0f;

        while (elapsedTime < displayTime)
        {
            float alpha = MathF.Sin(elapsedTime);
            icon.color = new Color(icon.color.r, icon.color.g, icon.color.b, alpha);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Player.Instance.SaveIcon.gameObject.SetActive(false);
        icon.color = new Color(icon.color.r, icon.color.g, icon.color.b, 255f);
    }
}