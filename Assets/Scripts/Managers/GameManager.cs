using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public GameManager() => Instance = this;

    [SerializeField] int stageIndex = 0;
    [SerializeField] float timePast = 0.0f;

    [Header("Gems")]
    [SerializeField] int m_maxGems = 20;
    [SerializeField] float m_gemGenerationTime = 10;
    [SerializeField] int tilePopRequired = 20;

    [Header("Board")]
    [SerializeField] HexBoard board;

    [Header("Base")]
    [SerializeField] PlayerBase m_playerBase;
    [SerializeField] Transform m_rightMapEdge;

    [Header("Cutscenes")]
    [SerializeField] Cutscene startCutscene;
    [SerializeField] Cutscene victoryCutscene, defeatCutscene;

    [Header("Audio")]
    [SerializeField] AudioSource gemEarnAudio;
    [SerializeField] AudioSource tilePopAudio;
    public PlayerBase playerBase => m_playerBase;
    public Transform rightMapEdge => m_rightMapEdge;
    public int maxGems => m_maxGems;
    public float gemGenerationTime => m_gemGenerationTime;
    int m_gems;
    public int gems
    {
        get { return m_gems; }
        set { m_gems = value; onGemCountChange?.Invoke(); }
    }
    public Action onGemCountChange, onGemEarn;
    public float gemGenerationCounter { get; private set; } = 0;

    public bool gameInProgress { get; private set; } = false;
    public Action onGameStart, onGameEnd;
    private void Awake()
    {
        board.onTilePop += OnTilePop;
        onGemEarn += gemEarnAudio.Play;
    }
    void OnTilePop()
    {
        tilePopAudio.Play();
        gemGenerationCounter = Mathf.Min(gemGenerationTime, gemGenerationCounter + gemGenerationTime / tilePopRequired);
        if (gemGenerationCounter >= gemGenerationTime && gems < maxGems)
        {
            gemGenerationCounter = 0.0f;
            gems++;
            onGemEarn?.Invoke();
        }
    }
    private void Start()
    {
        gems = maxGems / 2;
        CutsceneManager.Instance.PlayCutscene(startCutscene, StartGame);
    }
    private void Update()
    {
        if (!gameInProgress) return;
        timePast += Time.deltaTime;
        gemGenerationCounter = Mathf.Min(gemGenerationTime, gemGenerationCounter + Time.deltaTime);
        if(gemGenerationCounter >= gemGenerationTime && gems < maxGems)
        {
            gemGenerationCounter = 0.0f;
            gems++;
            onGemEarn?.Invoke();
        }
    }
    public void EarnGem(int count)
    {
        gems += count;
        onGemEarn?.Invoke();
    }
    public void ConsumeGem(int count) => gems -= count;
    public void StartGame()
    {
        board.canInteract = true;
        gameInProgress = true;
        onGameStart?.Invoke();
    }
    void EndGame()
    {
        board.canInteract = false;
        gameInProgress = false;
        onGameEnd?.Invoke();
    }
    public Action onGameDefeat;
    public Action onGameWin;
    public bool newRecord { get; private set; } = false;
    public void Defeat()
    {;
        EndGame();
        CutsceneManager.Instance.PlayCutscene(defeatCutscene, onGameDefeat);
    }
    public void Victory()
    {
        EndGame();
        if (GlobalManager.Instance.save.stageSaves[stageIndex].completed)
        {
            if (GlobalManager.Instance.save.stageSaves[stageIndex].record > timePast)
            {
                newRecord = true;
                GlobalManager.Instance.save.stageSaves[stageIndex].record = timePast;
            }
        }
        else
        {
            GlobalManager.Instance.save.stageSaves[stageIndex].record = timePast;
            GlobalManager.Instance.save.stageSaves[stageIndex].completed = true;
        }
        if (GlobalManager.Instance.save.lastCompleted < stageIndex) GlobalManager.Instance.save.lastCompleted = stageIndex;
        CutsceneManager.Instance.PlayCutscene(victoryCutscene, onGameWin);
    }
    public void Restart()
    {
        SceneSwitcher.SwitchScene("Stage" + stageIndex);
    }
    public void ReturnToMenu()
    {
        SceneSwitcher.SwitchScene("Stage");
    }
}