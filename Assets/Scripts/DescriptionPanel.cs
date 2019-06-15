using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DescriptionPanel : MonoBehaviour
{

	public Text Name;
	public Text Description;
	public Text Value;

	public void Show(Item item)
	{
		gameObject.SetActive(true);
		Name.text = item.Name;
		Description.text = item.Description;
		Value.text = item.Value.ToString();
	}


	public void Update()
	{
		transform.position = Input.mousePosition;
	}

	public void Hide()
	{
		gameObject.SetActive(false);
	}

}
