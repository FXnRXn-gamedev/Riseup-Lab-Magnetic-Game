using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
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
		
		[Header("Path Move : ")]
		[SerializeField] private Transform					pathStartPoint;
		[SerializeField] private Transform					pathEndPoint;
		[SerializeField] private float						pathDuration				= 2f;
		
		
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


			switch (npcBehaveState)
			{
				case NPCBehave.Static:
					break;
				case NPCBehave.Dynamic:
					
					if (pathStartPoint == null && pathEndPoint == null) return;
					PathMove();
					
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
			
			
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

		private void PathMove()
		{
			Vector3[] path = new Vector3[]
			{
				pathStartPoint.position,
				pathEndPoint.position
			};

			transform.DOLocalPath(path, pathDuration, PathType.Linear).SetEase(Ease.Linear)
				.SetLoops(-1, LoopType.Yoyo);
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

