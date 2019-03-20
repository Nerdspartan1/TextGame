using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitTarget : MonoBehaviour
{
	public Unit unit;

	private float speed;

    // Start is called before the first frame update
    void Start()
    {
		speed = 10*(10 + unit.DEX);
    }

    // Update is called once per frame
    void Update()
    {
		if (unit == GameManager.Instance.player)
		{

			Vector3 pos = transform.localPosition;
			pos.x += speed * Input.GetAxisRaw("Horizontal") * Time.deltaTime;
			pos.y += speed * Input.GetAxisRaw("Vertical") * Time.deltaTime;
			pos.x = Mathf.Clamp(pos.x, -FightManager.fightSceneWidth / 2, FightManager.fightSceneWidth / 2);
			pos.y = Mathf.Clamp(pos.y, -FightManager.fightSceneHeight / 2, FightManager.fightSceneHeight / 2);
			pos.z = 0;
			transform.localPosition = pos;
		}
    }
}
