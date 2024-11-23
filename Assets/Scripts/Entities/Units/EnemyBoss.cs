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
    protected override bool knockbackImmune => true;
    protected override void Awake()
    {
        base.Awake();
        GameManager.Instance.onGameStart += OnGameStart;
        Set(Alliance.Enemy, m_moveDir);
    }
    void OnGameStart()
    {
        anim.SetBool("Started", true);
        foreach (var i in spawns) StartCoroutine(SpawnUnits(i));
    }
    IEnumerator SpawnUnits(SpawnSchedule schedule)
    {
        yield return new WaitForSeconds(schedule.startTime);
        for (int i = 0; i < schedule.count; i++)
        {
            Instantiate(schedule.unit, spawnPosition.position, Quaternion.identity).Set(Alliance.Enemy, moveDir);
            yield return new WaitForSeconds(schedule.interval);
        }
    }
    protected override void OnDeath()
    {
        base.OnDeath();
        StopAllCoroutines();
        GameManager.Instance.Victory();
    }
    public void Attack()
    {
        if (scanned != null) scanned.OnDamage(damage);
    }
}