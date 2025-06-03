using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CartoonFX
{
	public class CFXR_Demo_RandomText : MonoBehaviour
	{
		public ParticleSystem particles;
		public CFXR_ParticleText dynamicParticleText;

		void OnEnable()
		{
			InvokeRepeating("SetRandomText", 0f, 1.5f);
		}

		void OnDisable()
		{
			CancelInvoke("SetRandomText");
			particles.Clear(true);
		}

		void SetRandomText()
		{
			// set text and properties according to the random damage:
			// - bigger damage = big text, red to yellow gradient
			// - lower damage = smaller text, fully red
			int damage = Random.Range(10, 1000);
			string text = damage.ToString();
			float intensity = damage / 1000f;
			float size = Mathf.Lerp(0.8f, 1.3f, intensity);
			Color color1 = Color.Lerp(Color.red, Color.yellow, intensity);
			dynamicParticleText.UpdateText(text, size, color1);

			particles.Play(true);
		}
	}
}