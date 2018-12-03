using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CandleScript : MonoBehaviour {
    Light lightComponent;
    Vector3 initialPosition, positionTarget;
    float initialIntensity, intensityTarget;
	void Start () {
        initialPosition = transform.position;
        SetNewPositionTarget();
        lightComponent = GetComponent<Light>();
        initialIntensity = lightComponent.intensity;
        SetNewIntensityTarget();
	}
	
	void Update () {
        transform.position = Vector3.Lerp(transform.position, positionTarget, .1f);
        if ((transform.position - positionTarget).sqrMagnitude < .01f) {
            SetNewPositionTarget();
        }
        lightComponent.intensity = Mathf.Lerp(lightComponent.intensity, intensityTarget, .3f);
        if (Mathf.Abs(lightComponent.intensity - intensityTarget) < .01f) {
            SetNewIntensityTarget();
        }
	}

    void SetNewPositionTarget() {
        positionTarget = initialPosition;
        positionTarget.x += Random.Range(-.2f, .2f);
        positionTarget.y += Random.Range(-.2f, .2f);
    }
    void SetNewIntensityTarget() {
        intensityTarget = initialIntensity * Random.Range(.975f, 1.025f);
    }
}
