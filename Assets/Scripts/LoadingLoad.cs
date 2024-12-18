using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI; 

public class LoadingLoad : MonoBehaviour
{
    public TextMeshProUGUI loadingText;
    private int dotCount = 0;

    void Start()
    {
        //StartCoroutine(UpdateLoadingText());
    }

    IEnumerator UpdateLoadingText()
    {
        while (true)
        {
            dotCount = (dotCount + 1) % 4; 
            loadingText.text = "Loading" + new string('.', dotCount);  
            yield return new WaitForSeconds(0.1f); 
        }
    }
}