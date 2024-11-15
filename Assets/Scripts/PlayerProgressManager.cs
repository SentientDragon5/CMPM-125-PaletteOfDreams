using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Saving.SaveUtil;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class PlayerProgressManager : MonoBehaviour
{
    // This is for saving and loading as well as handling saved stuff between scenes.

    public static PlayerProgressManager instance;
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogError("Player progress was tried to be created twice.");
            Destroy(this);
        }
        DontDestroyOnLoad(gameObject);
    }

    public string savePath = "save0.sv";

    public List<string> items;
    // each enemy has an id, when battle entered. the enemy is called "defeated"
    public List<int> enemiesDefeated;

    public Vector3 worldPosition;
    public Vector3 worldEuler;
    public string worldName;

    public bool inBattle = false;
    public string battleScene;

    public int hp;
    public int maxHp;
    public float strength;
    public float defense;

    // number of red paint?

    public class SaveFile
    {
        public string[] items;
        public int[] enemiesDefeated;
        public float[] worldPosition;
        public float[] worldEuler;
        public string worldName;
        public int hp;
        public int maxHp;
        public float strength;
        public float defense;
    }
    public void SaveGame()
    {
        var save = new SaveFile();
        save.items = items.ToArray();
        save.enemiesDefeated = enemiesDefeated.ToArray();
        save.worldPosition = VectorToArr(worldPosition);
        save.worldEuler = VectorToArr(worldEuler);
        save.worldName = worldName;
        save.hp = hp;
        save.maxHp = maxHp;
        save.strength = strength;
        save.defense = defense;
        Save(save, savePath);
    }
    public void LoadGame()
    {
        if(Load(savePath, out SaveFile save))
        {
            items = new List<string>(save.items);
            enemiesDefeated = new List<int>(save.enemiesDefeated);
            worldPosition = ArrToVector3(save.worldPosition);
            worldEuler = ArrToVector3(save.worldEuler);
            worldName = save.worldName;
            hp = save.hp;
            maxHp = save.maxHp;
            strength = save.strength;
            defense = save.defense;

            onLoad.Invoke();
        }
        else
        {
            Debug.Log("Failed to load!");
        }
    }
    public UnityEvent onLoad;

    public void NewGame()
    {
        items = new();
        enemiesDefeated = new();
        worldPosition = Vector3.zero;
        worldEuler = Vector3.zero;
        worldName = "L0-Hall";
        hp = 12;
        maxHp = 12;
        strength = 1;
        defense = 1;

        onLoad.Invoke();
    }
}
