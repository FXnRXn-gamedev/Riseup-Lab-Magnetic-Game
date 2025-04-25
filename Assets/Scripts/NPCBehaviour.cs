using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FXnRXn
{
	[RequireComponent(typeof(Rigidbody))]
	public class NPCBehaviour : MonoBehaviour
	{
		public static NPCBehaviour instance;

		public PoleState npcState { get; private set; }
		

		[Header("Setting : ")] 
		[SerializeField] private PoleState					startColor;
		[SerializeField] private NPCBehave					npcBehaveState;
		[SerializeField] private float						colorChangeInterval			= 5f;
		[SerializeField] private float						restricAreaRadius			= 10f;
		
		
		
		
		private Renderer									npcRend;
		private Color										npcCurrentColor;
		private Rigidbody									npcRb;
		
		
		
		
		
		//--------------------------------------------------------------------------------------------------------------


		private void Awake()
		{
			if (npcRend == null)
			{
				npcRend = GetComponentInChildren<Renderer>();
			}

			if (npcRb == null)
			{
				npcRb = GetComponent<Rigidbody>();
			}
		}

		private void Start()
		{
			if (instance == null)
			{
				instance = this;
			}
			
			npcCurrentColor = startColor == PoleState.RedPole ? Color.red : Color.blue;
			npcRend.material.color = npcCurrentColor;
			npcState = startColor == PoleState.RedPole? PoleState.RedPole : PoleState.BluePole;
			StartCoroutine(NPCColorChanger());
		}

		
		

		IEnumerator NPCColorChanger()
		{
			while (true)
			{
				yield return new WaitForSeconds(colorChangeInterval);
				npcCurrentColor = npcCurrentColor == Color.red ? Color.blue : Color.red;
				npcState = npcCurrentColor == Color.red ? PoleState.RedPole : PoleState.BluePole;
				npcRend.material.color = npcCurrentColor;
			}
		}

		public NPCBehave GetNPCBehave() => npcBehaveState;
		public float GetRestrictedRadius() => restricAreaRadius;
	}

	[System.Serializable]
	public enum NPCBehave
	{
		Static,
		Dynamic
	}
}

