using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    Button newGame;
    Button continueGame;
    Button quitGame;

    private void Awake()
    {
        newGame = transform.GetChild(1).GetComponent<Button>();
        continueGame = transform.GetChild(2).GetComponent<Button>();
        quitGame = transform.GetChild(3).GetComponent<Button>();

        newGame.onClick.AddListener(NewGame);
        continueGame.onClick.AddListener(ContinueGame);
        quitGame.onClick.AddListener(QuitGame);
    }
    void NewGame()
    {
        PlayerPrefs.DeleteAll();
        SceneManager.LoadScene(3);
        //ת������
    }
    void ContinueGame()
    {
        //ת������,��ȡ����
    }
    void QuitGame()
    {
        Application.Quit();
        Debug.Log("�˳���Ϸ");
    }
}
