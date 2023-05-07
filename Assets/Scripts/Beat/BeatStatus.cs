using AvatarModel;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BeatStatus : MonoBehaviour, BeatReactor
{
    [SerializeField] TextMeshProUGUI _text;
    [SerializeField] float _textVisibilityTime = 0.6f;


    public void GetBeatAction(BeatAction actType, float sampleThird)
    {
        if(actType == BeatAction.Miss)
            UpdateText(ActQuality.Miss, Color.red);
        else
            AnalyseSampleThird(sampleThird);
    }

    private void UpdateText(ActQuality actQual, Color color)
    {
        switch(actQual)
        {
            case ActQuality.ClearingAct:
                _text.text = string.Empty;
                _text.faceColor = color;
                break;
            case ActQuality.Miss:
                _text.text = ActQuality.Miss.ToString();
                _text.faceColor = color;
                break;
            case ActQuality.Weak:
                _text.text = ActQuality.Weak.ToString();
                _text.faceColor = color;
                break;
            case ActQuality.Good:
                _text.text = ActQuality.Good.ToString();
                _text.faceColor = color;
                break;
            case ActQuality.Great:
                _text.text = ActQuality.Great.ToString();
                _text.faceColor = color;
                break;
        }
    }

    private void AnalyseSampleThird(float sampleThird)
    {
        if(sampleThird < 0.3f)
            UpdateText(ActQuality.Weak, Color.yellow);
        else if(sampleThird < 0.6f)
            UpdateText(ActQuality.Good, Color.blue);
        else if(sampleThird < 1)
            UpdateText(ActQuality.Great, Color.green);
    }

    public void MoveToNextSample()
    {
        return;
    }

    public void UpdateCurrentSampleState(float sampleState)
    {
        return;
    }

    private enum ActQuality
    {
        Miss, Weak, Good, Great, ClearingAct
    }
}
