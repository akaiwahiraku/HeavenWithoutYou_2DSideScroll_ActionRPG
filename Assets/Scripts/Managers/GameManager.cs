using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour , ISaveManager
{

    public static GameManager instance;

    private Transform player;

    [SerializeField] private Checkpoint[] checkpoints;
    [SerializeField] private string closestCheckpointId;

    [Header("Lost currency")]
    [SerializeField] private GameObject lostCurrencyPrefab;
    public int lostCurrencyAmount;
    [SerializeField] private float lostCurrencyX;
    [SerializeField] private float lostCurrencyY;

    public GameData gameData;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        // ゲームデータの初期化
        gameData = new GameData();
    }

    private void Start()
    {
        checkpoints = FindObjectsOfType<Checkpoint>();
        player = CurrencyManager.instance.player.transform;

        // シーンの再読み込み後にセーブデータを自動ロード
        if (SaveManager.instance != null && SaveManager.instance.HasSavedData())
        {
            SaveManager.instance.LoadGame();
        }
    }

    public void RestartScene()
    {
        SaveManager.instance.SaveGame();
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

    public void LoadData(GameData _data)
    {
        StartCoroutine(LoadWithDelay(_data));

        // スキルツリーの状態をロード
        foreach (var skillSlot in FindObjectsOfType<UI_SkillTreeSlot>())
        {
            skillSlot.LoadData(_data);
        }
    }

    private void LoadCheckpoints(GameData _data)
    {
        foreach (KeyValuePair<string, bool> pair in _data.checkpoints)
        {
            foreach (Checkpoint checkpoint in checkpoints)
            {
                if (checkpoint.id == pair.Key && pair.Value == true)
                    checkpoint.ActivateCheckpoint();
            }
        }
    }

    private void LoadLostCurrency(GameData _data)
    {
        lostCurrencyAmount = _data.lostCurrencyAmount;
        lostCurrencyX = _data.lostCurrencyX;
        lostCurrencyY = _data.lostCurrencyY;

        if (lostCurrencyAmount > 0)
        {
            GameObject newLostCurrency = Instantiate(lostCurrencyPrefab, new Vector3(lostCurrencyX, lostCurrencyY), Quaternion.identity);
            newLostCurrency.GetComponent<LostCurrencyController>().currency = lostCurrencyAmount;
        }

        lostCurrencyAmount = 0;
    }

    private IEnumerator LoadWithDelay(GameData _data)
    {
        yield return new WaitForSeconds(.1f);

        LoadCheckpoints(_data);
        LoadClosestCheckpoint(_data);
        LoadLostCurrency(_data);
    }


    public void SaveData(ref GameData _data)
    {
        _data.lostCurrencyAmount = lostCurrencyAmount;
        _data.lostCurrencyX = player.position.x;
        _data.lostCurrencyY = player.position.y;

        if(FindClosestCheckpoint() != null)
            _data.closestCheckpointId = FindClosestCheckpoint().id;

        _data.checkpoints.Clear();

        foreach(Checkpoint checkpoint in checkpoints)
        {
            _data.checkpoints.Add(checkpoint.id, checkpoint.activationStatus);
        }

        // スキルツリーの状態をセーブ
        foreach (var skillSlot in FindObjectsOfType<UI_SkillTreeSlot>())
        {
            skillSlot.SaveData(ref _data);
        }
    }
    private void LoadClosestCheckpoint(GameData _data)
    {
        if (_data.closestCheckpointId == null)
            return;

        closestCheckpointId = _data.closestCheckpointId;

        foreach (Checkpoint checkpoint in checkpoints)
        {
            if (closestCheckpointId == checkpoint.id)
                player.position = checkpoint.transform.position;
        }
    }

    private Checkpoint FindClosestCheckpoint()
    {
        float closestDistance = Mathf.Infinity;
        Checkpoint closestCheckpoint = null;

        foreach (var checkpoint in checkpoints)
        {
            float distanceToCheckpoint = Vector2.Distance(player.position, checkpoint.transform.position);

            if (distanceToCheckpoint < closestDistance && checkpoint.activationStatus == true)
            {
                closestDistance = distanceToCheckpoint;
                closestCheckpoint = checkpoint;
            }
        }

        return closestCheckpoint;
    }

    public void PauseGame(bool _pause)
    {
        if (_pause)
            Time.timeScale = 0;
        else
            Time.timeScale = 1;
    }

    private Dictionary<string, bool> GetCurrentSkillStates()
    {
        // 現在のスキル状態を取得し、スキルIDとその状態（true/false）を返すように実装します。
        // 例: スキル管理クラスからスキル情報を取得するなど
        return new Dictionary<string, bool>();
    }

    private void LoadAcquiredSkills(SerializableDictionary<string, bool> skillTree)
    {
        // スキル管理クラスに保存されたスキル情報を設定する処理を記述します。
        foreach (var skill in skillTree)
        {
            // スキルを適用する処理
        }
    }

    public void ResetSkills()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.gameData.ResetSkills();

            foreach (var skillSlot in FindObjectsOfType<UI_SkillTreeSlot>())
            {
                skillSlot.LoadData(GameManager.instance.gameData);
            }
        }
    }
}
