using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class HealthUI : MonoBehaviour
{
    public PlayerController player;
    public GameObject heartPrefab;
    public GameObject emptyHeartPrefab;
    public Transform heartContainer;
    private List<GameObject> hearts = new List<GameObject>();
    public float spacing = 1f; 
    public Vector3 offset = new Vector3(2f, 2f, 0f); 
    public Camera mainCamera;
    private Vector3 currentHeartPosition;
    public float smoothFactor = 5f;
    public float flickerDuration = 1f;

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
        int previousHeartCount = hearts.Count;

        foreach (var heart in hearts)
        {
            Destroy(heart);
        }
        hearts.Clear();

        for (int i = 0; i < player.maxHealth; i++)
        {
            GameObject heart = Instantiate(i < player.health ? heartPrefab : emptyHeartPrefab, heartContainer);
            heart.SetActive(true);
            hearts.Add(heart);
        }
        if (previousHeartCount > player.health && previousHeartCount > 0)
        {
            StartCoroutine(FlickerHearts(previousHeartCount - player.health));
        }
        else
        {
            UpdateHeartPositions();

        }
    }

    private IEnumerator FlickerHearts(int heartsToFlicker)
    {
        float elapsedTime = 0f;
        float flickerInterval = 0.1f;

        while (elapsedTime < flickerDuration)
        {
            for (int i = player.health; i < player.health + heartsToFlicker; i++)
            {
                if (i < hearts.Count)
                {
                    hearts[i].SetActive(!hearts[i].activeSelf);
                }
            }

            elapsedTime += flickerInterval;
            yield return new WaitForSeconds(flickerInterval);
        }

        UpdateHearts();
        UpdateHeartPositions();
    }

    private void UpdateHeartPositions()
    {
        if (mainCamera == null || heartContainer == null) return;

        float viewportHeight = 2f * mainCamera.orthographicSize;
        float viewportWidth = viewportHeight * mainCamera.aspect;

        Vector3 topRight = mainCamera.transform.position + new Vector3(viewportWidth / 2, viewportHeight / 2, 0);

        Vector3 newPos = topRight - offset;
        currentHeartPosition = Vector3.Lerp(currentHeartPosition, newPos, Time.deltaTime * smoothFactor);

        for (int i = 0; i < hearts.Count; i++)
        {
            if (hearts[i] == null) continue;

            Vector3 heartPosition = currentHeartPosition - new Vector3(i * spacing, 0, 0);
            
            hearts[i].transform.position = heartPosition;

            hearts[i].transform.forward = -mainCamera.transform.forward;
            hearts[i].transform.position = new Vector3(hearts[i].transform.position.x, hearts[i].transform.position.y, 0);
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
        UpdateHeartPositions();
    }
}