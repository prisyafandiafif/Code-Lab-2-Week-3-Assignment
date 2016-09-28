using UnityEngine;
using System.Collections;

public class MatchManagerScript : MonoBehaviour {

	protected GameManagerScript gameManager;    //"protected" means this field is public to child scripts
												//but not to unrelated scripts

	public virtual void Start () {
		gameManager = GetComponent<GameManagerScript>();
	}

	/// <summary>
	/// Checks the entire grid for matches.
	/// </summary>
	/// 
	/// <returns><c>true</c>, if there are any matches, <c>false</c> otherwise.</returns>
	public virtual bool GridHasMatch(){
		bool match = false; //assume there is no match

		//check each square in the grid
		for(int x = 0; x < gameManager.gridWidth; x++){
			for(int y = 0; y < gameManager.gridHeight ; y++){
				if(x < gameManager.gridWidth - 2){	//GridHasHorizontalMatch checks 2 to the right
													//gameManager.gridWidth - 2 ensures you're never extending into
													//a space that doesn't exist
					match = match || GridHasHorizontalMatch(x, y); //if match was ever set to true, it stays true forever
				}
			}
		}

		return match;
	}

	/// <summary>
	/// Check if there is a horizontal match, based on the leftmost token.
	/// </summary>
	/// <returns><c>true</c> there is a horizontal match originating at these coordinates, 
	/// <c>false</c> otherwise.</returns>
	/// <param name="x">The x coordinate of the token to check.</param>
	/// <param name="y">The y coordinate of the token to check.</param>
	public bool GridHasHorizontalMatch(int x, int y){
		//check the token at given coordinates, the token to the right of it, and the token 2 to the right
		GameObject token1 = gameManager.gridArray[x + 0, y];
		GameObject token2 = gameManager.gridArray[x + 1, y];
		GameObject token3 = gameManager.gridArray[x + 2, y];

		if(token1 != null && token2 != null && token3 != null){ //ensure all of the token exists
			SpriteRenderer sr1 = token1.GetComponent<SpriteRenderer>();
			SpriteRenderer sr2 = token2.GetComponent<SpriteRenderer>();
			SpriteRenderer sr3 = token3.GetComponent<SpriteRenderer>();
			
			return (sr1.sprite == sr2.sprite && sr2.sprite == sr3.sprite);  //compare their sprites
																			//to see if they're the same
		} else {
			return false;
		}
	}

	/// <summary>
	/// Determine how far to the right a match extends.
	/// </summary>
	/// <returns>The horizontal match length.</returns>
	/// <param name="x">The x coordinate of the leftmost gameobject in the match.</param>
	/// <param name="y">The y coordinate of the leftmost gameobject in the match.</param>
	public int GetHorizontalMatchLength(int x, int y){
		int matchLength = 1;
		
		GameObject first = gameManager.gridArray[x, y]; //get the gameobject at the provided coordinates

		//make sure the script found a gameobject, and--if so--get its sprite
		if(first != null){
			SpriteRenderer sr1 = first.GetComponent<SpriteRenderer>();

			//compare the gameobject's sprite to the sprite one to the right, two to the right, etc.
			//each time the script finds a match, increment matchLength
			//stop when it's not a match, or if the matches extend to the edge of the play area
			for(int i = x + 1; i < gameManager.gridWidth; i++){
				GameObject other = gameManager.gridArray[i, y];

				if(other != null){
					SpriteRenderer sr2 = other.GetComponent<SpriteRenderer>();

					if(sr1.sprite == sr2.sprite){
						matchLength++;
					} else {
						break;
					}
				} else {
					break;
				}
			}
		}
		
		return matchLength;
	}

	/// <summary>
	/// Destroys all tokens in a match of three or more
	/// </summary>
	/// <returns>The number of tokens destroyed.</returns>
	public virtual int RemoveMatches(){
		int numRemoved = 0;

		//iterate across entire grid, looking for matches
		//wherever a horizontal match of three or more tokens is found, destroy them
		for(int x = 0; x < gameManager.gridWidth; x++){
			for(int y = 0; y < gameManager.gridHeight ; y++){
				if(x < gameManager.gridWidth - 2){

					int horizonMatchLength = GetHorizontalMatchLength(x, y);

					if(horizonMatchLength > 2){

						for(int i = x; i < x + horizonMatchLength; i++){
							GameObject token = gameManager.gridArray[i, y]; 
							Destroy(token);

							gameManager.gridArray[i, y] = null;
							numRemoved++;
						}
					}
				}
			}
		}
		
		return numRemoved;
	}
}
