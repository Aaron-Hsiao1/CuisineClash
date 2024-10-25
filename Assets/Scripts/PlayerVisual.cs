using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVisual : MonoBehaviour
{
	[SerializeField] private MeshRenderer playerMeshRenderer;

	[SerializeField] private Material materialOuter;
    [SerializeField] private Material materialInner;


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
		materialOuter.color = outerColor;
		materialInner.color = innerColor;
	}
}
