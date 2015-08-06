using UnityEngine;
using System.Collections;

public class ControlSuelo : MonoBehaviour {
	private string[] tags = {"abrigo", "gorro", "guante", "bola", "hielo"};
	void Start () {
	
	}

    void OnCollisionEnter2D(Collision2D other){
    	foreach(string tag in tags){
    		if (other.gameObject.CompareTag(tag)){
    			Destroy (other.gameObject);
    			break;
    		}
    	}
    }
}
