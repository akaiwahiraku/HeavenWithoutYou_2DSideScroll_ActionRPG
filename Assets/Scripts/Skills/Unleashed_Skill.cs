using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Unleashed_Skill : Skill
{
    public float slowFactor = 0.3f;
    public float duration = 7f;
    private List<Enemy> enemies;
    [SerializeField] private UI_SkillTreeSlot unleashedUnlockButton;
    public bool unleashedUnlocked { get; private set; }
    public static bool IsUnleashedActive { get; private set; } = false;

    // （黒いパネルは保留）
    //[SerializeField] private GameObject blackScreenPanel;

    // ▼スキル発動中に敵の色を変更する色
    [Header("Colors")]
    [SerializeField] private Color unleashedEnemyColor = new Color(0f, 0.5f, 1f, 1f);
    // ▼スキル発動中に地形（TilemapやBackground2など）の色を変更する色
    [SerializeField] private Color unleashedTerrainColor = new Color(0.3f, 0.8f, 1f, 1f);
    // ▼スキル発動中にプレイヤーの色を変更する色
    [SerializeField] private Color unleashedPlayerColor = new Color(0f, 1f, 1f, 1f);

    // ■敵用：SpriteRenderer の元の色を保存
    private Dictionary<SpriteRenderer, Color> originalEnemyColors = new Dictionary<SpriteRenderer, Color>();
    // ■プレイヤー用：元の色
    private Color originalPlayerColor;

    // ■地形用：Tilemap の元の色を保存
    private Dictionary<Tilemap, Color> terrainTilemapColors = new Dictionary<Tilemap, Color>();
    // ■Background2 用：SpriteRenderer の元の色を保存
    private Dictionary<SpriteRenderer, Color> background2Renderers = new Dictionary<SpriteRenderer, Color>();

    protected override void Start()
    {
        base.Start();

        enemies = new List<Enemy>(FindObjectsOfType<Enemy>());
        if (enemies.Count == 0)
        {
            Debug.LogWarning("シーン内にEnemyコンポーネントが付いたオブジェクトが見つかりませんでした。");
        }

        if (player == null)
        {
            player = FindObjectOfType<Player>();
            if (player == null)
            {
                Debug.LogError("シーン内にPlayerコンポーネントが見つかりませんでした。");
            }
        }
    }



    protected override void CheckUnlock()
    {
        UnlockUnleashed();
    }

    private void UnlockUnleashed()
    {
        if (unleashedUnlockButton.unlocked)
        {
            unleashedUnlocked = true;
            isUnlocked = true;
        }
    }

    public override void UseSkill()
    {
        base.UseSkill();
        // 必要なら追加処理（クールダウンなど）
    }

    // スキル発動直前に最新の敵リストを更新して開始
    public void ActivateUnleashedSkill()
    {
        enemies = new List<Enemy>(FindObjectsOfType<Enemy>());
        StartCoroutine(SlowAndUnleashedCoroutine());
    }

    IEnumerator SlowAndUnleashedCoroutine()
    {
        Debug.Log("Unleashed_Skill: 演出開始");
        IsUnleashedActive = true;

        // ■1. 敵の色変更（SpriteRenderer）
        foreach (Enemy enemy in enemies)
        {
            if (enemy == null) continue;
            SpriteRenderer[] srs = enemy.GetComponentsInChildren<SpriteRenderer>();
            foreach (SpriteRenderer sr in srs)
            {
                if (!originalEnemyColors.ContainsKey(sr))
                {
                    originalEnemyColors.Add(sr, sr.color);
                }
                // 敵には unleashedEnemyColor を適用
                sr.material.color = unleashedEnemyColor;
            }
        }

        // ■2. 地形（Tilemap）の色変更
        Tilemap[] allTilemaps = FindObjectsOfType<Tilemap>();
        foreach (Tilemap tilemap in allTilemaps)
        {
            if (!terrainTilemapColors.ContainsKey(tilemap))
            {
                terrainTilemapColors.Add(tilemap, tilemap.color);
            }
            // 地形には unleashedTerrainColor を適用
            tilemap.color = unleashedTerrainColor;
        }

        // ■3. Sorting Layer が "Background2" の SpriteRenderer を検索して色変更
        SpriteRenderer[] allSpriteRenderers = FindObjectsOfType<SpriteRenderer>();
        foreach (SpriteRenderer sr in allSpriteRenderers)
        {
            if (sr.sortingLayerName == "Background2")
            {
                if (!background2Renderers.ContainsKey(sr))
                {
                    background2Renderers.Add(sr, sr.color);
                }
                // 地形扱いとして unleashedTerrainColor を適用
                sr.material.color = unleashedTerrainColor;
                Debug.Log($"Sorting Layer = Background2: {sr.gameObject.name} の色を変更");
            }
        }

        // ■4. プレイヤーの色変更
        if (player != null)
        {
            SpriteRenderer playerSR = player.sr;
            if (playerSR != null)
            {
                originalPlayerColor = playerSR.color;
                playerSR.material.color = unleashedPlayerColor;
            }
            //player.stats.MakeUnleashed(true);
        }
        else
        {
            Debug.LogError("Player参照がnullです。");
        }

        // ■5. 敵へのスロー効果
        float slowPercentage = 1 - slowFactor;
        foreach (Enemy enemy in enemies)
        {
            if (enemy == null) continue;
            enemy.SlowEntityBy(slowPercentage, duration);
        }

        foreach (ISlowable slowable in FindObjectsOfType<MonoBehaviour>().OfType<ISlowable>())
        {
            slowable.ApplySlow(slowFactor, duration);
        }

        // ■6. 効果中のループ
        float timer = duration;
        while (timer > 0)
        {
            UpdateEffects();
            if (player != null && player.fx != null)
            {
                player.fx.CreateAfterImage(player.rb.velocity);
            }
            yield return null;
            timer -= Time.deltaTime;
        }


        // ■7. 終了時の処理
        ResetEffects();
        IsUnleashedActive = false;
        Debug.Log("Unleashed_Skill: 演出終了");
    }

    // 効果中に毎フレーム再適用
    private void UpdateEffects()
    {
        // 敵（SpriteRenderer）
        foreach (Enemy enemy in enemies)
        {
            if (enemy == null) continue;
            SpriteRenderer[] srs = enemy.GetComponentsInChildren<SpriteRenderer>();
            foreach (SpriteRenderer sr in srs)
            {
                sr.material.color = unleashedEnemyColor;
            }
        }

        // 地形（Tilemap）
        foreach (var kvp in terrainTilemapColors)
        {
            if (kvp.Key != null)
            {
                kvp.Key.color = unleashedTerrainColor;
            }
        }

        // Sorting Layer "Background2" のSpriteRenderer
        foreach (var kvp in background2Renderers)
        {
            if (kvp.Key != null)
            {
                kvp.Key.material.color = unleashedTerrainColor;
            }
        }

        // プレイヤー
        if (player != null && player.sr != null)
        {
            player.sr.material.color = unleashedPlayerColor;
        }
    }

    // 効果終了時に元の色へ戻す
    private void ResetEffects()
    {
        // ■敵の色を元に戻す
        foreach (var kvp in originalEnemyColors)
        {
            if (kvp.Key != null)
            {
                kvp.Key.material.color = kvp.Value;
            }
        }
        originalEnemyColors.Clear();

        // ■地形の色を元に戻す（Tilemap）
        foreach (var kvp in terrainTilemapColors)
        {
            if (kvp.Key != null)
            {
                kvp.Key.color = kvp.Value;
            }
        }
        terrainTilemapColors.Clear();

        // ■Background2 のSpriteRendererを元に戻す
        foreach (var kvp in background2Renderers)
        {
            if (kvp.Key != null)
            {
                kvp.Key.material.color = kvp.Value;
            }
        }
        background2Renderers.Clear();

        // ■プレイヤーの色を元に戻す＆無敵解除
        if (player != null && player.sr != null)
        {
            player.sr.material.color = originalPlayerColor;
        }
        //player.stats.MakeUnleashed(false);
    }
}
