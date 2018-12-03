using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AltarScript : MonoBehaviour {
    public GameObject glow;
    public Material glowMaterial;
    public Light glowLight;
    AltarState state;
    float initialScale, initialAlpha, initialIntensity;
    float bright;
    float targetScale, targetBright;

	void Start () {
        state = AltarState.Off;
        initialScale = glow.transform.localScale.y;
        // HACK: I'd like to use the material's albedo from the editor, but runtime changes to materials are SAVED to the actual asset.
        initialAlpha = .3f;// glowMaterial.color.a;
        initialIntensity = glowLight.intensity;
        bright = 1;
        NewTargetScale();
        NewTargetBright();
	}
    public void Await() {
        state = AltarState.Waiting;
        NewTargetScale();
        NewTargetBright();
    }
    public void Chosen() {
        state = AltarState.Chosen;
        NewTargetScale();
        NewTargetBright();
    }
    public void Activate() {
        state = AltarState.Activating;
        NewTargetScale();
        NewTargetBright();
    }
    public void Off() {
        state = AltarState.Off;
        NewTargetScale();
        NewTargetBright();
    }
    public AltarState GetState() {
        return state;
    }

    // Update is called once per frame
    void Update () {
        float newScale = Mathf.Lerp(glow.transform.localScale.y, targetScale, .15f);
        glow.transform.localScale = new Vector3(glow.transform.localScale.x, newScale, glow.transform.localScale.z);
        if (Mathf.Abs(newScale - targetScale) < .02f) {
            NewTargetScale();
        }
        bright = Mathf.Lerp(bright, targetBright, .15f);
        float a = initialAlpha * bright;
        glowMaterial.color = new Color(glowMaterial.color.r, glowMaterial.color.g, glowMaterial.color.b, a);
        glowLight.intensity = initialIntensity * bright;
        if (Mathf.Abs(bright - targetBright) < .01f) {
            NewTargetBright();
        }
    }
    void NewTargetScale() {
        if (state == AltarState.Off) {
            targetScale = 0;
        } else if (state == AltarState.Waiting) {
            targetScale = initialScale * Random.Range(.9f, 1.1f);
        } else if (state == AltarState.Chosen) {
            targetScale = initialScale * Random.Range(1.66f, 2.5f);
        } else if (state == AltarState.Activating) {
            targetScale = 8;
        }
    }
    void NewTargetBright() {
        if (state == AltarState.Off) {
            targetBright = 0;
        }
        else if (state == AltarState.Waiting) {
            targetBright = Random.Range(.9f, 1.1f);
        }
        else if (state == AltarState.Chosen) {
            targetBright = Random.Range(1.5f, 1.75f);
        }
        else if (state == AltarState.Activating) {
            targetBright = 2;
        }
    }

    public enum AltarState {
        Off, Waiting, Chosen, Activating
    }
}
