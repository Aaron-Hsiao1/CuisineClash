using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class ScoreManager : NetworkBehaviour
{
	public static ScoreManager instance;

	[SerializeField] private KingOfTheGrillManager kingOfTheGrillManager;

	[SerializeField] private TMP_Text scoreText;

	int score = 0;

	private void Awake()
	{
		instance = this;
	}

	// Start is called before the first frame update
	void Start()
	{
		scoreText.text = "Score: " + score.ToString();
	}

	public void AddPoint()
	{
		AddPointClientRpc();
	}

	[ClientRpc]
	private void AddPointClientRpc()
	{
		kingOfTheGrillManager.AddScore(1);
	}

	public int ReturnPlayerPoints()
	{
		return score;
	}
}
