using UnityEngine;
using System.Collections;

public class RepopulateScript : MonoBehaviour {
	
	protected GameManagerScript gameManager;

	public virtual void Start () {
		gameManager = GetComponent<GameManagerScript>();
	}

	/// <summary>
	/// Add tokens at the top of the grid.
	/// </summary>
	public virtual void AddNewTokensToRepopulateGrid(){

		//iterate across the top row of the grid
		//add a new token in all empty spaces
		for(int x = 0; x < gameManager.gridWidth; x++){
			GameObject token = gameManager.gridArray[x, gameManager.gridHeight - 1];
			if(token == null){
				gameManager.AddTokenToPosInGrid(x, gameManager.gridHeight - 1, gameManager.grid);
			}
		}
	}
}
