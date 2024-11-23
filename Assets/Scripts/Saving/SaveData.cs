using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public bool introWatched = false;
    public Settings settings = new();
    public SerializableDicionary<int, float> stageRecords = new();
}
