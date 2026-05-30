using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : SingletonMono<GameManager>
{
    public GameObject StopPanel;
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            ShowStopPanel();
        }
    }
    private void ShowStopPanel()
    {
        StopPanel.gameObject.SetActive(true);
    }
}
