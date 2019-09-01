using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

[RequireComponent(typeof(ContentSizeFitter),typeof(LayoutGroup))]
public abstract class OptionPanel : MonoBehaviour
{
	public GameObject ButtonPrefab;

	public abstract void UpdateUI();

	protected Button AddButton(string name, UnityAction onClick)
	{
		Button b = Instantiate(ButtonPrefab, transform).GetComponent<Button>();
		b.GetComponentInChildren<Text>().text = name;
		b.onClick.AddListener(onClick);
		return b;
	}

	protected void ClearButtons()
	{
		foreach (Transform child in transform)
		{
			Destroy(child.gameObject);
		}
	}
}
