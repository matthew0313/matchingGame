using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SaveFileHandler
{
    public abstract void Save(SaveData data, string fileName);
    public abstract SaveData Load(string fileName);
    public abstract void Delete(string fileName);
}