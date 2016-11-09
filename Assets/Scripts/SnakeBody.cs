using UnityEngine;
using System.Collections;

public class SnakeBody : MonoBehaviour {

	private int myOrder;
	public Transform head;

	void Start(){
		//head = GameObject.FindGameObjectWithTag("Player").gameObject.transform;
		for(int i = 0; i < head.GetComponent<SnakeMovement>().bodyParts.Count; i++){
			if(gameObject == head.GetComponent<SnakeMovement>().bodyParts[i].gameObject){
				myOrder = i;
			}
		}
	}
	private Vector3 movementVelocity;
	[Range(0.0f,1.0f)]
	public float overTime = 0.5f;
	void FixedUpdate(){
		if(myOrder == 0){
			transform.position = Vector3.SmoothDamp(transform.position, head.position, ref movementVelocity, overTime);
			transform.LookAt(head.transform.position);
		}
		else{
			transform.position = Vector3.SmoothDamp(transform.position, head.GetComponent<SnakeMovement>().bodyParts[myOrder-1].position, ref movementVelocity, overTime);
			transform.LookAt(head.transform.position);
		}
	}
}