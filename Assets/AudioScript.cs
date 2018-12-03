using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioScript : MonoBehaviour {
    Dictionary<SFX, AudioSource> sources;
    public AudioSource source_cardPick, source_cardReturn, source_click, source_get, source_sacrifice, source_set;

	// Use this for initialization
	void Start () {
        sources = new Dictionary<SFX, AudioSource>();
        sources.Add(SFX.CardPick, source_cardPick);
        sources.Add(SFX.CardReturn, source_cardReturn);
        sources.Add(SFX.Click, source_click);
        sources.Add(SFX.Get, source_get);
        sources.Add(SFX.Sacrifice, source_sacrifice);
        sources.Add(SFX.Set, source_set);
    }
    public void Play(SFX sfx) {
        //if (sfx == SFX.Get) {
        //    source_get.pitch = Random.Range(1.45f, 1.55f);
        //}
        sources[sfx].PlayOneShot(sources[sfx].clip);
    }

    // Update is called once per frame
    void Update () {
		
	}
}

public enum SFX {
    CardPick, CardReturn, Click, Get, Sacrifice, Set
}