using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.VisualScripting;

public class Cake : NetworkBehaviour
{
    public float maxHP = 100f;  // Maximum health of the cake
    public NetworkVariable<float> currentHp; //Current HP of the cake
    [SerializeField] private NetworkVariable<int> cakeTeam = new NetworkVariable<int>();
    //public float decayTime = 2f; // Time in seconds for the decay to occur
    // Start is called before the first frame update
    void Start()
    {
        currentHp = new NetworkVariable<float>();

        currentHp.Value = maxHP;
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsHost)
        {
            return;
        }
        if (currentHp.Value <= 0)
        {
            gameObject.GetComponent<NetworkObject>().Despawn(true);
            //Destroy(gameObject);
        }
    }
    public void EatCake(float damage)
    {
        EatCakeServerRpc(damage);
    }
    public int GetCakeTeam()
    {
        return cakeTeam.Value;
    }

    [ServerRpc(RequireOwnership = false)]
    private void EatCakeServerRpc(float damage)
    {
        if (currentHp.Value > 0)
        {
            currentHp.Value -= damage;
            Debug.Log("cake took damage");
        }
    }

    /*
    private IEnumerator DecayOverTime()
    {
        float elapsedTime = 0f;

        while (elapsedTime < decayTime && currentHP > 0)
        {
            elapsedTime += Time.deltaTime;
            yield return null; // Wait until the next frame
        }

        // Ensure we don't go below 0 HP
        currentHP = Mathf.Max(0, currentHP);
    }*/
}