using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRespawnManager : MonoBehaviour
{
    public GameObject enemyPrefab;
    public Transform[] respawnPoints;
    public float respawnInterval = 10.0f;

    private List<GameObject> activeEnemies = new List<GameObject>();

    void Start()
    {
        StartCoroutine(RespawnEnemiesAtIntervals());
    }

    private IEnumerator RespawnEnemiesAtIntervals()
    {
        while (true)
        {
            yield return new WaitForSeconds(respawnInterval);

            foreach (Transform point in respawnPoints)
            {
                RespawnEnemy(point.position);
            }
        }
    }

    private void RespawnEnemy(Vector3 position)
    {
        // activeEnemiesリストをクリーンアップしてnullを含まないようにする
        activeEnemies.RemoveAll(e => e == null);

        // 非アクティブな敵を再利用
        GameObject enemy = activeEnemies.Find(e => !e.activeInHierarchy);
        if (enemy != null)
        {
            enemy.transform.position = position;
            enemy.SetActive(true);
        }
        else
        {
            // 新しい敵を生成してリストに追加
            enemy = Instantiate(enemyPrefab, position, Quaternion.identity);
            activeEnemies.Add(enemy);
        }
    }
}
