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
    public Character character;     //使用中のキャラクター
    public float playTime;          //プレイ時間
    public int level;               //レベル
    public int stage;               //ステージ
    public float currentHealth;     //現在のHP
    public Vector3 playerPosition;  //現在のキャラクターの位置
    public List<SkillBase> skills;  //スキル
    public string warpPointName;    //解放されているワープポイント
    public string destinationText;  //現在の目的のテキスト

    // コンストラクタで初期値を設定
    public SaveData(Character character)
    {
        this.character = character;
        playTime = 0;
        level = 1;
        stage = 0;
        currentHealth = 100f;
        playerPosition = Vector3.zero;
        warpPointName = "WarpObject";
        destinationText = "遺跡に向かう";
    }
}
