using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Dot_Truck : System.Object
{
	public WheelCollider leftWheel;
	public GameObject leftWheelMesh;
	public WheelCollider rightWheel;
	public GameObject rightWheelMesh;
	public bool motor;
	public bool steering;
	public bool reverseTurn; 
}

public class Dot_Truck_Controller : MonoBehaviour {

	public float maxMotorTorque;
	public float maxSteeringAngle;
	public List<Dot_Truck> truck_Infos;

	public Rigidbody rigidBody;
	UISystem uiSystem;

	public float steeringAngle;

	void Start ()
	{
		uiSystem = FindObjectOfType<UISystem> ();
	}

	public void VisualizeWheel(Dot_Truck wheelPair)
	{
		Quaternion rot;
		Vector3 pos;
		wheelPair.leftWheel.GetWorldPose ( out pos, out rot);
		wheelPair.leftWheelMesh.transform.position = pos;
		wheelPair.leftWheelMesh.transform.rotation = rot;
		wheelPair.rightWheel.GetWorldPose ( out pos, out rot);
		wheelPair.rightWheelMesh.transform.position = pos;
		wheelPair.rightWheelMesh.transform.rotation = rot;
	}

	public void Update()
	{
		if (GameManager.instance.replayState != ReplayState.PlayBack) {
			float motor = 0.0f;
			if (Input.GetKey (KeyCode.LeftShift) || Input.GetKey (KeyCode.RightShift)) {
				motor = 2 * maxMotorTorque * Input.GetAxis (GameManager.instance.controllerSelected + "Vertical");
			} else
				motor = maxMotorTorque * Input.GetAxis (GameManager.instance.controllerSelected + "Vertical");

//		Debug.Log("Value got : "+Input.GetAxis(GameManager.instance.controllerSelected+"Vertical"));


			float steering = maxSteeringAngle * Input.GetAxis (GameManager.instance.controllerSelected + "Horizontal");
			float brakeTorque = Mathf.Abs (Input.GetAxis (GameManager.instance.controllerSelected + "Break"));

			uiSystem.SetAngleValue (steering);
			steeringAngle = steering;


			if (brakeTorque > 0.001) {
				brakeTorque = maxMotorTorque;
				motor = 0;
			} else {
				brakeTorque = 0;
			}
//		Debug.Log ("current motor value : " + motor);
			foreach (Dot_Truck truck_Info in truck_Infos) {
				if (truck_Info.steering == true) {
					truck_Info.leftWheel.steerAngle = truck_Info.rightWheel.steerAngle = ((truck_Info.reverseTurn) ? -1 : 1) * steering;
				}

				if (truck_Info.motor == true) {
					truck_Info.leftWheel.motorTorque = motor;
					truck_Info.rightWheel.motorTorque = motor;
				}

				truck_Info.leftWheel.brakeTorque = brakeTorque;
				truck_Info.rightWheel.brakeTorque = brakeTorque;

				VisualizeWheel (truck_Info);
			}

		}
	}


}