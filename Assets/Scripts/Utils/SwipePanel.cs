using System;
using System.Runtime.CompilerServices;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine;

namespace FXnRXn
{
	[RequireComponent(typeof(RectTransform))]
	public class SwipePanel : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
	{

		[Header("Swipe Settings")]
		[SerializeField] private float swipeDistanceThreshold = 100f;
		[SerializeField] private float maxSwipeTime = 0.5f;

		private Vector2 startTouchPosition;
		private Vector2 endTouchPosition;
		private float startTime;
		private bool swipeDetected = false;


		public bool WasSwipeDetected()
		{
			if (swipeDetected)
			{
				swipeDetected = false;
				return true;
			}
			
			return false;
		}
		

		public void OnPointerDown(PointerEventData eventData)
		{
			startTouchPosition = eventData.position;
			startTime = Time.time;
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			endTouchPosition = eventData.position;
			float swipeTime = Time.time - startTime;
        
			DetectSwipe(swipeTime);
		}
		
		private void DetectSwipe(float duration)
		{
			if (duration > maxSwipeTime) return;

			float swipeDistance = Vector2.Distance(startTouchPosition, endTouchPosition);
        
			if (swipeDistance >= swipeDistanceThreshold)
			{
				swipeDetected = true;
			}
		}
		
	}

}
