using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatBar : MonoBehaviour
{
	private RectTransform rectTransform;

	public float Value;
	public float MaxValue = 1;

	public Text ValueText;

	public void UpdateBar()
	{
		if(!rectTransform) rectTransform = GetComponent<RectTransform>();
		float ratio = MaxValue == 0 ? 0 : Value / MaxValue;
		rectTransform.localScale = new Vector3(ratio > 0 ? ratio : 0, 1, 1);
		if (ValueText) ValueText.text = $"{Value} / {MaxValue}";
	}
}
