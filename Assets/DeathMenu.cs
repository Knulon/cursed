using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathMenu : MonoBehaviour
{
    [SerializeField] GameObject deathScreen;

    public void ActivateDeath()
    {
        deathScreen.SetActive(true);
        Time.timeScale = 0;
    }
}
