using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AttributeBar : StatBar
{
	public Attribute Attribute;

	[Header("References")]
	public RectTransform UpperBarTransform;
	public Button LevelUpButton;

	public float OneBarValue;
	public Gradient Gradient;

	private Image upperBarImage;
	private Image barImage;

	public void Start()
	{
		upperBarImage = UpperBarTransform.GetComponent<Image>();
		barImage = BarTransform.GetComponent<Image>();
	}

	public override void UpdateBar()
	{
		float bottomRatio;
		float upperRatio;
		if (Value < OneBarValue) {
			bottomRatio = Value / OneBarValue;
			upperRatio = 0;
			barImage.color = Gradient.Evaluate(0);
		}
		else
		{
			barImage.color = Gradient.Evaluate(((int)(Value / OneBarValue)-1)*OneBarValue / MaxValue);
			upperBarImage.color = Gradient.Evaluate((int)(Value / OneBarValue)*OneBarValue / MaxValue);
			bottomRatio = 1;
			upperRatio = (Value % OneBarValue) / OneBarValue;
		}

		BarTransform.localScale = new Vector3(bottomRatio > 0 ? bottomRatio : 0, 1, 1);
		UpperBarTransform.localScale = new Vector3(upperRatio > 0 ? upperRatio : 0, 1, 1);

		if (ValueText)
		{
			ValueText.text = $"{Value}";
		}
	}
}
