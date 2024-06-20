using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class PointCounter : MonoBehaviour
{
    public GameObject GameOverScreen;
    public int maxPoints = 4;
    private int currentPoints = 0;


    public TMP_Text pointText;

    private void Start()
    {
        UpdatePointText();
    }

    public void AddPoints(int pointsToAdd)
    {
        currentPoints += pointsToAdd;
        if (currentPoints >= maxPoints)
        {
            MissionComplete();
        }
        UpdatePointText();
    }

    void UpdatePointText()
    {
        pointText.text = currentPoints + "/" + maxPoints;
    }

    void MissionComplete()
    {
        pointText.text = "Mission Complete!";
        // Reload the current scene after a delay
        GameOverScreen.SetActive(true);
        Invoke("ReloadScene", 5f);
    }

    void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
