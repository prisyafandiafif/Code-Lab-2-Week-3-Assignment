using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MatchManagerScript : MonoBehaviour 
{
	public static MatchManagerScript instance; //ADDED this

	//protected GameManagerScript gameManager;    //"protected" means this field is public to child scripts
												//but not to unrelated scripts

	public List<Vector2> toBeRemoved = new List<Vector2>(); //to store gameobjects/tiles that want to be removed after matching

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
	/// Checks the entire grid for matches.
	/// </summary>
	/// 
	/// <returns><c>true</c>, if there are any matches, <c>false</c> otherwise.</returns>
	public virtual bool GridHasMatch()
	{
		bool match = false; //assume there is no match

		//check each square in the grid
		for(int x = 0; x < GameManagerScript.instance.gridWidth; x++)
		{
			for(int y = 0; y < GameManagerScript.instance.gridHeight ; y++)
			{
				if(x < GameManagerScript.instance.gridWidth - 2) //GridHasHorizontalMatch checks 2 to the right
				{	
					//gameManager.gridWidth - 2 ensures you're never extending into
					//a space that doesn't exist
					match = match || GridHasHorizontalMatch(x, y); //if match was ever set to true, it stays true forever

					/*if (GridHasHorizontalMatch(x,y))
					{
						return true
					}*/
				}

				//ADDED THIS
				if(y < GameManagerScript.instance.gridHeight - 2) //GridHasVerticalMatch checks 2 to the top
				{	
					//gameManager.gridHeight - 2 ensures you're never extending into
					//a space that doesn't exist
					match = match || GridHasVerticalMatch(x, y); //if match was ever set to true, it stays true forever
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
	public bool GridHasHorizontalMatch(int x, int y)
	{
		//check the token at given coordinates, the token to the right of it, and the token 2 to the right
		GameObject token1 = GameManagerScript.instance.gridArray[x + 0, y];
		GameObject token2 = GameManagerScript.instance.gridArray[x + 1, y];
		GameObject token3 = GameManagerScript.instance.gridArray[x + 2, y];

		if(token1 != null && token2 != null && token3 != null) //ensure all of the token exists
		{ 
			SpriteRenderer sr1 = token1.GetComponent<SpriteRenderer>();
			SpriteRenderer sr2 = token2.GetComponent<SpriteRenderer>();
			SpriteRenderer sr3 = token3.GetComponent<SpriteRenderer>();
			
			return (sr1.sprite == sr2.sprite && sr2.sprite == sr3.sprite);  //compare their sprites
																			//to see if they're the same
		} 
		else 
		{
			return false;
		}
	}

	//ADDED THIS
	public bool GridHasVerticalMatch(int x, int y)
	{
		//check the token at given coordinates, the token on the top of it, and the token 2 to the top
		GameObject token1 = GameManagerScript.instance.gridArray[x, y + 0];
		GameObject token2 = GameManagerScript.instance.gridArray[x, y + 1];
		GameObject token3 = GameManagerScript.instance.gridArray[x, y + 2];

		if(token1 != null && token2 != null && token3 != null) //ensure all of the token exists
		{ 
			SpriteRenderer sr1 = token1.GetComponent<SpriteRenderer>();
			SpriteRenderer sr2 = token2.GetComponent<SpriteRenderer>();
			SpriteRenderer sr3 = token3.GetComponent<SpriteRenderer>();
			
			return (sr1.sprite == sr2.sprite && sr2.sprite == sr3.sprite);  //compare their sprites
																			//to see if they're the same
		} 
		else 
		{
			return false;
		}
	}

	/// <summary>
	/// Determine how far to the right a match extends.
	/// </summary>
	/// <returns>The horizontal match length.</returns>
	/// <param name="x">The x coordinate of the leftmost gameobject in the match.</param>
	/// <param name="y">The y coordinate of the leftmost gameobject in the match.</param>
	public int GetHorizontalMatchLength(int x, int y)
	{
		int matchLength = 1;
		
		GameObject first = GameManagerScript.instance.gridArray[x, y]; //get the gameobject at the provided coordinates

		//make sure the script found a gameobject, and--if so--get its sprite
		if(first != null)
		{
			SpriteRenderer sr1 = first.GetComponent<SpriteRenderer>();

			//compare the gameobject's sprite to the sprite one to the right, two to the right, etc.
			//each time the script finds a match, increment matchLength
			//stop when it's not a match, or if the matches extend to the edge of the play area
			for(int i = x + 1; i < GameManagerScript.instance.gridWidth; i++)
			{
				GameObject other = GameManagerScript.instance.gridArray[i, y];

				if(other != null)
				{
					SpriteRenderer sr2 = other.GetComponent<SpriteRenderer>();

					if(sr1.sprite == sr2.sprite)
					{
						matchLength++;
					} 
					else 
					{
						break;
					}
				} 
				else 
				{
					break;
				}
			}
		}
		
		return matchLength;
	}

	//ADDED THIS
	public int GetVerticalMatchLength(int x, int y)
	{
		int matchLength = 1;
		
		GameObject first = GameManagerScript.instance.gridArray[x, y]; //get the gameobject at the provided coordinates

		//make sure the script found a gameobject, and--if so--get its sprite
		if(first != null)
		{
			SpriteRenderer sr1 = first.GetComponent<SpriteRenderer>();

			//compare the gameobject's sprite to the sprite one to the right, two to the right, etc.
			//each time the script finds a match, increment matchLength
			//stop when it's not a match, or if the matches extend to the edge of the play area
			for(int i = y + 1; i < GameManagerScript.instance.gridHeight; i++)
			{
				GameObject other = GameManagerScript.instance.gridArray[x, i];

				if(other != null)
				{
					SpriteRenderer sr2 = other.GetComponent<SpriteRenderer>();

					if(sr1.sprite == sr2.sprite)
					{
						matchLength++;
					} 
					else 
					{
						break;
					}
				} 
				else 
				{
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
	public virtual int RemoveMatches()
	{
		//int numRemoved = 0;

		//ADDED THIS set to be removed length to zero
		if (toBeRemoved.Count > 0)
		{
			toBeRemoved.Clear();
		}

		//iterate across entire grid, looking for matches
		//wherever a horizontal/vertical match of three or more tokens is found, destroy them
		for(int x = 0; x < GameManagerScript.instance.gridWidth; x++)
		{
			for(int y = 0; y < GameManagerScript.instance.gridHeight ; y++)
			{
				if(x < GameManagerScript.instance.gridWidth - 2)
				{
					int horizonMatchLength = GetHorizontalMatchLength(x, y);

					if(horizonMatchLength > 2)
					{
						Debug.Log("Found horizon match > 2 at x: " + x + " and y: " + y);

						//add score ADDED THIS 2
						GameManagerScript.instance.totalScore += GameManagerScript.instance.scoreForOneTokenMatch * horizonMatchLength;

						//add time ADDED THIS 2
						GameManagerScript.instance.AddTime(horizonMatchLength * GameManagerScript.instance.timeAddedForOneTokenMatch);

						for(int i = x; i < x + horizonMatchLength; i++)
						{
							GameObject token = GameManagerScript.instance.gridArray[i, y]; 
							
							//ADDED THIS
							Vector2 index = new Vector2(i * 1f, y * 1f);

							//ADDED THIS if token is not one of the list members
							if (!toBeRemoved.Contains(index))
							{ 
								//Destroy(token);
								//token.transform.parent = GameManagerScript.instance.gameObject.transform;
	
								//put to toBeRemoved list
								toBeRemoved.Add(index);

								//GameManagerScript.instance.gridArray[i, y] = null;
								//numRemoved++;
							}
						}
					}
				}

				Debug.Log("Transition from horizon to vert");

				//ADDED THIS
				if(y < GameManagerScript.instance.gridHeight - 2)
				{
					int vertMatchLength = GetVerticalMatchLength(x, y);

					if(vertMatchLength > 2)
					{
						Debug.Log("Found vert match > 2 at x: " + x + " and y: " + y);

						//add score ADDED THIS 2
						GameManagerScript.instance.totalScore += GameManagerScript.instance.scoreForOneTokenMatch * vertMatchLength;

						//add time ADDED THIS 2
						GameManagerScript.instance.AddTime(vertMatchLength * GameManagerScript.instance.timeAddedForOneTokenMatch);

						for(int i = y; i < y + vertMatchLength; i++)
						{
							GameObject token = GameManagerScript.instance.gridArray[x, i]; 
							
							//ADDED THIS
							Vector2 index = new Vector2(x * 1f, i * 1f);

							//ADDED THIS if token is not one of the list members
							if (!toBeRemoved.Contains(index))
							{ 
								//Destroy(token);
								//token.transform.parent = GameManagerScript.instance.gameObject.transform;
	
								//put to toBeRemoved list
								toBeRemoved.Add(index);

								//GameManagerScript.instance.gridArray[x, i] = null;
								//numRemoved++;
							}
						}
					}
				}
			}
		}
		
		Debug.Log("To Be Removed: " + toBeRemoved.Count);

		//ADDED THIS delete all toberemoved gameobject from array
		for (int i = 0; i < toBeRemoved.Count; i++)
		{
			Destroy(GameManagerScript.instance.gridArray[(int)toBeRemoved[i].x, (int)toBeRemoved[i].y]);
			GameManagerScript.instance.gridArray[(int)toBeRemoved[i].x, (int)toBeRemoved[i].y] = null;
		}

		return toBeRemoved.Count; //ADDED THIS
	}
}
