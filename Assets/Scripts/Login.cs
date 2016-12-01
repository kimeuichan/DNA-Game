using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Login : MonoBehaviour {

	private PB_handler handler;
	private string usernameString = string.Empty;

	private Rect windowRect = new Rect (0, 0, Screen.width, Screen.width);

	void Start(){
		handler = GameObject.Find ("PB_handler").GetComponent<PB_handler>();
	}

	void OnGUI(){
		GUI.Window (0, windowRect, WindowFuction, "Login");
	}

	void WindowFuction(int windowId){
		usernameString = GUI.TextField (new Rect (Screen.width / 3, 2 * Screen.width / 5, Screen.width / 3, Screen.height / 5), usernameString, 10);
		if (GUI.Button (new Rect (Screen.width / 2, 4 * Screen.height / 5, Screen.width / 8, Screen.height / 8), "Login")) {
			if (usernameString != null && usernameString.Length != 0) {
				// Requset Login with user name
				handler.LoginReq(usernameString);
				GUI.Label (new Rect (Screen.width / 3, 35 * Screen.height / 100, Screen.width / 5, Screen.height / 8), "Username");
			}
		}
	}
}
