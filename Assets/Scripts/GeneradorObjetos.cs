using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum hilera { Cima1, Cima2, Cima3};

public class GeneradorObjetos : MonoBehaviour {

	[Tooltip ("x : cadencia minima, y: cadencia maxima [segundos]")]
    public Vector2 cadencia;
    public Transform nodoIzquierda, nodoDerecha;
    private Transform nodoActual;
	[Tooltip ("x : velocidad minima, y: velocidad maxima [segundos]")]
    public Vector2 velocidad;
	public List<GameObject> objetos = new List<GameObject>();
	[Range(0.0f, 100.0f)]
	public int porcentajeEsquimal, porcentajeEsquimalTope;
	private GameObject personajeElegido;
	private GameObject pinguino, esquimal;
	public hilera numHilera;
	
	void Awake () {
		buscarHijos();
		aplicarCapaOrdenAHijos();
		personajeElegido = pinguino;
	}
	
	private void buscarHijos(){
		foreach(Transform hijo in transform){
			if (hijo.CompareTag("pinguino"))
				pinguino = hijo.gameObject;
			else if (hijo.CompareTag("esquimal"))
				esquimal = hijo.gameObject;
		}
		pinguino.SetActive(false);
		esquimal.SetActive(false);
	}
	
	private void aplicarCapaOrdenAHijos(){
		foreach(Transform pieza in pinguino.transform){
			pieza.GetComponent<SpriteRenderer>().sortingLayerName = numHilera.ToString();
		}
		
		foreach(Transform pieza in esquimal.transform){
			pieza.GetComponent<SpriteRenderer>().sortingLayerName = numHilera.ToString();
		}
	}

	void Start () {
        nodoActual = nodoDerecha;
		Invoke("aparecerPersonaje", Random.Range(cadencia.x, cadencia.y));
        desplazarse();
		iTween.ValueTo(gameObject, iTween.Hash("from", cadencia.x, "to", 0.1f, "time", 100, "easeType", iTween.EaseType.linear, "onupdate", "disminuirCadenciaMin"));
		iTween.ValueTo(gameObject, iTween.Hash("from", cadencia.y, "to", 0.1f, "time", 100, "easeType", iTween.EaseType.linear, "onupdate", "disminuirCadenciaMax"));
		iTween.ValueTo(gameObject, iTween.Hash("from", porcentajeEsquimal, "to", porcentajeEsquimalTope, "time", 100, "easeType", iTween.EaseType.linear, "onupdate", "aumentarPorcentajeEsquimales"));
	}

    private void disminuirCadenciaMin(float nuevaCadenciaMin){
		cadencia.x = nuevaCadenciaMin;
    }

    private void disminuirCadenciaMax(float nuevaCadenciaMax){
		cadencia.y = nuevaCadenciaMax;
    }
    
	private void aumentarPorcentajeEsquimales(int nuevoPorcentajeEsquimales){
		porcentajeEsquimal = nuevoPorcentajeEsquimales;
	}

    private void aparecerPersonaje(){
    	iTween.Pause(gameObject);
		int porc = Random.Range(0, 100);
		if (porc <= porcentajeEsquimal)
			personajeElegido = esquimal;
		else
			personajeElegido = pinguino;
		personajeElegido.SetActive(true);
    }

    private void desplazarse(){
		float vel = Random.Range(velocidad.x, velocidad.y);
        nodoActual = (nodoActual.Equals(nodoIzquierda)?nodoDerecha:nodoIzquierda);
        iTween.MoveTo(gameObject, iTween.Hash("position", nodoActual.position, "time", vel, "onComplete", "desplazarse", "easetype", iTween.EaseType.linear));
    }
    
    public void reanudar(){
		iTween.Resume(gameObject);
		personajeElegido = null;
		Invoke ("aparecerPersonaje", Random.Range(cadencia.x, cadencia.y));
    }
}
