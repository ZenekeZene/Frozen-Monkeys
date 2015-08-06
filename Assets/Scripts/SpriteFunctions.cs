using UnityEngine;
using System.Collections;

public static class SpriteFunctions{
	public static void ResizeSpriteToScreen(GameObject theSprite, Camera theCamera, float fitToScreenWidth, float fitToScreenHeight){
		SpriteRenderer sr = theSprite.GetComponent<SpriteRenderer>();
		if (sr != null){
			theSprite.transform.localScale = new Vector3(1,1,1);
			float width = sr.sprite.bounds.size.x;
			float height = sr.sprite.bounds.size.y;
			float worldScreenHeight = (float)(theCamera.orthographicSize * 2.0);
			float worldScreenWidth = (float)(worldScreenHeight / Screen.height * Screen.width);
			if (fitToScreenWidth != 0){
				Vector2 sizeX = new Vector2(worldScreenWidth / width / fitToScreenWidth,theSprite.transform.localScale.y);
				theSprite.transform.localScale = sizeX;
			}
			if (fitToScreenHeight != 0){
				Vector2 sizeY = new Vector2(theSprite.transform.localScale.x, worldScreenHeight / height / fitToScreenHeight);
				theSprite.transform.localScale = sizeY;
			}
		} else {
			float yy = Camera.main.orthographicSize * 2.0f;
			float xx = yy * Screen.width / Screen.height;
			yy = yy / fitToScreenHeight;
			xx = xx / fitToScreenWidth;
			theSprite.gameObject.transform.localScale = new Vector3(xx, yy, 1);
		}
	}
}
