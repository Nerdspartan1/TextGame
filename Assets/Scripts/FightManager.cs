using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightManager : MonoBehaviour
{
	//Singleton instance
	private static FightManager instance = null;

	public static float fightSceneWidth = 600;
	public static float fightSceneHeight = 600;

	public Transform fightPanel;

	public GameObject playerObject;
	private PlayerBar playerBar;

	void Start()
	{
		playerBar = playerObject.GetComponent<PlayerBar>();
		//Singleton
		if (instance == null)
		{
			instance = this;
		}
		else
		{
			Destroy(this);
		}
	}

	void BeginFight(Unit foe)
	{
		fightPanel.gameObject.SetActive(true);
	}

    void Update()
    {
        
    }
}
