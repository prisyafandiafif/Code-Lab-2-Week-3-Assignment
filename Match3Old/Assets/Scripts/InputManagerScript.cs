using UnityEngine;
using System.Collections;

public class InputManagerScript : MonoBehaviour {

	//this script tracks player inputs and makes changes to the game

	protected GameManagerScript gameManager;
	protected MoveTokensScript moveManager;

	//selected is a variable which tracks which token (if any) we've currently selected
	protected GameObject selected = null;

	public virtual void Start () {
		moveManager = GetComponent<MoveTokensScript>();
		gameManager = GetComponent<GameManagerScript>();
	}
		
	public virtual void SelectToken(){
		if(Input.GetMouseButtonDown(0)){
			//when you click, check where on the screen you're clicking
			Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			//use a 2D raycast to see what you're clicking on
			Collider2D collider = Physics2D.OverlapPoint(mousePos);

			if(collider != null){
				//if you click on something...
				if(selected == null){
					//if we haven't yet selected a token, select this token and remember it
					selected = collider.gameObject;				
				} else {
					//if we HAVE already selected a token, calculate the distance between this token (which we're currently clicking on)
					//and that one (which we clicked on last time)
					Vector2 pos1 = gameManager.GetPositionOfTokenInGrid(selected);
					Vector2 pos2 = gameManager.GetPositionOfTokenInGrid(collider.gameObject);

					//if they're next to each other, we swap them
					if(Mathf.Abs((pos1.x - pos2.x) + (pos1.y - pos2.y)) == 1){
						moveManager.SetupTokenExchange(selected, pos1, collider.gameObject, pos2, true);
					}
					//then deselect our current token (because we're about to destroy or forget it)
					selected = null;
				}
			}
		}

	}

	/// <summary>
	/// This seems like a pretty dumb function. 
	/// Maybe Matt just put it here to show some cool commenting tricks?
	/// Maybe these comments aren't even right?
	/// </summary>
	/// 
	/// <param name="x">A float x that will be divided</param>
	/// <param name="y">A float x that will be divided</param>
	/// 
	/// <returns>The value of param x plus y</returns>
	///
	private int CommentFunc(int x, int y){
		return x - y;
	}
}
