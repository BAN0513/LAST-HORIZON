using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveData
{
    public float playTime;
    public int level;
    public int stage;
    public float currentHealth;
    public Vector3 playerPosition;

    // コンストラクタで初期値を設定
    public SaveData()
    {
        this.level = 1;
        this.currentHealth = 100f;
        this.playerPosition = Vector3.zero;
    }
}
