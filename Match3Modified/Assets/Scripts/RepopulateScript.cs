using UnityEngine;
using System.Collections;

public class RepopulateScript : MonoBehaviour 
{
	public static RepopulateScript instance; //ADDED this

	//protected GameManagerScript gameManager;

	//ADDED this
	public void Awake ()
	{
		instance = this;
	}

	public virtual void Start () 
	{
		//gameManager = GetComponent<GameManagerScript>();
	}

	/// <summary>
	/// Add tokens at the top of the grid.
	/// </summary>
	public virtual void AddNewTokensToRepopulateGrid()
	{
		Debug.Log("Repopulate grid!");

		//iterate across the top row of the grid
		//add a new token in all empty spaces
		for(int x = 0; x < GameManagerScript.instance.gridWidth; x++)
		{
			GameObject token = GameManagerScript.instance.gridArray[x, GameManagerScript.instance.gridHeight - 1];

			if(token == null)
			{
				GameManagerScript.instance.AddTokenToPosInGrid(x, GameManagerScript.instance.gridHeight - 1, GameManagerScript.instance.grid);
			}
		}

		//ADDED THIS check if after recreation, the grid is full or not. if full, then make move bool = false
		if (!GameManagerScript.instance.GridHasEmpty())
		{
			MoveTokensScript.instance.move = false;
		}
	}
}
