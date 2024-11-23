using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class EnemyBoss : Unit
{
    [SerializeField] MoveDirection m_moveDir;
    [SerializeField] List<SpawnSchedule> spawns;
    [SerializeField] Transform spawnPosition;
    private void Awake()
    {
        GameManager.Instance.onGameStart += OnGameStart;
        moveDir = m_moveDir;
        side = Alliance.Enemy;
    }
    void OnGameStart()
    {
        anim.SetBool("Started", true);
        foreach (var i in spawns) StartCoroutine(SpawnUnits(i));
    }
    public override Unit Instantiate(Vector2 position, Alliance side, MoveDirection direction)
    {
        return null;
    }
    public override void Release() { }
    IEnumerator SpawnUnits(SpawnSchedule schedule)
    {
        yield return new WaitForSeconds(schedule.startTime);
        for (int i = 0; i < schedule.count; i++)
        {
            schedule.unit.Instantiate(spawnPosition.position, Alliance.Enemy, moveDir);
            yield return new WaitForSeconds(schedule.interval);
        }
    }
    protected override void OnDeath()
    {
        base.OnDeath();
        GameManager.Instance.Victory();
    }
    public void Attack()
    {
        if (scanned != null) scanned.OnDamage(damage);
    }
}