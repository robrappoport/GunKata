using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TimeManipulation : object {
	/// <summary>
	/// Slows time temporarily.
	/// </summary>
	/// <param name="slowedTimeScale">Time scale while slowed.</param>
	/// <param name="slowingDuration">How long it should take to slow down.</param>
	/// <param name="slowedDuration">How long it should stay slowed.</param>
	/// <param name="returnDuration">How long it should take to return back to normal time.</param>
	public static IEnumerator SlowTimeTemporarily(float slowedTimeScale = 0.5f, float slowingDuration = 1, float slowedDuration = 1, float returnDuration = 1){

		float startingTimeScale = Time.timeScale;
        float startingFixedTimeScale = Time.fixedDeltaTime;
		float elapsedTime = 0;
		while (elapsedTime < slowingDuration) {

			elapsedTime += Time.deltaTime;
			Time.timeScale = Mathf.Lerp (1, slowedTimeScale, elapsedTime / slowingDuration);
            Time.fixedDeltaTime = Time.timeScale * startingFixedTimeScale;
			yield return null;
		}
		yield return new WaitForSeconds (slowedDuration);
		elapsedTime = 0;
		while (elapsedTime < returnDuration) {
			elapsedTime += Time.deltaTime; 
			Time.timeScale = Mathf.Lerp (slowedTimeScale, 1, elapsedTime / returnDuration);
            Time.fixedDeltaTime = Time.timeScale * startingFixedTimeScale;
			yield return null;
		}
	}

}
