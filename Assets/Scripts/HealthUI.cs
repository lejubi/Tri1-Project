using UnityEngine;
using System.Collections.Generic;

public class HealthUI : MonoBehaviour
{
    public PlayerController player;
    public GameObject heartPrefab;
    public GameObject emptyHeartPrefab;
    public Transform heartContainer;
    private List<GameObject> hearts = new List<GameObject>();
    public float spacing = 1f; 
    public Vector3 offset = new Vector3(2f, 2f, 1f); 
    public Camera mainCamera;

    void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;
        InitializeHearts();
    }

    void LateUpdate()
    {
        UpdateHeartPositions();
    }

    public void InitializeHearts()
    {
        foreach (var heart in hearts)
        {
            Destroy(heart);
        }
        hearts.Clear();

        for (int i = 0; i < player.maxHealth; i++)
        {
            GameObject heart = Instantiate(i < player.health ? heartPrefab : emptyHeartPrefab, heartContainer);
            hearts.Add(heart);
        }

        UpdateHeartPositions();
    }

    private void UpdateHeartPositions()
    {
        if (mainCamera == null || heartContainer == null) return;

        float viewportHeight = 2f * mainCamera.orthographicSize;
        float viewportWidth = viewportHeight * mainCamera.aspect;

        Vector3 topRight = mainCamera.transform.position + new Vector3(viewportWidth / 2, viewportHeight / 2, 1);

        Vector3 startPosition = topRight - offset;

        for (int i = 0; i < hearts.Count; i++)
        {
            if (hearts[i] == null) continue;

            Vector3 heartPosition = startPosition - new Vector3(i * spacing, 0, 1);
            
            hearts[i].transform.position = heartPosition;

            hearts[i].transform.forward = -mainCamera.transform.forward;
        }
    }

    public void UpdateHearts()
    {
        for (int i = 0; i < hearts.Count; i++)
        {
            if (i < player.health)
            {
                hearts[i].SetActive(true);
                hearts[i].GetComponent<SpriteRenderer>().sprite = heartPrefab.GetComponent<SpriteRenderer>().sprite;
            }
            else
            {
                hearts[i].SetActive(true);
                hearts[i].GetComponent<SpriteRenderer>().sprite = emptyHeartPrefab.GetComponent<SpriteRenderer>().sprite;
            }
        }
    }
}