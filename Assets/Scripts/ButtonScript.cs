using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class ButtonScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Button btn = GetComponent<Button>();
    }

    public void _startNewGame()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void _exitToMenu() {
        SceneManager.LoadScene("Main Menu");
    
    }

    public void _quitGame()
    {
        //UnityEditor.EditorApplication.isPlaying = false;
        Application.Quit();
    }

    public void _resumeGame()
    {
        WorldScript.Instance.pauseGame();
    }

}
