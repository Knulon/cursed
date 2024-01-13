using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathMenu : MonoBehaviour
{
    [SerializeField] GameObject deathScreen;
    // Start is called before the first frame update
    public void ActivateDeath()
    {
        deathScreen.SetActive(true);
        Time.timeScale = 0;
    }
}
