using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
	public float damage = 10;
	public Vector3 speed;

	void Update()
	{
		transform.position += speed * Time.deltaTime;
	}

	void OnTriggerEnter(Collider other)
    {
		if (other.CompareTag("Player"))
		{
			Destroy(gameObject);
		}
    }
}
