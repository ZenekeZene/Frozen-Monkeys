using UnityEngine;
using System.Collections;

public class AjustarTamano : MonoBehaviour {

	[Tooltip ("1 para ajustar a el objeto target. 2 para ajustar a la mitad. Tanto con textures como con sprites.")]
	public float ajustarEnAnchura, ajustarEnAltura;	
	
	void Awake () {
		ajustarTam (ajustarEnAnchura, ajustarEnAltura);
	}
	
	private void ajustarTam(float ajustarEnAnchura, float ajustarEnAltura){
		if (transform.childCount <= 0)
			SpriteFunctions.ResizeSpriteToScreen(this.gameObject, Camera.main, ajustarEnAnchura, ajustarEnAltura);
		else{
			foreach(Transform hijo in transform)
				SpriteFunctions.ResizeSpriteToScreen(hijo.gameObject, Camera.main, ajustarEnAnchura, ajustarEnAltura);
		}
	}
}
