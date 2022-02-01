using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeatherEffectRain : MonoBehaviour
{

    [SerializeField]
    private new Camera camera;

    [SerializeField]
    private ScriptablePoolContainer rainDrops;

    [SerializeField]
    private Vector2 minMaxSpawnSpeed;

    [SerializeField]
    private Vector2 spawnOffset;

    [SerializeField]
    private bool prewarm;

    private Vector2 screenBounds;

    private void OnEnable()
    {
        screenBounds = new Vector2()
        {
            x = camera.ScreenToWorldPoint(new Vector3(Screen.width, 0f, 0f)).x,
            y = camera.ScreenToWorldPoint(new Vector3(0, Screen.height, 0f)).y * 0.5f,
        };

        StartEffect();
    }

    private void OnDrawGizmos()
    {
        if (screenBounds.x != 0 && screenBounds.y != 0)
        {
            Gizmos.DrawWireSphere(camera.transform.position + new Vector3(screenBounds.x + spawnOffset.x, 0,0), 0.15f);
            Gizmos.DrawWireSphere(camera.transform.position + new Vector3(-screenBounds.x + spawnOffset.x, 0, 0), 0.15f);
            Gizmos.DrawWireSphere(camera.transform.position + new Vector3(0, screenBounds.y + spawnOffset.y, 0), 0.15f);
            Gizmos.DrawWireSphere(camera.transform.position + new Vector3(0, -screenBounds.y + spawnOffset.y, 0), 0.15f);
        }
    }

    public void StartEffect ()
    {
        if (prewarm)
        {
            for (int i = 0; i < 10; i++)
            {
                SpawnRainDrop();
            }
        }

        StartCoroutine(EffectCoroutine());
    }

    private IEnumerator EffectCoroutine ()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minMaxSpawnSpeed.x, minMaxSpawnSpeed.y));

            SpawnRainDrop();
        }
    }

    private void SpawnRainDrop()
    {
        GameObject rainDrop = rainDrops.Retrieve();

        // This keeps the Hierarchy clean by parenting the raindrops. Not required in build.
#if UNITY_EDITOR
        rainDrop.transform.SetParent(this.transform);
#endif

        Vector3 spawnPosition = new Vector3()
        {
            x = camera.transform.position.x + Random.Range(-screenBounds.x, screenBounds.x) + spawnOffset.x,
            y = camera.transform.position.y + Random.Range(-screenBounds.y, screenBounds.y) + spawnOffset.y
        };

        rainDrop.transform.position = spawnPosition;
        rainDrop.gameObject.SetActive(true);
    }
}
