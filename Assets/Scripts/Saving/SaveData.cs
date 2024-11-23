using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public bool introWatched = false;
    public Settings settings = new();
    public int lastCompleted = -1;
    public StageSaveData[] stageSaves = new StageSaveData[10];
}
[System.Serializable]
public struct StageSaveData
{
    public bool completed;
    public float record;
}
