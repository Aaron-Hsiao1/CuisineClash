using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnifeScript : MonoBehaviour
{
    public int damage = 1;
    public float attackRange = 10f;
    public LayerMask playerLayer;

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Debug.Log("Right Click - Attack initiated!");
            Attack();
        }
    }

    void Attack()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        Debug.DrawRay(ray.origin, ray.direction * attackRange, Color.red, 1.0f); // Draw the ray

        if (Physics.Raycast(ray, out hit, attackRange))
        {
            Debug.Log("Raycast hit something: " + hit.collider.name);

            CCPlayerHealth playerHealth = hit.collider.GetComponent<CCPlayerHealth>();
            if (hit.collider.CompareTag("Player"))
            {
                playerHealth.TakeDamage(damage);
                Debug.Log("Player attacked and hit another player.");
            }
            else
            {
                Debug.Log("Raycast hit, but not a player.");
            }
        }
        else
        {
            Debug.Log("Raycast did not hit anything.");
        }
    }
}
