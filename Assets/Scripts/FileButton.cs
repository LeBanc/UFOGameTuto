using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FileButton : MonoBehaviour {

    void Start()
    {
        GetComponent<Button>().onClick.AddListener(SelectFile);
    }

    void SelectFile()
    {
        FindObjectOfType<BuilderMenu>().SetFileToLoad(GetComponentInChildren<Text>().text);
    }

}
