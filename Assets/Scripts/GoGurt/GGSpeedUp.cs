using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

public class GGSpeedUp : MonoBehaviour
{
    private float BoostTime = 2.5f;
    
    private void OnTriggerEnter(Collider trigger)
    {
        if (trigger.gameObject.CompareTag("Player"))
        {
            BoostTime -= Time.deltaTime;
            if (BoostTime > 0)
            {
                for (int i = 0; i < trigger.gameObject.GetComponent<GoKartMovement>().boostFire.childCount; i++)
                {
                    if (!trigger.gameObject.GetComponent<GoKartMovement>().boostFire.GetChild(i).GetComponent<ParticleSystem>().isPlaying)
                    {
                        trigger.gameObject.GetComponent<GoKartMovement>().boostFire.GetChild(i).GetComponent<ParticleSystem>().Play();
                    }
                }
                trigger.gameObject.GetComponent<GoKartMovement>().MaxSpeed = 200f;


                trigger.gameObject.GetComponent<GoKartMovement>().CurrentSpeed = Mathf.Lerp(trigger.gameObject.GetComponent<GoKartMovement>().CurrentSpeed, 900, 1 * Time.deltaTime);
               
            }
            else
            {
                for (int i = 0; i < trigger.gameObject.GetComponent<GoKartMovement>().boostFire.childCount; i++)
                {
                    trigger.gameObject.GetComponent<GoKartMovement>().boostFire.GetChild(i).GetComponent<ParticleSystem>().Stop();
                }
                trigger.gameObject.GetComponent<GoKartMovement>().MaxSpeed = 60f;
            }
        }

            /*
           trigger.gameObject.GetComponent<GoKartMovement>().BoostTime -= Time.deltaTime;
            Debug.Log("Trigger entered");
            if (trigger.gameObject.CompareTag("Player"))
            {
                for (int i = 0; i < trigger.gameObject.GetComponent<GoKartMovement>().boostFire.childCount; i++)
                {
                    if (!trigger.gameObject.GetComponent<GoKartMovement>().boostFire.GetChild(i).GetComponent<ParticleSystem>().isPlaying)
                    {
                        trigger.gameObject.GetComponent<GoKartMovement>().boostFire.GetChild(i).GetComponent<ParticleSystem>().Play();
                    }
                }
                Debug.Log("Boost activated");
                trigger.gameObject.GetComponent<GoKartMovement>().CurrentSpeed = +100f;
            }
            StartCoroutine(BoostTimer());
            */
        }

    IEnumerator BoostTimer()
    {
        yield return new WaitForSeconds(10f);
        gameObject.GetComponent<GGSpeedUp>().enabled = false;

    }
}
