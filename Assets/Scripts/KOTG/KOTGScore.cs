using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class KOTGScore : MonoBehaviour
{
    public TMP_Text myText;

    public Player player;
    public ScoreManager SM;

    // Start is called before the first frame update
    void Start()
    {
        myText.text = "1." + ": " + SM.ReturnPlayerPoints();
    }

    // Update is called once per frame
    void Update()
    {
        myText.text = "1." + ": " + SM.ReturnPlayerPoints();
    }
}
