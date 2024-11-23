using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public Settings settings = new();
    public SerializableDicionary<int, StageSaveData> stages = new();
}
[System.Serializable]
public class StageSaveData
{
    public bool completed = false;
    public bool star1 = false, star2 = false, star3 = false;
    public float shortestTime;
}
