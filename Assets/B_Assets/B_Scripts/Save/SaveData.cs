using System;
using System.Collections.Generic;
using Takato;
using UnityEngine;

[Serializable]
public class SaveData
{
    public enum Character
    {
        Sword,
        GreateSword,
        Wizard
    }
    public Character character;
    public float playTime;
    public int level;
    public int stage;
    public float currentHealth;
    public Vector3 playerPosition;
    public List<SkillBase> skills;
    public string warpPointName;

    // コンストラクタで初期値を設定
    public SaveData(Character character)
    {
        this.character = character;
        this.playTime = 0;
        this.level = 1;
        this.stage = 0;
        this.currentHealth = 100f;
        this.playerPosition = Vector3.zero;
    }
}
