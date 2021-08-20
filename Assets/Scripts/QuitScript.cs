using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class QuitScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Button quitButton = GetComponent<Button>();
        quitButton.onClick.AddListener(quitGame);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void quitGame()
    {
        Application.Quit();
    }
}
