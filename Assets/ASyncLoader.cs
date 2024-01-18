using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class ASyncLoader : MonoBehaviour
{
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private GameObject mainMenu;

    [SerializeField] private Slider loadingBar;

    public void LoadLevelButton(string levelToLoad) {
        mainMenu.SetActive(false);
        loadingScreen.SetActive(true);
        StartCoroutine(LoadLevelASync(levelToLoad));
    }

    IEnumerator LoadLevelASync(string levelToLoad) {
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(levelToLoad);
        
        while (!loadOperation.isDone)
        {
            float progressValue = Mathf.Clamp01(loadOperation.progress / 0.9f);
            loadingBar.value = progressValue;
            yield return null;
        }
    }
}
