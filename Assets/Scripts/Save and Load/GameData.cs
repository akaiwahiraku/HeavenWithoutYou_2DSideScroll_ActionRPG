using System.Collections;
using System.Collections.Generic;
using Cinemachine.PostFX;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public int currency;
    public SerializableDictionary<string, bool> skillTree;
    public SerializableDictionary<string, int> inventory;
    public List<string> equipmentId;
    public SerializableDictionary<string, bool> checkpoints;
    public string closestCheckpointId;
    public float lostCurrencyX;
    public float lostCurrencyY;
    public int lostCurrencyAmount;
    public SerializableDictionary<string, float> volumeSettings;
    public int playerLevel;
    public int playerExperience;
    public int experienceToNextLevel; // 新しいフィールドを追加


    // ステータスフィールド
    public int strength;
    public int agility;
    public int vitality;
    public int intelligence;
    public int maxHealth;

    public GameData()
    {
        this.lostCurrencyX = 0;
        this.lostCurrencyY = 0;
        this.lostCurrencyAmount = 0;
        this.currency = 0;
        skillTree = new SerializableDictionary<string, bool>();
        inventory = new SerializableDictionary<string, int>();
        equipmentId = new List<string>();
        closestCheckpointId = string.Empty;
        checkpoints = new SerializableDictionary<string, bool>();
        volumeSettings = new SerializableDictionary<string, float>();
        playerLevel = 1;
        playerExperience = 0;
        experienceToNextLevel = 100;

        // ステータスの初期値設定
        strength = 30;
        agility = 20;
        vitality = 15;
        intelligence = 10;
        maxHealth = 500;
    }

    // スキルの保存
    public void SaveSkillState(string skillName, bool isUnlocked)
    {
        if (skillTree.ContainsKey(skillName))
        {
            skillTree[skillName] = isUnlocked;
        }
        else
        {
            skillTree.Add(skillName, isUnlocked);
        }
    }

    // スキルの状態を取得
    public bool LoadSkillState(string skillName)
    {
        if (skillTree.TryGetValue(skillName, out bool isUnlocked))
        {
            return isUnlocked;
        }
        return false; // デフォルトの状態（アンロックされていない状態）
    }

    // スキルのリセット
    public void ResetSkills()
    {
        skillTree.Clear();
    }
}
