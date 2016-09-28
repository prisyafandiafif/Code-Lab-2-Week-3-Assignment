﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameManagerScript : MonoBehaviour 
{
	public static GameManagerScript instance; //ADDED this

	public int gridWidth = 8;
	public int gridHeight = 8;
	public float tokenSize = 1;
	
	public int totalScore;
	[HideInInspector] public int scoreForOneTokenMatch = 100;
	public float totalTime;
	private float tempTotalTime;
	[HideInInspector] public float timeAddedForOneTokenMatch = 0.5f;

	public Text scoreText;
	public Image timeBar;

	//protected MatchManagerScript matchManager;
	//protected InputManagerScript inputManager;
	//protected RepopulateScript repopulateManager;
	//protected MoveTokensScript moveTokenManager;

	public GameObject grid;

	//we're declaring a MULTI-DIMENSIONAL ARRAY; this array has two dimensions, so it's a GRID, not a line
	//that will make it easy to track tokens in a grid (which is a two-dimensional shape)
	public GameObject[,] gridArray;

	protected Object[] tokenTypes;

	//TODO: use this variable. It doesn't seem to be used anywhere.
	GameObject selected;

	//ADDED this
	public void Awake ()
	{
		instance = this;
	}

	public virtual void Start () 
	{
		//load the tokens, make the grid, and create references to the other scripts
		tokenTypes = (Object[])Resources.LoadAll("Tokens/");
		gridArray = new GameObject[gridWidth, gridHeight];
		MakeGrid();
		//matchManager = GetComponent<MatchManagerScript>();
		//inputManager = GetComponent<InputManagerScript>();
		//repopulateManager = GetComponent<RepopulateScript>();
		//moveTokenManager = GetComponent<MoveTokensScript>();

		//ADDED THIS
		tempTotalTime = totalTime; 
	}

	public virtual void Update()
	{
		/*gridArray[0,0].GetComponent<SpriteRenderer>().color = Color.red;
		gridArray[0,1].GetComponent<SpriteRenderer>().color = Color.red;
		gridArray[1,0].GetComponent<SpriteRenderer>().color = Color.red;
		gridArray[1,1].GetComponent<SpriteRenderer>().color = Color.red;*/

		/*int countNullGrid = 0;
		for(int x = 0; x < gridWidth; x++)
		{
			for(int y = 0; y < gridHeight; y++)
			{
				if (gridArray[x,y] == null)
				{
					countNullGrid++;
				}
			}
		}
		Debug.Log("Null grid count: " + countNullGrid);*/

		//always update
		scoreText.text = "Score: " + totalScore; 
		totalTime -= Time.deltaTime;
		if (totalTime > 0f)
		{
			timeBar.GetComponent<RectTransform>().localScale = new Vector3(totalTime/tempTotalTime, 1f, 1f);
		}
		else
		{
			timeBar.GetComponent<RectTransform>().localScale = new Vector3(0f, 1f, 1f);
		}

		//every frame, check whether the grid is full of tokens.
		//if all full
		if(!GridHasEmpty())
		{
			//if the grid is full of tokens and has matches, remove them.
			if(MatchManagerScript.instance.GridHasMatch())
			{
				MatchManagerScript.instance.RemoveMatches();
			} 
			else 
			{
				//ADDED THIS
				if(!MoveTokensScript.instance.move)
				{
					//if the grid is full and there are no matches, wait for the player to make a move (and look for it in InputManager)
					InputManagerScript.instance.SelectToken();
				}
			}

		} 
		//otherwise
		else 
		{
			//if nothing is moving
			if(!MoveTokensScript.instance.move)
			{
				//if the icons are currently moving, set them up to move and leave it be
				MoveTokensScript.instance.SetupTokenMove();
			}

			if(!MoveTokensScript.instance.MoveTokensToFillEmptySpaces())
			{
				//if the MoveTokenManager hasn't added any tokens to the grid
				//tell Repopulate Script to add new tokens
				RepopulateScript.instance.AddNewTokensToRepopulateGrid();
			}
		}
	}
	
	public void AddTime (float addedTime)
	{
		if (totalTime + addedTime > tempTotalTime)
		{
			totalTime = tempTotalTime;
		}
		else
		{
			totalTime += addedTime;
		}
	}
	
	/// <summary>
	/// This creates the grid, fills it with tokens, and makes the GameObjects children of a new GameObject, TokenGrid.
	/// It looks like this should only be called once a game.
	/// </summary>
	void MakeGrid() 
	{
		//creates a GameObject called TokenGrid, then makes all of the tokens as children of it
		//this keeps our hierarchy neat
		grid = new GameObject("TokenGrid");

		//then, fill the grid with tokens
		for(int x = 0; x < gridWidth; x++)
		{
			for(int y = 0; y < gridHeight; y++)
			{
				AddTokenToPosInGrid(x, y, grid);
			}
		}
	}

	//checks whether there is an empty space in the grid
	public virtual bool GridHasEmpty()
	{
		//this checks every x and y in the grid
		for(int x = 0; x < gridWidth; x++)
		{
			for(int y = 0; y < gridHeight ; y++)
			{
				if(gridArray[x, y] == null)
				{
					//if this is an empty space, return true (so that we spawn more tokens)
					return true;
				}
			}
		}
		//if all spots in the grid are filled, we returned false (and will spawn no tokens)
		return false;
	}

	/// <summary>
	/// Gets the position of a specific token
	/// </summary>
	/// 
	/// <param name="token">A token GameObject, 'cause Matt likes sports</param>
	/// <returns>The Vector2 coordinate of a token in the grid</returns>
	public Vector2 GetPositionOfTokenInGrid(GameObject token)
	{
		for(int x = 0; x < gridWidth; x++)
		{
			for(int y = 0; y < gridHeight ; y++)
			{
				if(gridArray[x, y] == token)
				{
					return(new Vector2(x, y));
				}
			}
		}
		return new Vector2();
	}
		

	/// <summary>
	/// Converts a grid position into a Worldspace Position
	/// </summary>
	/// 
	/// <param name="x">An int x that is the x coordinate in the grid</param>
	/// <param name="y">An int y that is the y coordinate in the grid</param>
	/// <returns>A Vector2 coordinate in Worldspace</returns>
	public Vector2 GetWorldPositionFromGridPosition(int x, int y)
	{
		return new Vector2(
			(x - gridWidth/2) * tokenSize,
			(y - gridHeight/2) * tokenSize);
	}


	/// <summary>
	/// This creates a random token and puts it into the grid at the given coordinate
	/// (It also makes the token a child of the grid, to keep the hierarchy tidy.) 
	/// </summary>
	/// 
	/// <param name="x">An int x that is the x coordinate in the grid</param>
	/// <param name="y">An int y that is the y coordinate in the grid</param>
	/// <param name="parent">The parent of this token. It should be TokenGrid</param> 
	public void AddTokenToPosInGrid(int x, int y, GameObject parent)
	{
		Vector3 position = GetWorldPositionFromGridPosition(x, y);
		GameObject token = 
			//we create a random kind of token, at that exact position in the grid, with the same rotation as its parent (TokenGrid)
			Instantiate(tokenTypes[Random.Range(0, tokenTypes.Length)], 
			            position, 
			            Quaternion.identity) as GameObject;
		token.transform.parent = parent.transform;
		//then, we put this token into the array of tokens
		gridArray[x, y] = token;

		/*//THIS check if after recreation, the grid is full or not. if full, then make move bool = false
		if (!GridHasEmpty())
		{
			MoveTokensScript.instance.move = false;
		}*/
	}
}