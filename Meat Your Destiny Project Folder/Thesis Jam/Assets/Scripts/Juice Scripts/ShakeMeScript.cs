using System.Collections;
using UnityEngine;

public static class ShakeMeScript : object {


	/// <summary>
	/// Shakes a UI element
	/// </summary>
	/// <param name="r">The RectTransform of the UI element.</param>
	/// <param name="shakeMagnitude">Shake magnitude.</param>
	/// <param name="shakeTime">Shake duration.</param>
	/// <param name="horizontal">If set to <c>true</c>, shake horizontally.</param>
	/// <param name="vertical">If set to <c>true</c>, shake vertically.</param>
	public static IEnumerator ShakeUI(RectTransform r, float shakeMagnitude = 1.5f, float shakeTime = 0.5f, bool horizontal = true, bool vertical = false){
		Vector2 originalPos = r.anchoredPosition, rand;
        float elapsedTime = 0;
		float x = 0;
		float y = 0;
        while(elapsedTime < shakeTime){
            elapsedTime += Time.deltaTime;
			rand = Random.insideUnitCircle * shakeMagnitude;
			if (horizontal) {
				x = rand.x;
			} 
			if (vertical) {
				y = rand.y;
			}
			r.anchoredPosition = originalPos + new Vector2(x, y);
            yield return null;

        }
        elapsedTime = 0;
        while(elapsedTime < .1f){
            elapsedTime += Time.deltaTime;
            r.anchoredPosition = Vector2.Lerp(r.anchoredPosition, originalPos, elapsedTime / .1f);
            yield return null;
        }

    }
	/// <summary>
	/// Shakes an object in 3D space
	/// </summary>
	/// <param name="t">The transform of the shaking object.</param>
	/// <param name="shakeMagnitude">Shake magnitude.</param>
	/// <param name="shakeTime">The duration of the shake.</param>
	/// <param name="x">If set to <c>true</c>, shake in the x direction.</param>
	/// <param name="y">If set to <c>true</c>, shake in the y direction.</param>
	/// <param name="z">If set to <c>true</c>, shake in the z direction.</param>
	public static IEnumerator Shake(Transform t, float shakeMagnitude = 2f, float shakeTime = 0.5f, bool shakeX = true, bool shakeY = false, bool shakeZ = true){
		Vector3 originalPos = t.position;
		float elapsedTime = 0, x = 0, y = 0, z = 0;
		Vector3 rand;
		while (elapsedTime < shakeTime) {
			elapsedTime += Time.deltaTime;
			rand = Random.insideUnitSphere * shakeMagnitude;
			if (shakeX) {
				x = rand.x;
			}
			if (shakeY) {
				y = rand.y;
			}

			if (shakeZ) {
				z = rand.z;
			}
			t.position = originalPos + new Vector3 (x, y, z);
			yield return null;

		}
		elapsedTime = 0;
		while (elapsedTime < .1f) {
			elapsedTime += Time.deltaTime;
			t.position = Vector2.Lerp (t.position, originalPos, elapsedTime / .1f);
			yield return null;
		}
	}

}
