using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveSlot : MonoBehaviour
{
	public Text Name;
	public Button SaveButton;
	public Button LoadButton;
	public Button DeleteButton;
	private int slotId;

	public void SetSlotId(int slotId)
	{
		this.slotId = slotId;
		Name.text = $"Save {slotId}";
		SaveButton.onClick.RemoveAllListeners();
		SaveButton.onClick.AddListener(delegate { SaveManager.Instance.Save(slotId); UpdateUI(); });
		LoadButton.onClick.RemoveAllListeners();
		LoadButton.onClick.AddListener(delegate { SaveManager.Instance.Load(slotId); UpdateUI(); });
		DeleteButton.onClick.RemoveAllListeners();
		DeleteButton.onClick.AddListener(delegate { SaveManager.Instance.Delete(slotId); UpdateUI(); });
		UpdateUI();
	}

	public void UpdateUI()
	{
		if (System.IO.File.Exists(SaveManager.GetSavePath(slotId)))
		{
			LoadButton.interactable = true;
			DeleteButton.interactable = true;
		}
		else
		{
			LoadButton.interactable = false;
			DeleteButton.interactable = false;
		}
	}
}
