using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FXnRXn
{
	public class FadeManager : MonoBehaviour
	{
		
		[Header("References")]
		public Image						fadeImage;
		
		
		[Header("Settings")]
		public float						fadeDuration		= 1f;
		
		
		
    
		private Vector3 startPosition;
		private bool isFading = false;
		
		
		void Start() 
		{
			if (fadeImage == null) return;
			
			fadeImage.gameObject.SetActive(false);
		}

		void OnTriggerEnter(Collider other) 
		{
			if (other.CompareTag("Player") && !isFading)
			{
				
				StartCoroutine(FadeSequence());
			}
		}

		IEnumerator FadeSequence() {
			isFading = true;
			
			yield return StartCoroutine(Fade(0, 1));
			SceneManager.LoadScene(SceneManager.GetActiveScene().name);
			yield return StartCoroutine(Fade(1, 0));
        
			isFading = false;
		}

		IEnumerator Fade(float startAlpha, float endAlpha)
		{
			fadeImage.gameObject.SetActive(true);
			float timer = 0;
        
			while (timer < fadeDuration) {
				timer += Time.deltaTime;
				float alpha = Mathf.Lerp(startAlpha, endAlpha, timer / fadeDuration);
				fadeImage.color = new Color(0, 0, 0, alpha);
				yield return null;
			}
        
			if (endAlpha == 0) fadeImage.gameObject.SetActive(false);
		}
    
	}
}

