using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace FXnRXn
{
	[RequireComponent(typeof(CharacterController))]
	
	public class PlayerController : MonoBehaviour
	{
		#region Variables

		public static PlayerController instance;
		public PoleState playerState { get; set; }
		public bool canMove { get; private set; }
		
		
		[Header("Refference : ")]
		[SerializeField] private Joystick					input;
		[SerializeField] private SwipePanel					swipePanel;

		[Header("Setting : ")] 
		[SerializeField] private float						moveSpeed					= 10f;
		[SerializeField] private float						rotationSpeed				= 5f;
		[SerializeField] private float						gravity						= 9.81f;
		[SerializeField] private float						jumpHeight					= 2f;
		[SerializeField] private float						jumpMultiplier				= 2.5f;
		[SerializeField] private float						colorChangeInterval			= 2f;
		
		[Space(10)]
		[SerializeField] private float						attractRadius				= 10f;
		[SerializeField] private float						attractForce				= 15f;
		[SerializeField] private float						repelForce					= 2f;

		private CharacterController							characterController;
		private Vector3										movementDirection;
		private float										verticalVelocity;
		private Vector2										fingerDownPos;
		private Renderer									rend;
		private Color										currentColor;
		
		private bool										isStuck = false;
		private GameObject									targetNPC;

		public bool IsSwapping { get; private set; } = false;
		#endregion
		
		
		//--------------------------------------------------------------------------------------------------------------
		private void Awake()
		{
			if (input == null)
			{
				input = FindFirstObjectByType<Joystick>();
			}

			if (swipePanel == null)
			{
				swipePanel = FindFirstObjectByType<SwipePanel>();
			}

			if (rend == null)
			{
				rend = GetComponentInChildren<Renderer>();
			}

			if (characterController == null)
			{
				characterController = GetComponent<CharacterController>();
			}
		}

		private void Start()
		{
			if (instance == null)
			{
				instance = this;
			}

			canMove = true;
			currentColor = Color.red;
			rend.material.color = currentColor;
			playerState = PoleState.RedPole;
			StartCoroutine(ColorChanger());
			
			
		}
		
		private void Update()
		{
			HandleMovement();
		}

		private void HandleMovement()
		{
			Rotate();

			Collider[] colliders = Physics.OverlapSphere(transform.position, attractRadius);
			foreach (Collider col in colliders)
			{
				var npc = col.GetComponent<NPCBehaviour>();
				if (npc != null && canMove)
				{
					targetNPC = npc.gameObject;
					if (targetNPC.GetComponent<NPCBehaviour>().npcState != GetPlayerPoleState())
					{ // Attract
						
						Vector3 direction = (col.transform.position - transform.position).normalized;
						AttractForce(direction);
					}
					else
					{
						if (isStuck)
						{
							RepelForce(targetNPC);
						}
					}
					
				}
			}
			

			if (canMove)
			{
				Vector3 moveVector = GetMovementInput() * moveSpeed * Time.deltaTime;
				if (targetNPC != null && targetNPC.GetComponent<NPCBehaviour>().npcState == GetPlayerPoleState() && !isStuck)
				{
					moveVector = RestrictMovement(moveVector, targetNPC.transform, targetNPC.GetComponent<NPCBehaviour>().GetRestrictedRadius());
				}
				
				moveVector.y = VerticalVelocityCalculation();
				characterController.Move(moveVector);
			}

		}

		private Vector3 RestrictMovement(Vector3 intendedMovement , Transform npcTransform , float restrictedRadius)
		{
			Vector3 currentPosition = transform.position;
			Vector3 futurePosition = currentPosition + intendedMovement;
			
			Vector3 centerToFuture = futurePosition - npcTransform.position;
			float distanceToCenter = centerToFuture.magnitude;

			if (distanceToCenter < restrictedRadius)
			{
				Vector3 allowedPosition = npcTransform.position + centerToFuture.normalized * restrictedRadius;
				return allowedPosition - currentPosition;
			}
			
			return intendedMovement;
		}

		protected Vector3 GetMovementInput()
		{
			return new Vector3(input.Horizontal , 0f, input.Vertical);
		}
		

		protected void Rotate()
		{
			Vector3 currentLookDirection = characterController.velocity.normalized;
			Vector3 direction = new Vector3(-currentLookDirection.z, 0f, currentLookDirection.x);
			direction.Normalize();
			Quaternion targetRot = Quaternion.LookRotation(direction);
			transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);

		}


		private float VerticalVelocityCalculation()
		{
			if (characterController.isGrounded)
			{
				verticalVelocity = 0f;
				if (swipePanel.WasSwipeDetected())
				{
					verticalVelocity = Mathf.Sqrt(jumpHeight * gravity * jumpMultiplier);
				}
			}
			else
			{
				verticalVelocity -= gravity * Time.deltaTime;
			}

			return verticalVelocity;
		}

		
		IEnumerator ColorChanger()
		{
			while (true)
			{
				yield return new WaitForSeconds(colorChangeInterval);
				currentColor = currentColor == Color.red ? Color.blue : Color.red;
				playerState = currentColor == Color.red ? PoleState.RedPole : PoleState.BluePole;
				rend.material.color = currentColor;
			}
		}

		public void AttractForce(Vector3 _direction)
		{
			characterController.Move(_direction * attractForce * Time.deltaTime);
		}

		public void RepelForce(GameObject _targetNPC)
		{
			Vector3 repulseDirection = (_targetNPC.transform.position - transform.position).normalized;
			transform.DOMove(repulseDirection, repelForce).OnComplete(() =>
			{
				isStuck = false;
			});
			
		}

		private void OnControllerColliderHit(ControllerColliderHit hit)
		{
			if (targetNPC != null && hit.gameObject == targetNPC && !isStuck )
			{
				isStuck = true;
				// canMove = false;
				
				
			}
		}

		private void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.yellow;
			Gizmos.DrawWireSphere(transform.position, attractRadius);
		}

		public PoleState GetPlayerPoleState() => playerState;

	}
}

