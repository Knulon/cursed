using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinMenu : MonoBehaviour
{
    [SerializeField] GameObject winScreen;

    public void ActivateWin()
    {
        winScreen.SetActive(true);
        Time.timeScale = 0;
    }
}
