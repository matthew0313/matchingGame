using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public GameManager() => Instance = this;

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
    }
    void OnTilePop()
    {
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
    public void Defeat()
    {;
        EndGame();
        CutsceneManager.Instance.PlayCutscene(defeatCutscene);
    }
    public void Victory()
    {
        EndGame();
        CutsceneManager.Instance.PlayCutscene(victoryCutscene);
    }
}