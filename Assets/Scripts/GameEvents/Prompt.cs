﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prompt
{

	public Prompt Next;
	public System.Action<Prompt> Method;

	private bool waitForInput = false;

	public Prompt(System.Action<Prompt> method)
	{
		Method = method;
	}

	IEnumerator WaitForInput()
	{
		while (waitForInput) yield return null;
	}

	public void Proceed()
	{
		waitForInput = false;
	}

	public IEnumerator Display()
	{
		waitForInput = true;

		//do thing
		Method.Invoke(this);

		yield return WaitForInput();

		if (Next == null) //end of prompt chain
			yield break;
		else
			yield return Next.Display();
	}

	public static void PressOKToContinue(Prompt prompt)
	{
		GameManager.Instance.CreateButton("OK",
			delegate {
				prompt.Proceed();
			});
	}
}
