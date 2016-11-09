using UnityEngine;
using System.Collections;

public class CollisionNetworkScript : Photon.MonoBehaviour {

	void Awake(){
		if(photonView.isMine)
			photonView.RPC("ChangeMyName", PhotonTargets.AllBuffered, PhotonNetwork.playerList.Length.ToString());
	}

	[PunRPC]
	void ChangeMyName(string myNewName){
		gameObject.transform.name = myNewName;
	}

	public Transform bodyObject;
	void OnCollisionEnter(Collision other){
		if(other.transform.tag == "Orb"){

			if(photonView.isMine){
				photonView.RPC("AddThisSnakeNewBodyPart",PhotonTargets.AllBuffered, gameObject.transform.name);
				photonView.RPC("DeleteOrbForOthers", PhotonTargets.AllBuffered, other.gameObject.name);
			}
		}
		if(other.transform.tag == "Enemy"){
			if(photonView.isMine)
				KillBodies();
		}
	}

	public GameObject orbPrefabFromRes;
	void KillBodies(){
		SnakeMovement sM = gameObject.GetComponent<SnakeMovement>();
		for(int i = 0; i < sM.bodyParts.Count; i++){
			PhotonNetwork.Instantiate(orbPrefabFromRes.name, sM.bodyParts[i].gameObject.transform.position, Quaternion.identity, 0);
			Destroy(sM.bodyParts[i].gameObject);
		}
		Destroy(gameObject);
		sM.bodyParts.Clear();
	}

	[PunRPC]
	void DeleteOrbForOthers(string go){
		Destroy(GameObject.Find(go).gameObject);

	}
	[PunRPC]
	void AddThisSnakeNewBodyPart(string gO){
		Transform wantedPlayer = GameObject.Find(gO.ToString()).transform;

		if(wantedPlayer.GetComponent<SnakeMovement>().bodyParts.Count == 0){
			Vector3 currentPos = wantedPlayer.position;
			Transform newBodyPart = Instantiate (bodyObject, currentPos, Quaternion.identity) as Transform;

			newBodyPart.GetComponent<SnakeBody>().head = wantedPlayer;
			if(!photonView.isMine){
				newBodyPart.tag = "Enemy";
			}

			wantedPlayer.GetComponent<SnakeMovement>().bodyParts.Add(newBodyPart);
		}
		else{
			Vector3 currentPos = wantedPlayer.GetComponent<SnakeMovement>().bodyParts[wantedPlayer.GetComponent<SnakeMovement>().bodyParts.Count-1].position;
			Transform newBodyPart = Instantiate (bodyObject, currentPos, Quaternion.identity) as Transform;

			newBodyPart.GetComponent<SnakeBody>().head = wantedPlayer;
			if(!photonView.isMine){
				newBodyPart.tag = "Enemy";
			}

			wantedPlayer.GetComponent<SnakeMovement>().bodyParts.Add(newBodyPart);
		}
	}
}