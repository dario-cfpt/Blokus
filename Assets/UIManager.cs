using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    private const string PAUSE_TAG = "ShowOnPause";
    private const string PLAYERS_SCENE_NAME = "PlayerSelectionScene";
    private GameObject[] pauseObjects;

    public Grid MainGrid;
    public GameObject MainPanel;
    public GameObject ListPlayerInfo;

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1;
        pauseObjects = GameObject.FindGameObjectsWithTag(PAUSE_TAG);
        ShowPauseScreen(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (Time.timeScale == 1) {
                Time.timeScale = 0;
                ShowPauseScreen(true);
            } else if (Time.timeScale == 0) {
                Time.timeScale = 1;
                ShowPauseScreen(false);
            }
        }
    }

    public void ShowPauseScreen(bool isActive) {
        foreach (GameObject g in pauseObjects) {
            g.SetActive(isActive);
        }
        MainGrid.gameObject.SetActive(!isActive);
        MainPanel.SetActive(!isActive);
        ListPlayerInfo.SetActive(!isActive);
    }

    public void ResetGame() {
        SceneManager.LoadScene(PLAYERS_SCENE_NAME);
    }

    public void ExitGame() {
        Application.Quit();
    }
}
