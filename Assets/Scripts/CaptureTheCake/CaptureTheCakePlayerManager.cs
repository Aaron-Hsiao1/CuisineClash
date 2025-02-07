using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Burst.CompilerServices;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class CaptureTheCakePlayerManager : NetworkBehaviour
{
    [Header("Cake Eating")]
    public float eatAmount = 1f; // Amount of HP to reduce when eating the cake
    public Cake cake; // Reference to the cake
    private float eatCooldown = 0.5f;
    private bool canEat = true;

    [Header("Combat")]
    public int damage = 1;
    [SerializeField] private int attackWindupTime;
    [SerializeField] GameObject attackCollider;
    public List<CaptureTheCakePlayerManager> playersInAttackCollider = new List<CaptureTheCakePlayerManager>();
    public int maxHealth = 3;
    private int currentHealth;
    [SerializeField] private HealthBar healthBar;

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
        healthBar.SetMaxHealth(maxHealth);
        canAttack = true;
    }

    /*
    private void OnDestroy()
    {
        captureTheCakeManager.AllPlayersSpawned -= CaptureTheCakePlayerManager_AllPlayersSpawned;
    }*/
    //d
    private void Update()
    {
        // Check for interaction with the cake
        if (Input.GetKeyDown(KeyCode.L) && cake != null)
        {
            if (cake.GetCakeTeam() == team.Value || !canEat)
            {
                Debug.Log("Own Cake, cannot eat or eating on cooldown");
                return;
            }
            cake.EatCake(eatAmount);
            canEat = false;
            StartCoroutine(ResetCakeEatingCd());
            Debug.Log("Cake being eaten");
        }
        if (Input.GetMouseButtonDown(1) && canAttack && captureTheCakeManager.IsGamePlaying())
        {
            canAttack = false;
            Debug.Log("can attack set to false and  attack windup started");
            StartCoroutine(StartAttackWindup());
            
            /*RaycastHit hit;
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
            }*/

        }
    }

    private void Attack()
    {
        Debug.Log("Players in attack range: " + playersInAttackCollider.Count);
        foreach (CaptureTheCakePlayerManager player in playersInAttackCollider)
        {

#if UNITY_EDITOR
            EditorGUIUtility.PingObject(player.gameObject);
#endif
            Debug.Log("player.GetTeam(): " + player.GetTeam());
            Debug.Log("team.Value: " + team.Value);
            if (player.GetTeam() == team.Value || player.gameObject.GetComponent<NetworkObject>().OwnerClientId == NetworkManager.Singleton.LocalClientId) //null refernce
            {
                Debug.Log("Other player on same team, cannot attack");
                return;
            }
            Debug.Log($"attacked and hit player {player.gameObject.GetComponent<NetworkObject>().OwnerClientId}");
            TakeDamageServerRpc(player.gameObject.GetComponent<NetworkObject>().OwnerClientId, damage);
        }
    }

    /*void Attack(RaycastHit hit)
    {
        Debug.Log("Raycast hit something: " + hit.collider.name);

#if UNITY_EDITOR
        //EditorGUIUtility.PingObject(hit.collider.gameObject);
#endif

        CaptureTheCakePlayerManager playerCCManager = hit.collider.GetComponentInParent<CaptureTheCakePlayerManager>();
        Debug.Log(playerCCManager.currentHealth);

        if (playerCCManager.GetTeam() == team.Value)
        {
            Debug.Log("Other player on same team, cannot attack");
            return;
        }

        Debug.Log($"attacked and hit player {playerCCManager.gameObject.GetComponent<NetworkObject>().OwnerClientId}");
        TakeDamageServerRpc(playerCCManager.gameObject.GetComponent<NetworkObject>().OwnerClientId, damage);
        //playerCCManager.TakeDamageClientRpc(playerCCManager.gameObject.GetComponent<NetworkObject>().OwnerClientId, damage); client cannot call clientrpcs
    }*/

    public IEnumerator ResetAttackCooldown()
    {
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    [ServerRpc(RequireOwnership = false)]
    private void TakeDamageServerRpc(ulong client, int amount)
    {
        CaptureTheCakePlayerManager clientToTakeDamage = NetworkManager.Singleton.ConnectedClients[client].PlayerObject.transform.GetComponent<CaptureTheCakePlayerManager>();
#if UNITY_EDITOR
        EditorGUIUtility.PingObject(NetworkManager.Singleton.ConnectedClients[client].PlayerObject.gameObject);
#endif
        clientToTakeDamage.TakeDamageClientRpc(client, amount);
    } //runs on player that did the attack on the host

    [ClientRpc]
    private void TakeDamageClientRpc(ulong client, int amount)
    {
        if (NetworkManager.Singleton.LocalClientId != client)
        {
            return;
        }
        Debug.Log("take damage client rpc called on: " + transform.GetComponent<NetworkObject>().NetworkObjectId);
        CaptureTheCakePlayerManager playerObjectCCPlayerManager = NetworkManager.Singleton.LocalClient.PlayerObject.gameObject.GetComponent<CaptureTheCakePlayerManager>();
        currentHealth -= amount;
        healthBar.SetHealth(currentHealth);
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
        healthBar.SetHealth(maxHealth);
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
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player walked into attack range");
            playersInAttackCollider.Add(other.transform.gameObject.GetComponentInParent<CaptureTheCakePlayerManager>());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Clear reference when exiting the trigger
        if (other.CompareTag("Cake"))
        {
            cake = null;
        }
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player left attack range");
            playersInAttackCollider.Remove(other.transform.gameObject.GetComponentInParent<CaptureTheCakePlayerManager>());
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

    private IEnumerator ResetCakeEatingCd()
    {
        yield return new WaitForSeconds(eatCooldown);
        canEat = true;
    }

    private IEnumerator StartAttackWindup()
    {
        Debug.Log("Attack windup started");
        yield return new WaitForSeconds(attackWindupTime);
        Debug.Log("Attack windup ended, attacking");
        Attack();
        StartCoroutine(ResetAttackCooldown());
    }

}
