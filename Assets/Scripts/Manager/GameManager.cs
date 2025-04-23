using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace FXnRXn
{
	public class GameManager : MonoBehaviour
	{
    
		void Start()
		{
			Application.targetFrameRate = 120;
			QualitySettings.vSyncCount = 0;
		}
    
	}
}

