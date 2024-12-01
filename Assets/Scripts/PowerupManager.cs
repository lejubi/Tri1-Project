using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PowerupManager : MonoBehaviour
{
    [System.Serializable]
    public class PowerupInfo
    {
        public GameObject prefab;
        public float weight = 1f;
    }

    public PowerupInfo[] powerups;
    public float minSpawnInterval = 5f;
    public float maxSpawnInterval = 15f;
    public float spawnHeight = 30f;
    public LayerMask groundLayer;

    private float totalWeight;
    public GameObject player;
    public float minSpawnDistance = 5f;
    public float maxSpawnDistance= 15f;

    void Start()
    {
        
        totalWeight = 0f;
        foreach (var powerup in powerups)
        {
            totalWeight += powerup.weight;
        }

        StartCoroutine(SpawnPowerups());
    }

    IEnumerator SpawnPowerups()
    {
        while (true)
        {
            float interval = Random.Range(minSpawnInterval, maxSpawnInterval);
            yield return new WaitForSeconds(interval);

            SpawnRandomPowerup();
        }
    }

    void SpawnRandomPowerup()
    {
        float randomValue = Random.Range(0f, totalWeight);
        float weightSum = 0f;
        PowerupInfo selectedPowerup = null;

        foreach (var powerup in powerups)
        {
            weightSum += powerup.weight;
            if (randomValue <= weightSum)
            {
                selectedPowerup = powerup;
                break;
            }
        }

        if (selectedPowerup != null)
        {
            Vector3 spawnPosition = CalculateSpawnPosition();

            GameObject powerupInstance = Instantiate(selectedPowerup.prefab, spawnPosition, Quaternion.identity);
            Rigidbody2D rb = powerupInstance.GetComponent<Rigidbody2D>();
            
            if (rb == null)
            {
                rb = powerupInstance.AddComponent<Rigidbody2D>();
            }

            rb.gravityScale = 1f;
            rb.drag = 0.5f;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

            CircleCollider2D collider = powerupInstance.GetComponent<CircleCollider2D>();
            if (collider == null)
            {
                collider = powerupInstance.AddComponent<CircleCollider2D>();
            }

            // collider.isTrigger = true;

            PhysicsMaterial2D bouncyMaterial = new PhysicsMaterial2D();
            bouncyMaterial.bounciness = 0.5f;
            bouncyMaterial.friction = 0.4f;
            collider.sharedMaterial = bouncyMaterial;

            StartCoroutine(SettlePowerup(powerupInstance));
        }
    }
    Vector3 CalculateSpawnPosition()
    {
        float spawnDistance = Random.Range(minSpawnDistance, maxSpawnDistance);
        Vector3 playerPosition = player.transform.position;
        
        return new Vector3(playerPosition.x + spawnDistance, spawnHeight, playerPosition.z);
    }

    IEnumerator SettlePowerup(GameObject powerup)
    {
        yield return new WaitForSeconds(5f); 
        if (powerup != null)
        {
            Rigidbody2D rb = powerup.GetComponent<Rigidbody2D>();
            while (!IsGrounded(powerup))
            {
                yield return null;
            }

            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }
    }

    bool IsGrounded(GameObject obj)
    {
        Collider2D collider = obj.GetComponent<Collider2D>();
        return Physics2D.Raycast(collider.bounds.center, Vector2.down, collider.bounds.extents.y + 0.1f, groundLayer);
    }
}