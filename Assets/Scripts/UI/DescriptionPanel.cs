using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DescriptionPanel : MonoBehaviour
{

	public Text Name;
	public Text Description;
	public Text Value;

	public RectTransform canvasRect;
	new private Camera camera;

	private void Awake()
	{
		canvasRect = GameManager.Instance.Canvas.GetComponent<RectTransform>();
		camera = Camera.main;
	}

	public void Show(Item item)
	{
		gameObject.SetActive(true);
		Name.text = item.Name;
		Description.text = item.Description;
		Value.text = item.Value.ToString();

		Update();
	}


	public void Update()
	{
		Vector3 position;
		RectTransformUtility.ScreenPointToWorldPointInRectangle(canvasRect, Input.mousePosition, camera, out position);
		transform.position = position;
	}

	public void Hide()
	{
		gameObject.SetActive(false);
	}

}
