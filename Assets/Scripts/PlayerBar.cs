using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBar : MonoBehaviour
{
	public float speed;
	private RectTransform rectTransform;

	private void Start()
	{
		rectTransform = GetComponent<RectTransform>();
	}
	// Update is called once per frame
	void Update()
    {
		float X = transform.localPosition.x;
		X += speed * Input.GetAxisRaw("Horizontal")*Time.deltaTime;
		X = Mathf.Clamp(X,-FightManager.fightSceneWidth/2, FightManager.fightSceneWidth / 2);
		transform.localPosition = new Vector3(X, -FightManager.fightSceneHeight/2, 0);
    }
}
