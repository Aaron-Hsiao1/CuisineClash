using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KOTGAttackCooldownUi : MonoBehaviour
{
    public Image imageCooldown;
    public KOTGAttack KOTGA;

    private bool isCooldown = false;
    private float cooldownTime = 2.0f;
    private float cooldownTimer = 0.0f;


    // Start is called before the first frame update
    void Start()
    {
        imageCooldown.fillAmount = 0.0f;
    }

     void ApplyCooldown()
     {
        cooldownTimer -= Time.deltaTime;

        if(cooldownTimer < 0.0f)
        {
            isCooldown = false;
            imageCooldown.fillAmount = 0.0f;
        }
        else
        {
            imageCooldown.fillAmount = cooldownTimer / cooldownTime;
        }
     }

     public void UseAttack()
     {
        if(isCooldown)
        {
            //idk
        }
        else
        {
            isCooldown = true;
            cooldownTimer = cooldownTime;
        }
     }

    // Update is called once per frame
    void Update()
    {
        if(KOTGA.isDashing)
        {
            UseAttack();
        }
        if (isCooldown)
        {
            ApplyCooldown();
        }
    }
}
