using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SnakeMovement : Photon.MonoBehaviour {

	public List<Transform> bodyParts = new List<Transform>();

	public float velocity;
	private Vector3 lastFrame;
	void Update () {
		if(photonView.isMine){
			MouseRotationSnake();
			SpawnOrbManager();
			Running();
			Scaling();
		}
		ColorMySnake();


		if(velocity > 0.08f)
			MakeOurSnakeGlow(true);
		else
			MakeOurSnakeGlow(false);

	}

	public Material purple, orange;
	void ColorMySnake(){
		for(int i = 0; i < bodyParts.Count; i++){
			if(i % 2 == 0){
				bodyParts[i].GetComponent<Renderer>().material = purple;
			}
			else{
				bodyParts[i].GetComponent<Renderer>().material = orange;
			}
		}
	}

	public float spawnOrbEveryXSeconds = 1;
	public GameObject orbPrefab;
	void SpawnOrbManager(){
		//if(photonView.isMine)
		StartCoroutine("CallEveryFewSeconds", spawnOrbEveryXSeconds);
	}
	IEnumerator CallEveryFewSeconds(float x){
		yield return new WaitForSeconds(x);

		float radiusSpawn = 5;
		Vector3 randomNewOrbPosition = new Vector3(
			Random.Range(
				Random.Range(transform.position.x - 10, transform.position.x - 5),
				Random.Range(transform.position.x + 5, transform.position.x + 10)
			),
			Random.Range(
				Random.Range(transform.position.y - 10, transform.position.y - 5),
				Random.Range(transform.position.y + 5, transform.position.y + 10)
			),
			0
		);
		Vector3 direction = randomNewOrbPosition - transform.position;
		Vector3 finalPosition = transform.position + (direction.normalized * radiusSpawn);
		int randomNum = Random.Range(1,100000);

		photonView.RPC("SpawnOrbFromHead", PhotonTargets.AllBuffered, finalPosition, randomNum);

		//GameObject newOrb = Instantiate(orbPrefab, finalPosition, Quaternion.identity) as GameObject;
		//  GameObject orbParent = GameObject.Find("Orbs");
		//newOrb.transform.parent = orbParent.transform;
		StopCoroutine("CallEveryFewSeconds");

	}

	[PunRPC]
	void SpawnOrbFromHead(Vector3 finalPosition_, int randomNum_){
		GameObject newOrb = Instantiate(orbPrefab, finalPosition_, Quaternion.identity) as GameObject;
		newOrb.transform.name = randomNum_.ToString();
		GameObject orbParent = GameObject.Find("Orbs");
		newOrb.transform.parent = orbParent.transform;
	}

	private Vector3 pointInWorld;
	private Vector3 mousePosition;
	private float radius = 3.0f;
	private Vector3 direction;
	void MouseRotationSnake(){
		Ray ray = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		Physics.Raycast(ray, out hit, 1000.0f);

		mousePosition = new Vector3(hit.point.x, hit.point.y, 0);

		direction = Vector3.Slerp(direction, mousePosition-transform.position, Time.deltaTime * 1);
		direction.z = 0;

		pointInWorld = direction.normalized * radius  + transform.position;

		transform.LookAt(pointInWorld);
	}



	public float speed = 3.5f;
	void FixedUpdate(){
		if(photonView.isMine){
			MoveForward();
			CameraFollow();
			ApplyingStuffForBody();
		}
		velocity = Vector3.Magnitude(transform.position-lastFrame);
		lastFrame = transform.position;
	}

	void MoveForward(){
		transform.position += transform.forward * speed * Time.deltaTime;
	}


	[Range(0.0f,1.0f)]
	public float cameraFollowTime = 0.5f;
	void CameraFollow(){
		Transform camera = GameObject.FindGameObjectWithTag("MainCamera").gameObject.transform;
		Vector3 cameraVelocity = Vector3.zero;
		camera.position = Vector3.SmoothDamp(camera.position,
			new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, -10), ref cameraVelocity, cameraFollowTime);
	}

	private int orbCounter;
	private int currentOrb;
	public int[] growOnThisOrb;
	private Vector3 currentSize = Vector3.one;
	private float bodyPartOverTimeFollow = 0.19f;
	bool SizeUp(int x){
		try{
			if(x == growOnThisOrb[currentOrb]){
				currentOrb ++;
				return false;
			}
			else{
				return false;
			}
		}
		catch(System.Exception e){
			print ("No more grow from this point(add more rows). + " + e.StackTrace.ToString());
		}

		return false;
	}
	/*
    public Transform bodyObject;
    void OnCollisionEnter(Collision other){
        if(other.transform.tag == "Orb"){
            Destroy(other.gameObject);
        //  orbCounter++;
 
            if(SizeUp(orbCounter) == false){
                orbCounter++;
                if(bodyParts.Count == 0){
                    Vector3 currentPos = transform.position;
                    Transform newBodyPart = Instantiate (bodyObject, currentPos, Quaternion.identity) as Transform;
 
                    //newBodyPart.localScale = currentSize;
                    //newBodyPart.GetComponent<SnakeBody>().overTime = bodyPartOverTimeFollow;
 
                    bodyParts.Add(newBodyPart);
                }
                else{
                    Vector3 currentPos = bodyParts[bodyParts.Count-1].position;
                    Transform newBodyPart = Instantiate (bodyObject, currentPos, Quaternion.identity) as Transform;
 
                    //newBodyPart.localScale = currentSize;
                    //newBodyPart.GetComponent<SnakeBody>().overTime = bodyPartOverTimeFollow;
 
                    bodyParts.Add(newBodyPart);
                }
            }
        }
    }
*/
	private bool running;
	public float speedWhileRunnin = 6.5f;
	public float speedWhileWalking = 3.5f;
	public float bodyPartFollowTimeWalking = 0.19f;
	public float bodyPartFollowTimeRunning = 0.1f;
	void Running(){
		if(bodyParts.Count > 2){
			if(Input.GetMouseButtonDown(0)){
				speed = speedWhileRunnin;
				running = true;
				bodyPartOverTimeFollow = bodyPartFollowTimeRunning;
			}
			if(Input.GetMouseButtonUp(0)){
				speed = speedWhileWalking;
				running = false;
				bodyPartOverTimeFollow = bodyPartFollowTimeWalking;
			}
		}
		else{
			speed = speedWhileWalking;
			running = false;
			bodyPartOverTimeFollow = bodyPartFollowTimeWalking;
		}

		if(running == true){
			if(photonView.isMine)
				StartCoroutine("LoseBodyParts");
		}
		else{
			bodyPartOverTimeFollow = bodyPartFollowTimeWalking;
		}
	}

	IEnumerator LoseBodyParts(){
		yield return new WaitForSeconds(0.5f);

		if(photonView.isMine){
			int randomName = Random.Range(1,10000);
			photonView.RPC("LoseNetworkBodyPart", PhotonTargets.AllBuffered, randomName);
		}
		/*
        int lastIndex = bodyParts.Count -1;
        Transform lastBodyPart = bodyParts[lastIndex].transform;
 
        Instantiate(orbPrefab, lastBodyPart.position, Quaternion.identity);
 
        bodyParts.RemoveAt(lastIndex);
        Destroy(lastBodyPart.gameObject);
 
        orbCounter--;
*/
		StopCoroutine("LoseBodyParts");

	}
	[PunRPC]
	void LoseNetworkBodyPart(int _randomName){
		int lastIndex = bodyParts.Count -1;
		Transform lastBodyPart = bodyParts[lastIndex].transform;

		GameObject orbAthTheMoment = Instantiate(orbPrefab, lastBodyPart.position, Quaternion.identity) as GameObject;
		orbAthTheMoment.name = _randomName.ToString();

		bodyParts.RemoveAt(lastIndex);
		Destroy(lastBodyPart.gameObject);

		orbCounter--;
	}

	void MakeOurSnakeGlow(bool areWeRunning){

		foreach(Transform bodyParts_x in bodyParts){
			bodyParts_x.FindChild("glow").gameObject.SetActive(areWeRunning);
		}

	}

	private Vector3 headV;
	void ApplyingStuffForBody(){
		transform.localScale = Vector3.SmoothDamp(transform.localScale, currentSize, ref headV, 0.5f);
		foreach(Transform bodyPart_x in bodyParts){
			bodyPart_x.localScale = transform.localScale;
			bodyPart_x.GetComponent<SnakeBody>().overTime = bodyPartOverTimeFollow;
		}
	}

	public List<bool> scalingTrack;
	public int howBigAreWe_questionMark;
	public float followTimeSensitvity;
	public float scaleSenstivity;
	void Scaling(){
		scalingTrack = new List<bool>(new bool[growOnThisOrb.Length]);

		howBigAreWe_questionMark = 0;

		for(int i = 0; i < growOnThisOrb.Length; i++){
			if(orbCounter >= growOnThisOrb[i]){
				scalingTrack[i] = !scalingTrack[i];
				howBigAreWe_questionMark ++;
			}
		}

		currentSize = new Vector3(
			1 + (howBigAreWe_questionMark * scaleSenstivity),
			1 + (howBigAreWe_questionMark * scaleSenstivity),
			1 + (howBigAreWe_questionMark * scaleSenstivity)
		);
		bodyPartFollowTimeWalking = (howBigAreWe_questionMark/100.0f) + followTimeSensitvity;
		bodyPartFollowTimeRunning = bodyPartFollowTimeWalking / 2;
	}
}