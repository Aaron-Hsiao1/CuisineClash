using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Cinemachine;

public class GoGurtCameraFollo : NetworkBehaviour
{
    public Vector3 offset;
    private Transform player;
    private GoKartMovement playerScript;

    public Vector3 origCamPos;
    public Vector3 boostCamPos;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        {
            gameObject.SetActive(false); // Disable camera for non-owners
            return;
        }

        // Find the player's kart (assuming the player is the owner of a kart)
        player = transform.parent; // Ensure the camera is attached to the correct player
        if (player != null)
        {
            playerScript = player.GetComponent<GoKartMovement>();
        }
    }

    void LateUpdate()
    {
        if (!IsOwner || player == null || playerScript == null)
            return;

        transform.position = player.position + offset;

        if (!playerScript.GLIDER_FLY)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, player.rotation, 3 * Time.deltaTime);
        }
        else
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, player.eulerAngles.y, 0), 3 * Time.deltaTime);
        }

        if (playerScript.BoostTime > 0)
        {
            transform.GetChild(0).localPosition = Vector3.Lerp(transform.GetChild(0).localPosition, boostCamPos, 3 * Time.deltaTime);
        }
        else
        {
            transform.GetChild(0).localPosition = Vector3.Lerp(transform.GetChild(0).localPosition, origCamPos, 3 * Time.deltaTime);
        }
    }
}
