using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] GameObject settingsCanvace;
    [SerializeField] GameObject mainCanvace;
    [SerializeField] GameObject player; 
    public void SwitchScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void StartGame()
    {
        SceneManager.LoadScene("RoomsScene");
        Time.timeScale = 1;
    }
    public void LoadSettings() {
        settingsCanvace.SetActive(true);
    }
    
    public void LoadSettingsMain() {
        mainCanvace.SetActive(false);
        settingsCanvace.SetActive(true);
    }

    public void CloseSettings(){
        settingsCanvace.SetActive(false);
        player.GetComponent<PlayerMoveScript>().updatePlayerPrefs();
    }
    
    public void CloseSettingsMain(){
        settingsCanvace.SetActive(false);
        mainCanvace.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ToggleFullScreen() 
    {
        Screen.fullScreen = !Screen.fullScreen;
    }
}
