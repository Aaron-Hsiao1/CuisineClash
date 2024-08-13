using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVisual : MonoBehaviour
{
	[SerializeField] private MeshRenderer playerMeshRenderer;

	private Material material;


	private void Awake()
	{
		material = new Material(playerMeshRenderer.material);
		playerMeshRenderer.material = material;
	}

	private void Start()
	{

	}

	public void SetPlayerColor(Color color)
	{
		material.color = color;
	}
}
