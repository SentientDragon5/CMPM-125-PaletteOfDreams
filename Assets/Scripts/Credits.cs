using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using System.Text;

public class Credits : MonoBehaviour
{
    public GameObject template;
    public TextAsset credits;
    List<string> strings;

    void Awake()
    {
        char[] archDelim = new char[] { '\r', '\n' };
        strings = credits.text.Split(archDelim).ToList();
        template.GetComponent<TextMeshProUGUI>().text = strings[0];
        for (int i = 0; i < strings.Count; i++)
        {
            var g = Instantiate(template, transform);
            g.GetComponent<TextMeshProUGUI>().text = strings[i];
        }
    }

}