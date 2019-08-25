using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatBar : MonoBehaviour
{
	public RectTransform BarTransform;
	public Text ValueText;

	public float Value;
	public float MaxValue = 1;

	public bool ShowMaxValue = true;

	public void UpdateBar()
	{

		float ratio = MaxValue == 0 ? 0 : Value / MaxValue;
		BarTransform.localScale = new Vector3(ratio > 0 ? ratio : 0, 1, 1);
		if (ValueText)
		{
			if (ShowMaxValue) ValueText.text = $"{Value} / {MaxValue}";
			else ValueText.text = $"{Value}";
		}
	}
}
