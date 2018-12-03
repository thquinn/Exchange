using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CardControllerScript : MonoBehaviour {
    public StoryControllerScript storyScript;
    public AltarScript altarScript;

    public GameObject cardPrefab;

    List<GameObject> hand;
    GameObject held;
    GameObject created;
    GameObject chosen;
    public GameObject altar;
    float heldDX = 0, heldDY = 0;

    public void CreateCard(string card) {
        List<string> cardNames = new List<string>(storyScript.items.Keys);
        cardNames.Sort();
        int number = cardNames.IndexOf(card) + 1;

        created = GameObject.Instantiate(cardPrefab);
        created.name = card;
        TextMeshPro[] texts = created.GetComponentsInChildren<TextMeshPro>();
        foreach (TextMeshPro text in texts) {
            if (text.gameObject.tag == "Number") {
                text.text = Util.ToRoman(number);
            } else {
                text.text = "THE " + card.ToUpper();
            }
        }

        created.transform.position = new Vector3(-7, 7, -10);
    }
    public void AddCreatedToHand() {
        hand.Add(created);
        created = null;
    }
    public bool PlayerIsHolding() {
        return held != null;
    }
    public bool HasChosen() {
        return chosen != null;
    }
    public string GetChosenName() {
        return chosen.name;
    }
    public void DestroyChosen() {
        Destroy(chosen);
        chosen = null;
    }

	void Start () {
        hand = new List<GameObject>();
	}
	
	void Update () {
        // Created card.
        if (created != null) {
            created.transform.position = Vector3.Lerp(created.transform.position, new Vector3(-2, 0, -10), .1f);
        }

        // Cards in hand.
        for (int i = 0; i < hand.Count; i++) {
            if (hand[i] == held) {
                continue;
            }
            Quaternion handRotation;
            Vector3 handPosition = CardInHandTargetTransform(i, hand.Count, out handRotation);
            hand[i].transform.position = Vector3.Lerp(hand[i].transform.position, handPosition, .1f);
            hand[i].transform.position = new Vector3(hand[i].transform.position.x, hand[i].transform.position.y, handPosition.z);
            hand[i].transform.rotation = Quaternion.Lerp(hand[i].transform.rotation, handRotation, .25f);
        }

        // Mouse position.
        Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(new Vector3(0, 0, -1), new Vector3(0, 0, -10f));
        float enter;
        plane.Raycast(mouseRay, out enter);
        Vector3 mousePosition = mouseRay.GetPoint(enter);

        // Hovering and grabbing cards.
        if (held == null && hand.Count > 0) {
            // TODO: Hovering cards.
            float percentY = 1 - (Input.mousePosition.y / Screen.height);
            if (percentY > .8f) {
                int closestIndex = -1;
                float closestSqDist = float.MaxValue;
                for (int i = 0; i < hand.Count; i++) {
                    float sqDist = (hand[i].transform.position - mousePosition).sqrMagnitude;
                    if (sqDist < closestSqDist) {
                        closestIndex = i;
                        closestSqDist = sqDist;
                    }
                }
                if (Input.GetMouseButtonDown(0)) {
                    held = hand[closestIndex];
                }
            }
        }
        // Held card.
        if (held != null) {
            int cx = Screen.width / 2, cy = Screen.height / 2;
            float dx = Input.mousePosition.x - cx, dy = Input.mousePosition.y - (cy * .9f);
            dx /= cy;
            dy /= cy;
            bool hoverAltar = altarScript.GetState() == AltarScript.AltarState.Waiting && Util.Hypot(dx, dy) < .3f;

            if (!Input.GetMouseButton(0)) {
                // Card has been dropped.
                if (hoverAltar) {
                    chosen = held;
                    hand.Remove(chosen);
                    chosen.transform.position = altar.transform.position;
                    chosen.transform.position += altar.transform.up * .01f;
                    chosen.transform.rotation = Quaternion.Euler(70, 0, 0);
                }
                held = null;
                return;
            }
            
            Vector3 heldPosition;
            Quaternion heldAngle;
            if (hoverAltar) {
                heldPosition = altar.transform.position;
                heldPosition += altar.transform.up * .01f;
                heldAngle = Quaternion.Euler(70, 0, 0);
            }
            else {
                heldPosition = mousePosition;
                heldPosition.z -= 1;
                heldDX += heldPosition.x - held.transform.position.x;
                heldDY += heldPosition.y - held.transform.position.y;
                float xTilt = Mathf.Clamp(-25, heldDX * -5, 25);
                float yTilt = Mathf.Clamp(-25, heldDY * 5, 25);
                heldDX *= .75f;
                heldDY *= .75f;
                heldAngle = Quaternion.Euler(yTilt, xTilt, 0);
            }
            held.transform.position = Vector3.Lerp(held.transform.position, heldPosition, .4f);
            held.transform.rotation = Quaternion.Lerp(held.transform.rotation, heldAngle, .4f);
        }
    }

    Vector3 CardInHandTargetTransform(int i, int count, out Quaternion rotation) {
        float maxTheta = Mathf.PI / 7;
        float dTheta = Mathf.PI / 12;
        float theta;
        if (count * dTheta < 2 * maxTheta) {
            float multiplier = i - ((count - 1) / 2f);
            theta = dTheta * multiplier;
        } else {
            maxTheta += count * .01f;
            theta = -maxTheta + i * (2 * maxTheta / count);
        }
        float cy = -10, r = 5f;
        Vector3 position = new Vector3(1.5f * Mathf.Sin(theta) * r, cy + r * Mathf.Cos(theta), -10 - i / 100f);
        rotation = Quaternion.Euler(0, 0, theta * -30);
        return position;
    }
}