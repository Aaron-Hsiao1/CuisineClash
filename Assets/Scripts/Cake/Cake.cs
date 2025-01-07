using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cake : MonoBehaviour
{
    public float maxHP = 100f;  // Maximum health of the cake
    public float currentHP;      // Current health of the cake
    public float decayRate = 1f; // Amount of HP to lose per interaction
    public float decayTime = 2f; // Time in seconds for the decay to occur
    // Start is called before the first frame update
    void Start()
    {
       currentHP = maxHP;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentHP <= 0)
        {
            Destroy(gameObject);
        }
    }
    public void EatCake(float amount)
    {
        if (currentHP > 0)
        {
            currentHP -= amount;

            // Start decay over time
            StartCoroutine(DecayOverTime());
        }
    }
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
    }
}
