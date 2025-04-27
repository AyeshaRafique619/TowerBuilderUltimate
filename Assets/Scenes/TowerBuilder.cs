using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TowerBuilder : MonoBehaviour
{
    [Header("Block Settings")]
    [SerializeField] private GameObject blockPrefab;
    [SerializeField] private float spawnOffset = 0.3f;
    [SerializeField] private Vector2 minMaxBlockScale = new Vector2(1.7f, 2.3f);
    [SerializeField] private Color[] blockColors;

    [Header("Physics Settings")]
    [SerializeField] private float swayForce = 1.0f;
    [SerializeField] private float maxTiltAngle = 30.0f;
    [SerializeField] private float swayHeightMultiplier = 0.1f;

    [Header("Game Settings")]
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TMP_Text finalScoreText;

    [Header("Audio Settings")]
    [SerializeField] private AudioSource backgroundMusic;


    private List<GameObject> towerBlocks = new List<GameObject>();
    private GameObject lastPlacedBlock;
    private int score = 0;
    private bool gameOver = false;

    void Start()
    {
        // Initialize game
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        UpdateScoreText();
    }

    void Update()
    {
        // Process touch input when game is active
        if (!gameOver)
        {
            ProcessTouchInput();
            CheckTowerStability();
            ApplySwayForce();
        }
    }


    void ProcessTouchInput()
    {
        // For mobile platforms, use touch input
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            PlaceBlock(Input.GetTouch(0).position);
            return; // Important! Exit the method after handling touch
        }

        // For editor/desktop testing, use mouse input
#if UNITY_EDITOR || UNITY_STANDALONE
        if (Input.GetMouseButtonDown(0))
        {
            PlaceBlock(Input.mousePosition);
        }
#endif
    }


    void PlaceBlock(Vector2 screenPosition)
    {
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            // Check if we hit a valid surface (ground or another block)
            if (hit.collider.CompareTag("Block") || hit.collider.CompareTag("Ground"))
            {
                Vector3 spawnPosition = hit.point + new Vector3(0, spawnOffset, 0);

                // Create the new block
                GameObject newBlock = Instantiate(blockPrefab, spawnPosition, Quaternion.identity);
                ConfigureBlock(newBlock);

                towerBlocks.Add(newBlock);
                lastPlacedBlock = newBlock;

                // Increment score
                score++;
                UpdateScoreText();
            }
        }
    }

    void ConfigureBlock(GameObject block)
    {
        // Tag the block
        block.tag = "Block";

        // Randomize the block properties
        RandomizeBlockProperties(block);
    }

    void RandomizeBlockProperties(GameObject block)
    {
        // Random scale
        float randomScale = Random.Range(minMaxBlockScale.x, minMaxBlockScale.y);
        block.transform.localScale = new Vector3(randomScale, 0.3f, 1f);

        // Random color
        if (blockColors != null && blockColors.Length > 0)
        {
            Renderer renderer = block.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = blockColors[Random.Range(0, blockColors.Length)];
            }
        }

        // Perlin noise based rotation for subtle variation
        float noise = Mathf.PerlinNoise(Time.time, 0) * 10f;
        block.transform.Rotate(0, noise, 0);
    }

    void ApplySwayForce()
    {
        if (towerBlocks.Count == 0) return;

        for (int i = 0; i < towerBlocks.Count; i++)
        {
            GameObject block = towerBlocks[i];
            if (block == null) continue;

            Rigidbody rb = block.GetComponent<Rigidbody>();
            if (rb != null)
            {
                // Calculate sway force that increases with height
                float heightFactor = (i + 1) * swayHeightMultiplier;
                float swayX = Mathf.Sin(Time.time) * swayForce * heightFactor * Time.deltaTime;
                float swayZ = Mathf.Cos(Time.time * 0.8f) * swayForce * heightFactor * Time.deltaTime;

                rb.AddForce(new Vector3(swayX, 0, swayZ), ForceMode.Force);
            }
        }
    }

    void CheckTowerStability()
    {
        if (lastPlacedBlock == null || towerBlocks.Count == 0) return;

        // Get the current up direction of the top block
        Vector3 blockUpDirection = lastPlacedBlock.transform.up;

        // Calculate angle between world up and block up
        float tiltAngle = Vector3.Angle(Vector3.up, blockUpDirection);

        // Check if tilt exceeds the maximum allowed angle
        if (tiltAngle > maxTiltAngle)
        {
            StartCoroutine(CollapseTower());
        }
    }

    IEnumerator CollapseTower()
    {
        gameOver = true;


        // Optional: Slow motion effect
        Time.timeScale = 0.5f;

        // Make all blocks fall freely
        foreach (GameObject block in towerBlocks)
        {
            if (block != null)
            {
                Rigidbody rb = block.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    // Add random explosion force
                    rb.AddExplosionForce(Random.Range(5f, 10f),
                                        block.transform.position + Random.insideUnitSphere,
                                        5f);
                }
            }
        }

        // Wait before showing game over
        yield return new WaitForSecondsRealtime(2f);

        // Reset time scale
        Time.timeScale = 1f;

        // Show game over panel
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            if (finalScoreText != null)
                finalScoreText.text = "Final Height: " + score;
        }

        if (backgroundMusic != null)
            backgroundMusic.Stop();
    }

    void UpdateScoreText()
    {
        if (scoreText != null)
            scoreText.text = "Height: " + score;
    }

    public void RestartGame()
    {
        // Remove all blocks
        foreach (GameObject block in towerBlocks)
        {
            if (block != null)
                Destroy(block);
        }

        // Clear list and reset variables
        towerBlocks.Clear();
        lastPlacedBlock = null;
        score = 0;
        gameOver = false;

        // Hide game over panel
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        // Reset score
        UpdateScoreText();
        if (backgroundMusic != null)
            backgroundMusic.Play();
    }
}