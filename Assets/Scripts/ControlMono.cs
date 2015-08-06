using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class ControlMono : MonoBehaviour {

    public enum Estado { Desnudo, Activo, Muerto, Vestido, Congelado };
	public enum NivelFrio { Normal, PocoFrio, Frio, MuchoFrio }
	public Estado estado;
	public NivelFrio nivelFrio;
    
    private bool estaParado = true;
    public float vel, velMax;
    private int direction;
    public int prendasMax;
    public List<GameObject> prendasPuestas = new List<GameObject>();
    public int vida;
    
	private Rigidbody2D cuerpoFisico;
	private Transform cabeza, tronco, brazo1, antebrazo1, mano1, brazo2, antebrazo2, mano2, pierna1, pierna2, cola;
	private Transform ojo1, ojo2, boca;
	private Animator troncoAnimator;
    public Transform nodoSalida, nodoEntrada;
    
	public int numTapsCongelado;
	public int tapsCongeladoActual = 0;

	public List<Sprite> piezasCara = new List<Sprite>();
	
	private AudioSource[] sonidos;
	public AudioSource sonidoVestir, sonidoDesvestir;

    void Awake() {
		asignarPartesCuerpo();
		asignarPartesCara();
		cuerpoFisico = GetComponent<Rigidbody2D>();
		troncoAnimator = tronco.GetComponent<Animator>();
		sonidos = GetComponents<AudioSource>();
		sonidoVestir = sonidos[0];
		sonidoDesvestir = sonidos[1];
    }

	void Start () {
		NotificationCenter.DefaultCenter().AddObserver(this, "actualizarMonoFrio");
		NotificationCenter.DefaultCenter().AddObserver(this, "monoMuerto");
	}
	
	private void asignarPartesCuerpo(){
		cabeza = transform.FindChild("Cabeza");
		tronco = transform.FindChild ("Tronco");
		brazo1 = tronco.FindChild ("brazo1");
		antebrazo1 = brazo1.FindChild ("antebrazo1");
		mano1 = antebrazo1.FindChild ("mano1");
		brazo2 = tronco.FindChild ("brazo2");
		antebrazo2 = brazo2.FindChild ("antebrazo2");
		mano2 = antebrazo2.FindChild ("mano2");
		cola = transform.FindChild ("Cola");
	}
	
	private void asignarPartesCara(){
		ojo1 = cabeza.FindChild("ojo1");
		ojo2 = cabeza.FindChild("ojo2");
		boca = cabeza.FindChild("boca");
	}
	
	void Update () {
        if (estado == Estado.Activo){    
			float xx = damePosicionEnX();
            if (xx != 0)
				desplazarse(Camera.main.ScreenToWorldPoint(new Vector3(xx, 0, 0)).x);
        } else if (estado == Estado.Congelado){
			if (tapsCongeladoActual < numTapsCongelado){
				if (estaTocando()){
					tapsCongeladoActual++;
					quitarHielo();
				}
			} else if (tapsCongeladoActual >= numTapsCongelado){
				descongelar ();
				tapsCongeladoActual = 0;
			}
		}
	}

    private void desplazarseConFuerzas(int direction){
    	estaParado = false;
    	troncoAnimator.SetBool("EstaParado", estaParado);
        Vector2 desp = new Vector2(vel * direction, 0);
        if (cuerpoFisico.velocity.x < velMax || cuerpoFisico.velocity.x > (-velMax))
            cuerpoFisico.AddForce(desp);
    }
    
	private void desplazarse(float xx){
    	estaParado = false;
    	troncoAnimator.SetBool("EstaParado", estaParado);
    	iTween.MoveUpdate(gameObject, iTween.Hash("position", new Vector3(xx, transform.position.y, transform.position.z), "time", 1f, "onupdate", "moverEnX"));
    }
    
    private void moverEnX(float nuevaX){
    	transform.position = new Vector3(nuevaX, transform.position.y, transform.position.z);
    }

    private void OnCollisionEnter2D(Collision2D otro){
		GameObject objeto = otro.gameObject;
    	if (estado == Estado.Activo){
			if (esUnaPrenda (objeto))
				vestir(objeto);
			if (objeto.CompareTag("bola")){
				Destroy(objeto);
				desvestir ();
			} else if (objeto.CompareTag("hielo"))
				congelar(objeto);
		}
    }
    
	private bool esUnaPrenda(GameObject posiblePrenda){
		return (posiblePrenda.CompareTag("abrigo") || posiblePrenda.CompareTag("gorro") || posiblePrenda.CompareTag("guante"));
	}

    private void vestir(GameObject prenda){
    	string tagPrenda = prenda.tag;
		if (tagPrenda.Equals("gorro")){
			if (comprobarSiYaTengo(prenda) == false){
				ponersePrenda(prenda, cabeza, "cabezaMono", 1);
				prenda.GetComponent<SpriteRenderer>().sprite = prenda.GetComponent<Gorro>().gorroSin;
			}
		} else if (tagPrenda.Equals("abrigo")){
			if (comprobarSiYaTengo(prenda) == false)
				ponersePrenda(prenda, tronco, "Mono", 1);
		} else if (tagPrenda.Equals("guante")){
			if (cuantosGuantesTengo() == 0)
				ponersePrenda(prenda, antebrazo1, mano1, "Mono", 3);
			else if (cuantosGuantesTengo() == 1){
				ponersePrenda(prenda, antebrazo2, mano2, "Mono", 3);
				prenda.transform.localScale = new Vector3(-1, prenda.transform.localScale.y, prenda.transform.localScale.z);
			}
		}
		if (prendasPuestas.Count == prendasMax){
			estado = Estado.Vestido;
			iTween.MoveTo(gameObject, iTween.Hash("position", nodoSalida, "time", 1, "easeType", iTween.EaseType.linear, "oncomplete", "fueraJuego"));
			NotificationCenter.DefaultCenter().PostNotification(this, "monoVestido");
		}
    }
    
	private void ponersePrenda(GameObject prenda, Transform padre, Transform donde, string sortingCapa, int orden){
		ponersePrenda(prenda, donde, sortingCapa, orden);
		prenda.transform.parent = padre;
	}
    
    private void ponersePrenda(GameObject prenda, Transform donde, string sortingCapa, int orden){
		prenda.GetComponent<Rigidbody2D>().isKinematic = true;
		prenda.GetComponent<CircleCollider2D>().enabled = false;
		prenda.transform.position = donde.transform.position;
		prenda.transform.rotation = donde.transform.rotation;
		prenda.transform.parent = donde;
		SpriteRenderer renderer = prenda.GetComponent<SpriteRenderer>();
		renderer.sortingLayerName = sortingCapa;
		renderer.sortingOrder = orden;
		prendasPuestas.Add(prenda);
		NotificationCenter.DefaultCenter().PostNotification(this, "disminuirFrio");
		sonidoVestir.Play();
    }    
	
    private void desvestir(){
		if (prendasPuestas.Count > 0){
			GameObject prenda = buscarPrenda();
			prendasPuestas.Remove(prenda);
			prenda.transform.SetParent(null);
			prenda.GetComponent<Rigidbody2D>().isKinematic = false;
			prenda.GetComponent<Rigidbody2D>().AddForce(new Vector2(5, 5));
			sonidoDesvestir.Play();
		}
    }
    
    private GameObject buscarPrenda(){
    	return (prendasPuestas[Random.Range(0, prendasPuestas.Count)]);
    }
    
    private void congelar(GameObject hielo){
		estado = Estado.Congelado;
		hielo.GetComponent<BoxCollider2D>().enabled = false;
		hielo.GetComponent<Rigidbody2D>().isKinematic = true;
		hielo.transform.SetParent (transform);
		hielo.transform.localScale = new Vector3(5, 8, 5);
		Material material = hielo.GetComponent<SpriteRenderer>().material;
		Color colorOriginal = material.color;
		material.color = new Color(colorOriginal.r, colorOriginal.g, colorOriginal.b, 0.9f);
    }
    
    private void descongelar(){
		estado = Estado.Activo;
		Destroy(transform.GetChild(3).gameObject);
    }
    
	private void quitarHielo(){
		GameObject hielo = transform.GetChild(3).gameObject;
		if (hielo != null){
			Material material = hielo.GetComponent<SpriteRenderer>().material;
			Color colorOriginal = material.color;
			if (colorOriginal.a >= 0.1f)
				material.color = new Color(colorOriginal.r, colorOriginal.g, colorOriginal.b, colorOriginal.a - 0.1f);
		}
	}
    
    private bool comprobarSiYaTengo(GameObject prenda){
    	foreach(GameObject pieza in prendasPuestas){
    		if (prenda.CompareTag(pieza.tag))
    			return true;
    	}
    	return false;
    }
    
	private int cuantosGuantesTengo(){
		int numGuantes = 0;
		foreach(GameObject pieza in prendasPuestas){
			if (pieza.CompareTag("guante"))
				numGuantes++;
		}
		return (numGuantes);
	}

    public void activar(){
        if (estado == Estado.Desnudo)
            iTween.MoveTo(gameObject, iTween.Hash("x", nodoEntrada.position.x, "y", nodoEntrada.position.y, "z", 0, "time", 1, "easeType", iTween.EaseType.linear, "onComplete", "enJuego"));
    }

    private void enJuego(){
        if (estado == Estado.Desnudo){
            estado = Estado.Activo;
            cuerpoFisico.isKinematic = false;
        }
    }
    
	private void fueraJuego(){
		Destroy(gameObject);
	}
	
	private void actualizarMonoFrio(Notification notification){
		if (estado == Estado.Activo){
			nivelFrio = (NivelFrio) notification.data;
			switch (nivelFrio){
				case NivelFrio.Normal:
					ojo1.GetComponent<SpriteRenderer>().sprite = piezasCara[0];
					ojo2.GetComponent<SpriteRenderer>().sprite = piezasCara[1];
					boca.GetComponent<SpriteRenderer>().sprite = piezasCara[8];
					colorearCuerpo(new Vector3(1, 1, 1));
					
					break;
				case NivelFrio.PocoFrio:
					ojo1.GetComponent<SpriteRenderer>().sprite = piezasCara[2];
					ojo2.GetComponent<SpriteRenderer>().sprite = piezasCara[3];
					boca.GetComponent<SpriteRenderer>().sprite = piezasCara[10];
					colorearCuerpo(new Vector3(1, 1, 1));
					break;
				case NivelFrio.Frio:
					ojo1.GetComponent<SpriteRenderer>().sprite = piezasCara[4];
					ojo2.GetComponent<SpriteRenderer>().sprite = piezasCara[5];
					boca.GetComponent<SpriteRenderer>().sprite = piezasCara[10];
					colorearCuerpo(new Vector3(0, 1, 1));
					break;
				case NivelFrio.MuchoFrio:
					ojo1.GetComponent<SpriteRenderer>().sprite = piezasCara[6];
					ojo2.GetComponent<SpriteRenderer>().sprite = piezasCara[7];					
					boca.GetComponent<SpriteRenderer>().sprite = piezasCara[11];
					colorearCuerpo(new Vector3(0, 0.7f, 1));
					break;
			}
		}
	}
	
	private void colorearCuerpo(Vector3 color){
		colorear(tronco, new Vector3(color.x, color.y, color.z));
		colorear(cabeza, new Vector3(color.x, color.y, color.z));
		colorear(cola, new Vector3(color.x, color.y, color.z));
	}
	
	private void colorear(Transform aColorear, Vector3 color){
		if (aColorear.childCount != null ){
			foreach(Transform pieza in aColorear.transform)
				iTween.ColorTo(pieza.gameObject, iTween.Hash ("r", color.x, "g", color.y, "b", color.z, "time", 2));
		}
		iTween.ColorTo(aColorear.gameObject, iTween.Hash ("r", color.x, "g", color.y, "b", color.z, "time", 2));
		
	}

	public float damePosicionEnX(){
		if (Application.platform == RuntimePlatform.Android){
			if (Input.touchCount > 0){
				if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))// && UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject != null)
					return 0;
				if (Input.GetTouch(0).phase == TouchPhase.Began || Input.GetTouch(0).phase == TouchPhase.Moved)
					return Input.GetTouch(0).position.x;
			}
		} else if (Application.platform == RuntimePlatform.WindowsEditor){
			if (Input.GetMouseButton(0)){
				if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject(0))
					return 0;
				return Input.mousePosition.x;
			}
		}
		return 0;
	}

	private bool estaTocando(){
		if (Application.platform == RuntimePlatform.Android){
			if (Input.touchCount > 0){
				if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))// && UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject != null)
					return false;
				if (Input.GetTouch(0).phase == TouchPhase.Began)
					return true;
			}
		} else if (Application.platform == RuntimePlatform.WindowsEditor){
			if (Input.GetMouseButtonDown(0)){
				if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
					return false;
				return true;
			}
		}
		return false;
	}

	private void monoMuerto(Notification notification){
		estado = Estado.Muerto;
	}
}
