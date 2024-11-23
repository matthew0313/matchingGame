using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalManager : MonoBehaviour
{
    public static GlobalManager Instance { get; private set; }
    public GlobalManager()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    public SaveData save { get; private set; }
    public Settings settings => save.settings;
    public Action onLanguageChange;

    SaveFileHandler saveHandler;
    [Header("Save")]
    [SerializeField] string fileExtension = ".json";

    [Header("Debug")]
    [SerializeField] bool debug = false;
    [SerializeField] Language debugLanguage = Language.Korean;
    private void Awake()
    {
        DontDestroyOnLoad(this);
        saveHandler = new LocalSaveFileHandler(Application.persistentDataPath, fileExtension);
        save = saveHandler.Load("Data");
        if (save == null) save = new();
    }
    private void Start()
    {
        if (debug)
        {
            save.settings.language = debugLanguage;
            onLanguageChange?.Invoke();
        }
    }
    public void ChangeLanguage(Language language)
    {
        settings.language = language;
        onLanguageChange?.Invoke();
    }
    private void OnApplicationQuit()
    {
        saveHandler.Save(save, "Data");
    }
}
[System.Serializable]
public class Settings
{
    public Language language = Language.Korean;
}