using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public class FishSlapIndicator : NetworkBehaviour
{
    public Image indicatorImage;  // The image to represent the indicator
    public float growthSpeed = 0.5f; // Speed at which the indicator grows
    public float maxGrowth = 1f; // Maximum scale of the indicator (1 is 100%)

    public GameObject attackIcon;

    private bool isGrowing = false;

    public KOTGAttack KOTGA;

    // Start is called before the first frame update
    void Start()
    {
        if (!IsLocalPlayer)
        {
            indicatorImage.gameObject.SetActive(false);
        }
        if (SceneManager.GetActiveScene().name != "KingOfTheGrill")
        {
            attackIcon.SetActive(false);
        }

        if (indicatorImage == null)
        {
            indicatorImage = GetComponent<Image>();
        }

        indicatorImage.fillAmount = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (KOTGA.isCharging)
        {
            isGrowing = true;
        }
        else
        {
            isGrowing = false;
            indicatorImage.fillAmount = 0f;
        }


        if (isGrowing)
        {
            // Increase the fill amount of the image (this is how the growth is represented)
            indicatorImage.fillAmount += growthSpeed * Time.deltaTime;

            // Ensure the indicator doesn't exceed the max value
            if (indicatorImage.fillAmount >= maxGrowth)
            {
                indicatorImage.fillAmount = maxGrowth;
                isGrowing = false; // Stop growing after reaching max
            }
        }
    }

    public void StartGrowth()
    {
        isGrowing = true;
    }

    // Call this method to stop the growth (e.g., on attack release)
    public void StopGrowth()
    {
        isGrowing = false;
    }
}
