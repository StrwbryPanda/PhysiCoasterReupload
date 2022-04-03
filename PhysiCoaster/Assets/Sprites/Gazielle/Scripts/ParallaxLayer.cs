using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode()]
public class ParallaxLayer : MonoBehaviour {
	[Range(-10f, 1f)]
	public float ParallaxSpeed = 1f;
	public GameObject BaseObject;

	private Transform _transform;
	private Vector3 _offset;
	private Vector3 _newPosition;
	private Vector3 _roundedPosition;

	private void Awake() {
		_transform = GetComponent<Transform> ();
		_transform.position = new Vector3(_transform.position.x, 3.0f, 1.0f);
		_newPosition = _transform.position;
		_offset = BaseObject.transform.position - _newPosition;
	}

	private void Update() {
		_newPosition.x = _offset.x + BaseObject.transform.position.x * ParallaxSpeed;
		_roundedPosition = _newPosition;
		_roundedPosition.x = _newPosition.x;
		//_roundedPosition.x = Mathf.Floor (_newPosition.x * 32f) / 32f;
		_transform.position = _roundedPosition;
	}
}
