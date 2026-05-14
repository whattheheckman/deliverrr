using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelectManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField seedInputField;
    [SerializeField] private string gameSceneName = "FirstLevel";

    // Called by the "Play" button
    public void StartGame()
    {
        if (seedInputField != null && int.TryParse(seedInputField.text, out int parsed))
            LevelSeedConfig.Seed = parsed;
        SceneManager.LoadScene(gameSceneName);
    }

    // Called by preset seed buttons — wire via UnityEvent with int argument
    public void StartGameWithSeed(int seed)
    {
        LevelSeedConfig.Seed = seed;
        SceneManager.LoadScene(gameSceneName);
    }
}
