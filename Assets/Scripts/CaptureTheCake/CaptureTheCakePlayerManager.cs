using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;

public class CaptureTheCakePlayerManager : NetworkBehaviour
{
    public int maxHealth = 3;
    private int currentHealth;
    public float eatAmount = 10f; // Amount of HP to reduce when eating the cake
    public Cake cake; // Reference to the cake

    [Header("Combat")]
    public int damage = 1;
    public float attackRange = 100f;

    private float attackCooldown;
    private bool canAttack;
    [SerializeField] private GameObject playerRaycastLocation;

    [SerializeField] public CaptureTheCakeManager captureTheCakeManager;

    [Header("Teams")]
    [SerializeField] private NetworkVariable<int> team;

    void Start()
    {
        captureTheCakeManager = GameObject.Find("CaptureTheCakeManager").GetComponent<CaptureTheCakeManager>();
        currentHealth = maxHealth;
        canAttack = true;
    }

    private void Update()
    {
        // Check for interaction with the cake
        if (Input.GetKeyDown(KeyCode.L) && cake != null)
        {
            if (cake.GetCakeTeam() == team.Value)
            {
                Debug.Log("Own Cake, cannot eat");
                return;
            }
            cake.EatCake(eatAmount);
            Debug.Log("Cake being eaten");
        }
        if (Input.GetMouseButtonDown(1) && canAttack)
        {
            RaycastHit hit;
            canAttack = false;
            StartCoroutine(ResetAttackCooldown());
            //Debug.Log("right click pressed");

            if (Physics.Raycast(playerRaycastLocation.transform.position, playerRaycastLocation.transform.forward, out hit, attackRange) && hit.collider.CompareTag("PlayerPush"))
            {
                if (hit.collider.gameObject.GetComponentInParent<NetworkObject>().OwnerClientId == NetworkManager.Singleton.LocalClientId)
                {
                    Debug.Log("Hit self, attack canceled");
                    return;
                }
                Debug.Log("hit");
                Debug.DrawRay(playerRaycastLocation.transform.position, playerRaycastLocation.transform.forward, Color.red, 2);

                Attack(hit);
            }

        }
    }
    void Attack(RaycastHit hit)
    {
        Debug.Log("Raycast hit something: " + hit.collider.name);

#if UNITY_EDITOR
        EditorGUIUtility.PingObject(hit.collider.gameObject);
#endif

        CaptureTheCakePlayerManager playerCCManager = hit.collider.GetComponentInParent<CaptureTheCakePlayerManager>();
        Debug.Log(playerCCManager.currentHealth);

        if (playerCCManager.GetTeam() == team.Value)
        {
            Debug.Log("Other player on same team, cannot attack");
            return;
        }

        Debug.Log($"Host attacked and hit player {playerCCManager.gameObject.GetComponent<NetworkObject>().OwnerClientId}");
        TakeDamageServerRpc(playerCCManager.gameObject.GetComponent<NetworkObject>().OwnerClientId, damage);
        //playerCCManager.TakeDamageClientRpc(playerCCManager.gameObject.GetComponent<NetworkObject>().OwnerClientId, damage); client cannot call clientrpcs
    }

    public IEnumerator ResetAttackCooldown()
    {
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    [ServerRpc(RequireOwnership = false)]
    private void TakeDamageServerRpc(ulong client, int amount)
    {
        TakeDamageClientRpc(client, amount);
    }

    [ClientRpc]
    private void TakeDamageClientRpc(ulong client, int amount)
    {
        if (NetworkManager.Singleton.LocalClientId != client)
        {
            return;
        }
        currentHealth -= amount;
        Debug.Log("Player took damage. Current health: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Player is dead!");
        captureTheCakeManager.KillPlayerServerRpc(NetworkManager.Singleton.LocalClientId);
        currentHealth = maxHealth;
    } //create stop spectating, clinet rpc to set person who died inactive
    

    private void OnTriggerEnter(Collider other)
    {
        // Check if the player is near the cake
        Debug.Log("Entered Collider");
        if (other.CompareTag("Cake"))
        {
            Debug.Log("Set Cake");
            cake = other.GetComponentInParent<Cake>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Clear reference when exiting the trigger
        if (other.CompareTag("Cake"))
        {
            cake = null;
        }
    }

    public void SetTeam(int team)
    {
        this.team.Value = team;
    }

    public int GetTeam()
    {
        return team.Value;
    }
}