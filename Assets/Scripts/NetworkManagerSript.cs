using UnityEngine;
using System.Collections;
using Google.Protobuf;

public class NetworkManagerSript : MonoBehaviour {

	private RoomInfo[] roomsList;

	public GameObject ourSnakeHead;

	void Start(){
		PhotonNetwork.ConnectUsingSettings("0.1");
	}

	void OnGUI(){

		if(!PhotonNetwork.connected){
			GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());
		}
		else if(PhotonNetwork.room == null){
			if(GUI.Button(new Rect(100,100,250,100), "Start Server")){
				PhotonNetwork.CreateRoom("Slither room", new RoomOptions(){ maxPlayers = 15 }, null);
			}
			if(roomsList != null){
				for(int i = 0; i < roomsList.Length; i++){
					if(GUI.Button(new Rect(100,250 + (110 * i), 250,100), "Join this room")){
						PhotonNetwork.JoinRoom(roomsList[i].name);
					}
				}
			}
		}

	}

	void OnReceivedRoomListUpdate(){
		roomsList = PhotonNetwork.GetRoomList();
	}

	void OnJoinedRoom(){
		Debug.Log("Connected to the room");
		PhotonNetwork.Instantiate(ourSnakeHead.transform.name, Vector3.zero, Quaternion.identity, 0);
	}
}