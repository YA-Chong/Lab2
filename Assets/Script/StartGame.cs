using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{
    // game start
    public void StartGameLogic()
    {
        SceneManager.LoadScene("GameScence");

    }

    // quit game
    public void QuitGame()
    {
        Debug.Log("Game is quitting...");

        Application.Quit();

        // test in editor
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
