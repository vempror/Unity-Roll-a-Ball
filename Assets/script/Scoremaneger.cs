using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public Transform player;            // The player’s transform
    public TextMeshProUGUI scoreText;   // UI Text for displaying score
    private float startZ;               // Starting Z position
    private int score = 0;

    void Start()
    {
        startZ = player.position.z;
    }

    void Update()
    {
        // Score based on distance traveled
        score = Mathf.FloorToInt(player.position.z - startZ);

        if (score < 0) score = 0; // prevent negative score

        scoreText.text = "Score: " + score;
    }
}
