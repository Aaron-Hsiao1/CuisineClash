using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVisual : MonoBehaviour
{
	[SerializeField] private SkinnedMeshRenderer playerMeshRenderer;

	//[SerializeField] private Material materialOuter;
	//[SerializeField] private Material materialInner;


	private void Awake()
	{
		//material = new Material(playerMeshRenderer.material);
		//playerMeshRenderer.material = material;
	}

	private void Start()
	{

	}

	public void SetPlayerColor(Color outerColor, Color innerColor)
	{
		playerMeshRenderer.materials[1].color = outerColor;
		playerMeshRenderer.materials[0].color = innerColor;

		playerMeshRenderer.materials[1].SetColor("_EmissionColor", outerColor);
		playerMeshRenderer.materials[0].SetColor("_EmissionColor", innerColor);
	}
}
