using UnityEditor;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
public class EditorPlayMode
{
    static EditorPlayMode()
    {
        EditorApplication.playModeStateChanged += OnPlayModeChanged;
    }

    private static void OnPlayModeChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.EnteredPlayMode)
        {
            if (SceneManager.GetActiveScene().name == "GameLoading") return;
            SceneManager.LoadScene("GameLoading");
        }
    }
}