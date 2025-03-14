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

            if (Unleashed_Skill.IsUnleashedActive)
            {
                Debug.Log("Unleashed�X�L���������̂��߁A�G���X�|�[�����X�L�b�v���܂��B");
                continue;
            }

            foreach (Transform point in respawnPoints)
            {
                RespawnEnemy(point.position);
            }
        }
    }

    private void RespawnEnemy(Vector3 position)
    {
        // activeEnemies���X�g�̃N���[�j���O
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
            // �V�K�������ă��X�g�ɒǉ�
            enemy = Instantiate(enemyPrefab, position, Quaternion.identity);
            activeEnemies.Add(enemy);
        }
    }
}
