using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SnakeMovement : MonoBehaviour {

	public List<Transform> bodyParts = new List<Transform>();
	void Update () {
		MouseRotationSnake ();
		ColorMySnake ();
		SpawnOrbManger ();
		Running ();
	}

	public Material orange, purple;
	void ColorMySnake(){
		for (int i = 0; i < bodyParts.Count; i++) {
			if (i % 2 == 0) {
				bodyParts [i].GetComponent<Renderer> ().material = purple;
			} else {
				bodyParts [i].GetComponent<Renderer> ().material = orange;
 			}
		}
	}

	public float spawnOrbEveryXSeonds = 1;
	public GameObject orbPrefab;
	void SpawnOrbManger(){
		StartCoroutine ("CallEveryFewSeonds", spawnOrbEveryXSeonds);
	}
	IEnumerator CallEveryFewSeonds(float x){
		yield return new WaitForSeconds (x); 
		StopCoroutine ("CallEveryFewSeonds");
		Vector3 randomOrbPos = new Vector3 (
			Random.Range(
				Random.Range(transform.position.x-10, transform.position.x-5),
				Random.Range(transform.position.x+5, transform.position.x+10)
			),
			Random.Range(
				Random.Range(transform.position.y-10, transform.position.y-5),
				Random.Range(transform.position.y+5, transform.position.y+10)
			),
			0
		);
		GameObject orb = Instantiate (orbPrefab, randomOrbPos, Quaternion.identity) as GameObject;
		GameObject orbParent = GameObject.Find ("Orbs");
		orb.transform.parent = orbParent.transform;
	}

	private bool running;
	public float speedwhileRunning = 6.5f;
	public float speedwhileWalking = 3.5f;
	public float speedFollowRunning = 0.19f;
	public float speedFollowWalking = 0.1f;
	void Running(){
		if (bodyParts.Count > 2) {
			if (Input.GetMouseButtonDown (0)) {
				speed = speedwhileRunning;
				bodyPartOverTimeFollow = speedFollowRunning;
				running = true;
			}
			if (Input.GetMouseButtonUp (0)) {
				speed = speedwhileWalking;
				bodyPartOverTimeFollow = speedFollowWalking;
				running = false;
			}
		} //else {
//			speed = speedwhileWalking;
//			bodyPartOverTimeFollow = speedFollowWalking;
//			running = false;
//		}
		if (running == true) {
			StartCoroutine ("LosingPart");
		} else {
			bodyPartOverTimeFollow = speedFollowWalking;
		}
	}

	IEnumerator LosingPart(){
		yield return new WaitForSeconds (0.5f);
		StopCoroutine ("LosingPart");

		int lastIndex = bodyParts.Count - 1;
		Transform lastTrans = bodyParts [lastIndex].transform;

		Instantiate (orbPrefab, lastTrans.position, Quaternion.identity);
		bodyParts.RemoveAt (lastIndex);
		Destroy (lastTrans.gameObject);

		orbCounter--;

	}
	void ApplyingForBody(){
		foreach (Transform body_x in bodyParts) {
			body_x.localScale = currentSize;
			body_x.GetComponent<SnakeBody> ().overTime = bodyPartOverTimeFollow;
		}
	}

	private Vector3 pointInWorld;
	private Vector3 mousePoint;
	private float radius = 3.0f;
	private Vector3 direction;
	void MouseRotationSnake(){
		Ray ray = GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<Camera> ().ScreenPointToRay (Input.mousePosition);
		RaycastHit hit;
		Physics.Raycast (ray, out hit, 1000.0f);

		mousePoint = new Vector3 (hit.point.x, hit.point.y, 0);
		direction = Vector3.Slerp (direction, mousePoint - transform.position, 1 * Time.deltaTime);
		direction.z = 0;
		pointInWorld = direction.normalized * radius + transform.position;
		pointInWorld.z = 0;
		transform.LookAt (pointInWorld);
	}

	void InputRotation(){
		if (Input.GetKey (KeyCode.A)) {
			currentRotation += rotationSensivitiy * Time.deltaTime;
		}
		if (Input.GetKey (KeyCode.D)) {
			currentRotation -= rotationSensivitiy * Time.deltaTime;
		}
	}

	public float speed = 3.5f;
	public float currentRotation;
	public float rotationSensivitiy = 50.0f;
	void FixedUpdate(){
		MoveForward ();
		Rotation ();
		CameraFollow ();
		ApplyingForBody ();
	}

	void MoveForward(){
		transform.position += transform.forward * speed * Time.deltaTime;
	}

	void Rotation(){
		transform.rotation = Quaternion.Euler (new Vector3 (transform.rotation.x, transform.rotation.y, currentRotation));
	}

	private int orbCounter;
	private int currentOrb;
	public int[] grownOnThisOrb;
	private Vector3 currentSize = Vector3.one;
	public float growRate = 0.1f;
	public float bodyPartOverTimeFollow = 0.19f;
	bool SizeUp(int x){
		try{
			if (x == grownOnThisOrb [currentOrb]) {
				currentOrb++;
				return true;
			} else {
				return false;
			}
		}
		catch(System.Exception e){
			print ("No more grwo up");
		}
		return false; 
	}


	[Range(0.0f, 1.0f)]
	public float smoothTime = 0.5f;
	void CameraFollow(){
		Transform camera = GameObject.FindGameObjectWithTag ("MainCamera").gameObject.transform;
		Vector3 cameraVelocity = Vector3.zero;

		camera.position = Vector3.SmoothDamp(camera.position,
			new Vector3(transform.position.x, transform.position.y, -20), ref cameraVelocity, smoothTime);
	}

	public Transform bodyObejct;
	void OnCollisionEnter(Collision other){
		if (other.gameObject.tag == "Orb") {
//			orbCounter++;
			Destroy (other.gameObject);
			if (SizeUp (orbCounter) == false) {
				orbCounter++;
				if (bodyParts.Count == 0) {
					Vector3 currentPos = transform.position;
					Transform newBodyPart = Instantiate (bodyObejct, currentPos, Quaternion.identity) as Transform;
//					newBodyPart.localScale = currentSize;
//					newBodyPart.GetComponent<SnakeBody> ().overTime = bodyPartOverTimeFollow;
					bodyParts.Add (newBodyPart);
				} else {
					Vector3 currentPos = bodyParts [bodyParts.Count - 1].position;
					Transform newBodyPart = Instantiate (bodyObejct, currentPos, Quaternion.identity) as Transform;
//					newBodyPart.localScale = currentSize;
//					newBodyPart.GetComponent<SnakeBody> ().overTime = bodyPartOverTimeFollow;
					bodyParts.Add (newBodyPart);
				}
			} else {
				currentSize += Vector3.one * growRate;
				bodyPartOverTimeFollow += 0.04f;
				transform.localScale = currentSize;


			}

		}
	}
}
