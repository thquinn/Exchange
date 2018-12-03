﻿using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StoryControllerScript : MonoBehaviour {
    public TextAsset itemAsset;
    public Dictionary<string, string> items;
    public TextAsset[] pageAssets;
    Dictionary<string, Page> pages;
    Page currentPage;
    int index, subIndex;
    ContinueTrigger continueTrigger;
    bool intro = true;
    int score;

    // Display.
    public CardControllerScript cardScript;
    public AltarScript altarScript;
    public Image globalFade;
    public CanvasGroup introFade, mainTextFade, acceptCardDescriptionFade, daysLeftFade;
    public RectTransform rootCanvasTransform;
    public TextMeshProUGUI mainText, introText, promptLabel, acceptCardDescriptionLabel;
    public ScrollRect scrollRect;
    bool needScrollToBottom = false;
    public TextMeshProUGUI timeLabel, daysLeftLabel;
    int day;
    string time;
    int timer;

	void Start () {
        score = 0;

        introText.text = "";
        mainText.text = "";
        introFade.alpha = 1;
        mainTextFade.alpha = 0;
        acceptCardDescriptionFade.alpha = 0;

        items = new Dictionary<string, string>();
        string[] itemAssetTokens = Regex.Split(itemAsset.text, "\r\n|\n|\r");
        for (int i = 0; i < itemAssetTokens.Length; i += 2) {
            items.Add(itemAssetTokens[i], itemAssetTokens[i + 1]);
        }
        pages = new Dictionary<string, Page>();
		foreach (TextAsset pageAsset in pageAssets) {
            string name = pageAsset.name;
            string[] lines = Regex.Split(pageAsset.text, "\r\n|\n|\r");
            pages.Add(name, new Page(lines));
        }
        GoToPage("start");
        Time startTime = (Time)CurrentLineOrSubline();
        day = startTime.day;
        time = startTime.time;
	}
    void GoToPage(string name) {
        currentPage = pages[name];
        index = 0;
        subIndex = -1;
        SetContinueTrigger(CurrentLineOrSubline().GetLineType(), currentPage.lines[1].GetLineType());
    }
    Line CurrentLineOrSubline() {
        Line currentLine = currentPage.lines[index];
        if (currentLine.GetLineType() == LineType.Sacrifice) {
            return ((Sacrifice)currentLine).sublines[subIndex];
        }
        return currentLine;
    }
    LineType NextType() {
        if (subIndex == -1)
            return currentPage.lines[index + 1].GetLineType();
        Sacrifice sacrificeLine = (Sacrifice)(currentPage.lines[index]);
        if (subIndex < sacrificeLine.sublines.Length - 1) {
            return sacrificeLine.sublines[subIndex + 1].GetLineType();
        }
        int seekIndex = index;
        while (true) {
            seekIndex++;
            LineType type = currentPage.lines[seekIndex].GetLineType();
            if (type != LineType.Sacrifice) {
                return type;
            }
        }
    }

    static HashSet<ContinueTrigger> mainTextFadeTriggers = new HashSet<ContinueTrigger>() { ContinueTrigger.AcceptCard, ContinueTrigger.AddCardToHand, ContinueTrigger.HideTimer, ContinueTrigger.LossFadeIn, ContinueTrigger.LossFadeOut, ContinueTrigger.RevealFade };
    void LateUpdate () {
        if (mainTextFadeTriggers.Contains(continueTrigger) || cardScript.PlayerIsHolding()) {
            mainTextFade.alpha = Mathf.Max(0, mainTextFade.alpha -= .05f);
        } else {
            mainTextFade.alpha = Mathf.Min(1, mainTextFade.alpha += .05f);
        }
        if (needScrollToBottom || Input.GetKeyDown(KeyCode.A)) {
            scrollRect.verticalNormalizedPosition = 0;
            // HACK: Why do we need to do this? Forcing layout on the content fitter, and even all canvases, does nothing.
            mainText.gameObject.SetActive(false);
            mainText.gameObject.SetActive(true);
            // END HACK
            needScrollToBottom = false;
        }
        promptLabel.enabled = false;

        if (continueTrigger == ContinueTrigger.AcceptCard) {
            if (acceptCardDescriptionFade.alpha < 1) {
                acceptCardDescriptionFade.alpha += .04f;
            }
            if (Input.GetMouseButtonDown(0)) {
                cardScript.AddCreatedToHand();
                continueTrigger = ContinueTrigger.AddCardToHand;
            }
        }
        else if (continueTrigger == ContinueTrigger.AddCardToHand) {
            if (acceptCardDescriptionFade.alpha > 0) {
                acceptCardDescriptionFade.alpha -= .04f;
            }
            else {
                continueTrigger = ContinueTrigger.HideTimer;
                timer = 30;
            }
        }
        else if (continueTrigger == ContinueTrigger.Click) {
            promptLabel.enabled = true;
            promptLabel.text = "Click to continue.";
            promptLabel.color = new Color(.8f, .8f, 1);
            bool clicked = Input.GetMouseButtonDown(0);
            if (clicked && !intro) {
                PointerEventData pointer = new PointerEventData(EventSystem.current);
                pointer.position = Input.mousePosition;
                List<RaycastResult> raycastResults = new List<RaycastResult>();
                EventSystem.current.RaycastAll(pointer, raycastResults);
                clicked = raycastResults.Count > 0 && raycastResults[0].gameObject.tag == "Click";
            }
            if (clicked) {
                continueTrigger = ContinueTrigger.None;
            }
        }
        else if (continueTrigger == ContinueTrigger.ClearFade) {
            if (globalFade.color.a < 1) {
                Color color = new Color(0, 0, 0, globalFade.color.a + .01f);
                globalFade.color = color;
            }
            else {
                introText.text = "";
                Color color = new Color(0, 0, 0, 0);
                globalFade.color = color;
                continueTrigger = ContinueTrigger.Timer;
                timer = 60;
            }
        }
        else if (continueTrigger == ContinueTrigger.HideTimer) {
            timer--;
            if (timer == 0) {
                continueTrigger = ContinueTrigger.None;
            }
        }
        else if (continueTrigger == ContinueTrigger.LossFadeIn) {
            if (globalFade.color.a < 1) {
                Color color = new Color(255, 0, 0, Mathf.Min(1, globalFade.color.a + .1f));
                globalFade.color = color;
            }
            else {
                // TODO: Not handling Lose lines for non-sacrificed cards.
                cardScript.DestroyChosen();
                altarScript.Off();
                continueTrigger = ContinueTrigger.LossFadeOut;
            }
        }
        else if (continueTrigger == ContinueTrigger.LossFadeOut) {
            if (globalFade.color.a > 0) {
                Color color = new Color(255, 0, 0, Mathf.Max(0, globalFade.color.a - .01f));
                globalFade.color = color;
            }
            else {
                continueTrigger = ContinueTrigger.HideTimer;
                timer = 30;
            }
        }
        else if (continueTrigger == ContinueTrigger.RevealDaysLeft) {
            if (daysLeftFade.alpha < 1) {
                daysLeftFade.alpha = Mathf.Min(1, daysLeftFade.alpha + .1f);
            } else {
                continueTrigger = ContinueTrigger.None;
            }
        }
        else if (continueTrigger == ContinueTrigger.RevealFade) {
            if (introFade.alpha > 0) {
                introFade.alpha -= .01f;
            }
            else {
                intro = false;
                continueTrigger = ContinueTrigger.HideTimer;
                timer = 60;
            }
        }
        else if (continueTrigger == ContinueTrigger.Sacrifice) {
            promptLabel.enabled = true;
            promptLabel.text = "Make a sacrifice.";
            promptLabel.color = new Color(1, .33f, .33f);
            if (cardScript.HasChosen()) {
                string chosen = cardScript.GetChosenName();
                while (true) {
                    index++;
                    Sacrifice sacrificeLine = (Sacrifice)(currentPage.lines[index]);
                    if (sacrificeLine.card == chosen) {
                        break;
                    }
                }
                altarScript.Chosen();
                continueTrigger = ContinueTrigger.None;
            }
        }
        else if (continueTrigger == ContinueTrigger.Timer) {
            timer--;
            if (timer == 0) {
                continueTrigger = ContinueTrigger.None;
            }
        }
        else if (continueTrigger == ContinueTrigger.TimeUpdate && UnityEngine.Time.frameCount % 3 == 0) {
            Time timeLine = (Time)CurrentLineOrSubline();
            if (day < timeLine.day) {
                day++;
            }
            if (day == timeLine.day) {
                time = timeLine.time;
                continueTrigger = ContinueTrigger.None;
            }
            UpdateTimeLabel();
        }

        if (continueTrigger == ContinueTrigger.None) {
            // Execute the next line.
            if (currentPage.lines[index].GetLineType() == LineType.Sacrifice) {
                subIndex++;
                Sacrifice sacrificeLine = (Sacrifice)currentPage.lines[index];
                if (subIndex >= sacrificeLine.sublines.Length) {
                    subIndex = -1;
                    while (currentPage.lines[index].GetLineType() == LineType.Sacrifice) {
                        index++;
                        // TODO: Prevent out of bounds for pages that end in sacrifice.
                    }
                }
            }
            else {
                index++;
            }
            LineType currentType = CurrentLineOrSubline().GetLineType();
            if (subIndex >= 0) {
                Debug.Log("Continuing to line " + index + "," + subIndex + ": " + currentType);
            }
            else {
                Debug.Log("Continuing to line " + index + ": " + currentType);
            }
            if (currentType == LineType.Go) {
                Go goLine = (Go)CurrentLineOrSubline();
                GoToPage(goLine.page);
                currentType = CurrentLineOrSubline().GetLineType();
            } else if (currentType == LineType.Score) {
                Score scoreLine = (Score)CurrentLineOrSubline();
                score += scoreLine.points;
            } else if (currentType == LineType.Text) {
                Text textLine = (Text)CurrentLineOrSubline();
                string newText = mainText.text + (mainText.text.Length > 0 ? "\n\n" : "") + textLine.text;
                mainText.SetText(newText);
                if (intro) {
                    if (introText.text.Length > 0) {
                        introText.text += "\n\n";
                    }
                    introText.text += textLine.text;
                }
                needScrollToBottom = true;
            }

            // Determine the next continue trigger.
            LineType nextType = NextType();
            SetContinueTrigger(currentType, nextType);
            Debug.Log("New continue trigger: " + continueTrigger + " (" + currentType + " -> " + nextType + ")");
        }
	}
    void SetContinueTrigger(LineType currentType, LineType nextType) {
        if (currentType == LineType.Clear) {
            continueTrigger = ContinueTrigger.ClearFade;
            return;
        }
        if (currentType == LineType.Get) {
            Get getLine = (Get)CurrentLineOrSubline();
            cardScript.CreateCard(getLine.card);
            acceptCardDescriptionLabel.text = items[getLine.card];
            continueTrigger = ContinueTrigger.AcceptCard;
            return;
        }
        if (currentType == LineType.Lose) {
            continueTrigger = ContinueTrigger.LossFadeIn;
            altarScript.Activate();
            return;
        }
        if (currentType == LineType.Reveal) {
            continueTrigger = intro ? ContinueTrigger.RevealFade : ContinueTrigger.RevealDaysLeft;
            return;
        }
        if (nextType == LineType.Sacrifice) {
            altarScript.Await();
            continueTrigger = ContinueTrigger.Sacrifice;
            return;
        }
        if (currentType == LineType.Text) {
            continueTrigger = ContinueTrigger.Click;
            return;
        }
        if (currentType == LineType.Time) {
            continueTrigger = ContinueTrigger.TimeUpdate;
            return;
        }
        continueTrigger = ContinueTrigger.None;
    }
    void UpdateTimeLabel() {
        int month = ((day - 1) / 30) % 12;
        int dayOfMonth = (day - 1) % 30 + 1;
        string suffix;
        if (dayOfMonth > 10 && dayOfMonth < 21) {
            suffix = "th";
        }
        else if (dayOfMonth % 10 == 1) {
            suffix = "st";
        }
        else if (dayOfMonth % 10 == 2) {
            suffix = "nd";
        }
        else if (dayOfMonth % 10 == 3) {
            suffix = "rd";
        }
        else {
            suffix = "th";
        }
        timeLabel.text = dayOfMonth + suffix + " " + Constants.MONTHS[month] + ", " + time;

        int daysRemaining = 368 - day;
        daysLeftLabel.text = daysRemaining + (daysRemaining == 1 ? " day" : " days") + " remaining";
    }

    enum ContinueTrigger {
        None, AcceptCard, AddCardToHand, ClearFade, Click, HideTimer, LossFadeIn, LossFadeOut, RevealDaysLeft, RevealFade, Sacrifice, Timer, TimeUpdate
    }
}