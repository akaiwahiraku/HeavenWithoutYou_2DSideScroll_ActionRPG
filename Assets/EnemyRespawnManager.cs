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
        // activeEnemies���X�g���N���[���A�b�v����null���܂܂Ȃ��悤�ɂ���
        activeEnemies.RemoveAll(e => e == null);

        // ��A�N�e�B�u�ȓG���ė��p
        GameObject enemy = activeEnemies.Find(e => !e.activeInHierarchy);
        if (enemy != null)
        {
            enemy.transform.position = position;
            enemy.SetActive(true);
        }
        else
        {
            // �V�����G�𐶐����ă��X�g�ɒǉ�
            enemy = Instantiate(enemyPrefab, position, Quaternion.identity);
            activeEnemies.Add(enemy);
        }
    }
}
