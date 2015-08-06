using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class controlLanzarObjeto : MonoBehaviour {

	public List<GameObject> objetos = new List<GameObject>();

	void Start () {
	
	}
	
	void OnEnable(){
		iTween.MoveTo(gameObject, iTween.Hash ("position", new Vector3(transform.position.x, transform.position.y + 3, transform.position.z), "time", 0.4f, "easeType", iTween.EaseType.linear));
	}

	public void lanzarObjeto(){
		GameObject objetoPrefab = objetos[Random.Range (0, objetos.Count)];
		GameObject obj = Instantiate(objetoPrefab, new Vector3(transform.position.x, transform.position.y, 0), Quaternion.identity) as GameObject;
		if (!obj.CompareTag("bola") && !obj.CompareTag("hielo")){
			obj.GetComponent<SpriteRenderer>().color = new Color((float)Random.Range (0, 100)/100, (float)Random.Range (0, 100)/100, (float)Random.Range (0, 100)/100, 1f);
			obj.transform.Rotate(new Vector3(0, 0, Random.Range (0, 360)));
		}
		audio.Play ();
	}
	
	public void desaparecer(){
		iTween.MoveTo(gameObject, iTween.Hash ("position", new Vector3(transform.position.x, transform.position.y - 3, transform.position.z), "time", 0.4f, "easeType", iTween.EaseType.linear, "onComplete", "personajeDesaparecido"));
		GetComponent<Animator>().SetBool("disparar", false);
	}
	
	private void personajeDesaparecido(){
		transform.position = transform.parent.position;
		transform.gameObject.SetActive(false);
		transform.parent.GetComponent<GeneradorObjetos>().reanudar();
	}
}
