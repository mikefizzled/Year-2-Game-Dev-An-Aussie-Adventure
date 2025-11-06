using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using static GameManager;

public class PauseMenu : MonoBehaviour
{
    public GameObject PauseObject;
    public GameObject PauseMainMenu;
    public GameObject SaveSubMenu;
    // Start is called before the first frame update

    public TMP_Text[] SaveTexts;
    public TMP_Text[] LoadTexts;
    public TMP_Text AutoSaveText;

    public void BackButton(GameObject current)
    {
        current.SetActive(false);
        PauseMainMenu.SetActive(true);
    }

    public void OpenSaveMenu()
    {
        SaveSubMenu.SetActive(true);
        PauseMainMenu.SetActive(false);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        Player.Instance.State = Player.PlayerState.Idle;
    }



    public void UpdateSaveUI()
    {
        for (int i = 0; i < GameManager.Instance.SaveFiles.Length; i++)
        {
            SaveTexts[i].text =
                $"{GameManager.Instance.SaveFiles[i].DateTime}\n${GameManager.Instance.SaveFiles[i].MoneyCollected} - {GameManager.Instance.SaveFiles[i].Scene} ";
            LoadTexts[i].text =
                $"{GameManager.Instance.SaveFiles[i].DateTime}\n${GameManager.Instance.SaveFiles[i].MoneyCollected} - {GameManager.Instance.SaveFiles[i].Scene} ";
        }
    }

    public void UpdateLoadMenuUI()
    {
        for (int i = 0; i < GameManager.Instance.SaveFiles.Length; i++)
        {
            // Check if the save data exists, UI should remain on empty
            if (!string.IsNullOrEmpty(GameManager.Instance.SaveFiles[i].DateTime))
                LoadTexts[i].text = $"{GameManager.Instance.SaveFiles[i].DateTime}\n${GameManager.Instance.SaveFiles[i].MoneyCollected} - {GameManager.Instance.SaveFiles[i].Scene} ";
        }
        if (!string.IsNullOrEmpty(GameManager.Instance.AutoSave.DateTime))
            AutoSaveText.text = $"{GameManager.Instance.AutoSave.DateTime}\n${GameManager.Instance.AutoSave.MoneyCollected} - {GameManager.Instance.AutoSave.Scene}";
    }

}
