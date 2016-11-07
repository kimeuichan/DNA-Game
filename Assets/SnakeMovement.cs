using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SnakeMovement : MonoBehaviour {

	public List<Transform> bodyParts = new List<Transform>();
	void Update () {
		MouseRotationSnake ();
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
	}

	void MoveForward(){
		transform.position += transform.forward * speed * Time.deltaTime;
	}

	void Rotation(){
		transform.rotation = Quaternion.Euler (new Vector3 (transform.rotation.x, transform.rotation.y, currentRotation));
	}

	[Range(0.0f, 1.0f)]
	public float smoothTime = 0.5f;
	void CameraFollow(){
		Transform camera = GameObject.FindGameObjectWithTag ("MainCamera").gameObject.transform;
		Vector3 cameraVelocity = Vector3.zero;

		camera.position = Vector3.SmoothDamp(camera.position,
			new Vector3(transform.position.x, transform.position.y, -10), ref cameraVelocity, smoothTime);
	}

	public Transform bodyObejct;
	void OnCollisionEnter(Collision other){
		if (other.gameObject.tag == "Orb") {
			Destroy (other.gameObject);
			if (bodyParts.Count == 0) {
				Vector3 currentPos = transform.position;
				Transform newBodyPart = Instantiate (bodyObejct, currentPos, Quaternion.identity) as Transform;
				bodyParts.Add (newBodyPart);
			} else {
				Vector3 currentPos = bodyParts[bodyParts.Count-1].position;
				Transform newBodyPart = Instantiate (bodyObejct, currentPos, Quaternion.identity) as Transform;
				bodyParts.Add (newBodyPart);
			}
		}
	}
}
