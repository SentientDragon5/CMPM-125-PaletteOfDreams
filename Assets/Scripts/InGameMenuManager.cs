using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class InGameMenuManager : MonoBehaviour
{
    public PlayerInput playerInput;
    public InputAction GetAct(string n) => playerInput.actions[n];
    public List<Canvas> menus;
    public int current_menu;

    public int progress_menu;

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
        current_menu = gameMenu;
        menus[gameMenu].enabled = true;
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void LoadSceneName(string s) => StartCoroutine(LoadAsync(s));

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

    public int gameMenu = 0;
    public int pauseMenu = 1;
    public void TogglePause()
    {
        if (current_menu == gameMenu)
        {
            Time.timeScale = 0;
            Cursor.lockState = CursorLockMode.Confined;
            OpenMenu(pauseMenu);
        }
        else
        {
            Time.timeScale = 1;
            Cursor.lockState = CursorLockMode.Locked;
            OpenMenu(gameMenu);
        }
    }

    private void OnEnable()
    {
        GetAct("Pause").performed += _ => TogglePause();
    }

    private void OnDisable()
    {
        if (playerInput != null)
        {
            GetAct("Pause").performed -= _ => TogglePause();
        }
    }
    public string mainMenu = "MainMenu";
    public void Save()
    {
        PlayerProgressManager.instance.SaveGame();
    }
    public void ExitToMain()
    {
        LoadSceneName(mainMenu);
    }
}
