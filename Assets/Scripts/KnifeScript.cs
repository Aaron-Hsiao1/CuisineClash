using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnifeScript : MonoBehaviour
{
    public int damage = 1;
    public float attackRange = 2f;
    public LayerMask playerLayer;

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Attack();
        }
    }

    void Attack()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        
        if (Physics.Raycast(transform.position, transform.forward, out hit, attackRange, playerLayer))
        {
            CCPlayerHealth playerHealth = hit.collider.GetComponent<CCPlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
                Debug.Log("Player attacked and hit another player.");
            }
            else
            {
                Debug.Log("Player attacked but did not hit a player.");
            }
        }
    }
}

