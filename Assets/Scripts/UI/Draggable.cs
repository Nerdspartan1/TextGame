using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Draggable : MonoBehaviour
{
	private bool dragging = false;
	public RectTransform canvasRect;
	public bool CenterOnMouse = false;

	private Vector3 dragPointLocalPosition;

	new private Camera camera;
	
	private void Awake()
	{
		camera = Camera.main;
	}

	public void ToggleDrag(bool drag)
	{
		dragging = drag;
		if (drag)
		{
			Vector3 position;

			RectTransformUtility.ScreenPointToWorldPointInRectangle(canvasRect, Input.mousePosition, camera, out position);

			dragPointLocalPosition = CenterOnMouse ? Vector3.zero : position - transform.position;
		}
	}
	private void Update()
	{
		if (dragging)
		{
			Vector3 position;

			RectTransformUtility.ScreenPointToWorldPointInRectangle(canvasRect, Input.mousePosition, camera, out position);

			transform.position = position - dragPointLocalPosition;
		}
	}
}
