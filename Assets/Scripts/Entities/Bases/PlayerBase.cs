using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBase : Base
{
    public override Alliance side => Alliance.Player;
    protected override void OnDeath()
    {
        base.OnDeath();
        GameManager.Instance.Defeat();
    }
}