using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Login : MonoBehaviour {
	private string u = "username";

	private NetworkClient client;

	private string usernameString = string.Empty;

	private byte[] send_data = new byte[1024];

	private Rect windowRect = new Rect (0, 0, Screen.width, Screen.width);
	private DnaInfo.LoginRequest loginReq = new DnaInfo.LoginRequest();

	// Update is called once per frame

	void Start(){
		client = GameObject.Find ("Network Client").GetComponent<NetworkClient> ();
	}

	void OnGUI(){
		GUI.Window (0, windowRect, WindowFuction, "Login");
	}

	void WindowFuction(int windowId){
		usernameString = GUI.TextField (new Rect (Screen.width / 3, 2 * Screen.width / 5, Screen.width / 3, Screen.height / 5), usernameString, 10);

		if (GUI.Button (new Rect (Screen.width / 2, 4 * Screen.height / 5, Screen.width / 8, Screen.height / 8), "Login")) {
			if (usernameString != null) {
				// Set Id
				loginReq.Id = usernameString;

				Google.Protobuf.CodedOutputStream reqStream = new Google.Protobuf.CodedOutputStream (send_data);
				// Make MemoryStream
				loginReq.WriteTo (reqStream);

				// Data Send
				client.Send (send_data, send_data.Length, (int)DnaInfo.packet_type.LoginReq);
			}
		}
	}
}
