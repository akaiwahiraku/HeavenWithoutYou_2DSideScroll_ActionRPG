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
    public int experienceToNextLevel; // �V�����t�B�[���h��ǉ�


    // �X�e�[�^�X�t�B�[���h
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

        // �X�e�[�^�X�̏����l�ݒ�
        strength = 30;
        agility = 20;
        vitality = 15;
        intelligence = 10;
        maxHealth = 500;
    }

    // �X�L���̕ۑ�
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

    // �X�L���̏�Ԃ��擾
    public bool LoadSkillState(string skillName)
    {
        if (skillTree.TryGetValue(skillName, out bool isUnlocked))
        {
            return isUnlocked;
        }
        return false; // �f�t�H���g�̏�ԁi�A�����b�N����Ă��Ȃ���ԁj
    }

    // �X�L���̃��Z�b�g
    public void ResetSkills()
    {
        skillTree.Clear();
    }
}
