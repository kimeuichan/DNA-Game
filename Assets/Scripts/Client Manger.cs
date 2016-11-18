using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientManger : MonoBehaviour {

	public NetworkClient client = new NetworkClient();
	// Use this for initialization
	void Start () {
		client.StartClient ();
	}


		
}
