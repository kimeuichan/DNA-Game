using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Login : MonoBehaviour {
	private string u = "username";
	private string p = "password";

	private string usernameString = string.Empty;
	private string passwordString = string.Empty;

	//private Rect windowRect = new Rcet ();
	public DnaInfo.LoginRequest loginReq = new DnaInfo.LoginRequest();

	// Use this for initialization
	void Start () {
		loginReq.Id = "test";
		loginReq.Passwd = "test";
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
