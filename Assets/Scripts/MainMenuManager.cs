using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static Saving.SaveUtil;

public class MainMenuManager : MonoBehaviour
{
    public List<Canvas> menus;
    public int current_menu;

    public int progress_menu;

    public string savePath = "/save0.sv";

    public Button continueButton;

    public void OpenMenu(int index)
    {
        menus[current_menu].enabled = false;
        current_menu = index;
        menus[current_menu].enabled = true;
    }

    public UnityEngine.UI.Extensions.UITorus progressBar;
    void Awake()
    {
        foreach (var m in menus)
            m.enabled = false;
        menus[current_menu].enabled = true;
    }

    private void Start()
    {
        if (!FileExists(savePath))
        {
            print("Not ok to continue!");
            continueButton.interactable = false;
        }
    }



    public void LoadSceneName(string s) => StartCoroutine(LoadAsync(s));

    public void Quit() => Application.Quit();

    IEnumerator LoadAsync(string s)
    {
        OpenMenu(progress_menu);
        AsyncOperation op = SceneManager.LoadSceneAsync(s);
        while (!op.isDone)
        {
            float progress = Mathf.Clamp01(op.progress / 0.9f);
            print(progress);
            progressBar.FillAmount = progress;
            yield return null;
        }
    }

    public void LoadGame()
    {
        PlayerProgressManager.instance.LoadGame();
        LoadSceneName(PlayerProgressManager.instance.worldName);
    }

    public void NewGame()
    {
        PlayerProgressManager.instance.NewGame();
        LoadSceneName(PlayerProgressManager.instance.worldName);
    }
}