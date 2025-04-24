using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FXnRXn
{
	[RequireComponent(typeof(CharacterController))]
	public class PlayerController : MonoBehaviour
	{
		#region Variables

		[Header("Refference : ")] 
		[SerializeField] private Joystick					input;


		[Header("Setting : ")] 
		[SerializeField] private float						moveSpeed = 10f;
		[SerializeField] private float						rotationSpeed = 5f;

		private CharacterController							characterController;
		private Vector3										movementDirection;


		#endregion
		
		
		//--------------------------------------------------------------------------------------------------------------
		private void Awake()
		{
			if (input == null)
			{
				input = FindFirstObjectByType<Joystick>();
			}

			if (characterController == null)
			{
				characterController = GetComponent<CharacterController>();
			}
		}


		private void Update()
		{
			HandleMovement();
		}



		private void HandleMovement()
		{
			
			if (GetMovementInput().magnitude > 0.1f)
			{
				Rotate();
			}
			
			Vector3 moveVector = GetMovementInput() * moveSpeed * Time.deltaTime;
			characterController.Move(moveVector);
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
		
	}
}

