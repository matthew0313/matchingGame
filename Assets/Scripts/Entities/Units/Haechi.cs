using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Haechi : Unit
{
    [SerializeField] Transform beam, beamPoint;
    [SerializeField] float bonusDamageLimit;
    [SerializeField] float weighIn;
    [SerializeField] float beamTickRate = 0.5f;

    private bool isAttack;

    private float tmpDamage;
    private IDamagable tmpScanned;
    private void Start()
    {
        tmpDamage = damage;
        
    }
    readonly int beamingID = Animator.StringToHash("Beaming");
    float counter = 0.0f;
    float bonusDamage = 0.0f;
    IDamagable prev = null;
    protected override void Update()
    {
        base.Update();
        if (anim.GetBool(beamingID) && (scanned as MonoBehaviour) != null)
        {
            if(scanned != prev)
            {
                bonusDamage = 0.0f;
            }
            prev = scanned;

            beam.position = beamPoint.position;
            Vector2 dir = (scanned as MonoBehaviour).transform.position - beamPoint.position;
            dir.Normalize();
            beam.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
            beam.localScale = new Vector2(Vector2.Distance(beamPoint.position, (scanned as MonoBehaviour).transform.position), 1.0f + bonusDamage / bonusDamageLimit * 3.0f);
            beam.gameObject.SetActive(true);

            counter += Time.deltaTime;
            if(counter >= beamTickRate)
            {
                counter = 0.0f;
                scanned.OnDamage(damage + bonusDamage);
                bonusDamage = Mathf.Min(bonusDamageLimit, bonusDamage + weighIn);
            }
        }
        else
        {
            counter = 0.0f;
            bonusDamage = 0.0f;
            beam.gameObject.SetActive(false);
        }
    }
}
