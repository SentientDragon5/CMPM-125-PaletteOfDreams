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

    public string savePath = "save0.sv";

    public void OpenMenu(int index)
    {
        menus[current_menu].enabled = false;
        current_menu = index;
        menus[current_menu].enabled = true;
    }

    public Image progressBar;
    void Awake()
    {
        progressBar.transform.parent.parent.gameObject.SetActive(false);
    }

    private void Start()
    {
        if (FileExists(savePath))
        {
            print("Ok to continue!");
        }
    }

    public void LoadSceneName(string s) => StartCoroutine(LoadAsync(s));

    public void Quit() => Application.Quit();

    IEnumerator LoadAsync(string s)
    {
        OpenMenu(progress_menu);
        progressBar.transform.parent.parent.gameObject.SetActive(true);
        AsyncOperation op = SceneManager.LoadSceneAsync(s);
        while (!op.isDone)
        {
            float progress = Mathf.Clamp01(op.progress / 0.9f);
            print(progress);
            progressBar.fillAmount = progress;
            yield return null;
        }
        progressBar.transform.parent.parent.gameObject.SetActive(false);
    }

    public void LoadGame()
    {
        LoadSceneName(game_scene);
    }

    public void NewGame()
    {
        LoadSceneName(game_scene);
    }
}