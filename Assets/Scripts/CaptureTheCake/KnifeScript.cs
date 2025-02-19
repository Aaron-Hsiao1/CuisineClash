using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnifeScript : MonoBehaviour
{
    public int damage = 1;
    public float attackRange = 100f;
    public LayerMask playerLayer;

    private float attackCooldown;
    private bool canAttack;

    [SerializeField] private GameObject playerRaycastLocation;


    void Update()
    {
        if (Input.GetMouseButtonDown(1) && canAttack)
        {
            RaycastHit hit;
            canAttack = false;
            StartCoroutine(ResetAttackCooldown());
            Debug.Log("B pressed");

            if (Physics.Raycast(playerRaycastLocation.transform.position, playerRaycastLocation.transform.forward, out hit, attackRange) && hit.collider.CompareTag("PlayerPush"))
            {
                Debug.Log("hit");
                Debug.DrawRay(playerRaycastLocation.transform.position, playerRaycastLocation.transform.forward, Color.red, 2);

                Attack(hit);
            }

        }
    }   

    void Attack(RaycastHit hit)
    {
        Debug.Log("Raycast hit something: " + hit.collider.name);

        CaptureTheCakePlayerManager playerHealth = hit.collider.GetComponentInParent<CaptureTheCakePlayerManager>();
        Debug.Log(playerHealth);
        if (hit.collider.CompareTag("Player"))
        {
            //playerHealth.TakeDamage(damage);
            Debug.Log("Player attacked and hit another player.");
        }
        else
        {
            Debug.Log("Raycast hit, but not a player.");
        }
    }

    public IEnumerator ResetAttackCooldown()
    {
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }
}