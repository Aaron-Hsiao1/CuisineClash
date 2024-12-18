using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class RecipeSO : ScriptableObject
{
	public List<IngredientSO> recipeSO; //List of ingredients from top to bottom
	public string recipeName;
}
