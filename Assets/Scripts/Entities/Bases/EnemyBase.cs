using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class EnemyBase : Base
{
    [SerializeField] List<SpawnSchedule> spawns;
    public override Alliance side => Alliance.Enemy;
    private void Awake()
    {
        GameManager.Instance.onGameStart += OnGameStart;
    }
    protected override void OnDeath()
    {
        base.OnDeath();
        StopAllCoroutines();
        GameManager.Instance.Victory();
    }
    float timeSinceStart = 0.0f;
    void OnGameStart()
    {
        foreach (var i in spawns) StartCoroutine(SpawnUnits(i));
    }
    IEnumerator SpawnUnits(SpawnSchedule schedule)
    {
        yield return new WaitForSeconds(schedule.startTime);
        for(int i = 0; i < schedule.count; i++)
        {
            SpawnUnit(schedule.unit);
            yield return new WaitForSeconds(schedule.interval);
        }
    }
}
[System.Serializable]
public struct SpawnSchedule
{
    [SerializeField] float m_startTime;
    [SerializeField] Unit m_unit;
    [SerializeField] int m_count;
    [SerializeField] float m_interval;
    public float startTime => m_startTime;
    public Unit unit => m_unit;
    public int count => m_count;
    public float interval => m_interval;
}