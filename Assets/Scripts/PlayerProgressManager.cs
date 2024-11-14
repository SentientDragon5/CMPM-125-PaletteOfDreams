using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Saving.SaveUtil;
using UnityEngine.Events;

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
    }

    public string savePath = "save0.sv";

    public List<string> items;

    public Vector3 worldPosition;
    public Vector3 worldEuler;
    public string worldName;

    public bool inBattle = false;
    public string battleScene;

    public int hp;
    public int maxHp;
    public int strength;
    public int defense;

    // number of red paint?

    public class SaveFile
    {
        public string[] items;
        public float[] worldPosition;
        public float[] worldEuler;
        public string worldName;
        public int hp;
        public int maxHp;
        public int strength;
        public int defense;
    }
    public void SaveGame()
    {
        var save = new SaveFile();
        save.items = items.ToArray();
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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
