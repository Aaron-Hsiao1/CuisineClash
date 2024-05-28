using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpObject : MonoBehaviour
{
    public GameObject ObjectOnPLayer;

    void Start()
    {
        ObjectOnPlayer.SetActive(false);
    }
    private void OnTriggerStay(Collider other){
        if(other.gameObject.tag -- "Player"){
            if(Input.GetKey(KeyCode.E)){
                this.gameObject.SetActive(false);

                ObjectOnPlayer.SetActive(true);
            }
        }
    }
}
