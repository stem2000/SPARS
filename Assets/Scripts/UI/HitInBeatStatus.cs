using System;
using TMPro;
using UnityEngine;

[Serializable]
public class HitInBeatStatus : BeatReactor
{
    [SerializeField] TextMeshProUGUI _text;
    private int _actCount = 0;


    public void GetBeatAction(BeatAction actType, float sampleThird)
    {
        if(actType == BeatAction.Miss)
            UpdateText(ActQuality.Miss);
        else
            AnalyseSampleThird(sampleThird);
    }

    private void UpdateText(ActQuality actQual)
    {
        _actCount++;
        switch (actQual)
        {
            case ActQuality.ClearingAct:
                _text.text = string.Empty;
                _text.faceColor = Color.white;
                break;
            case ActQuality.Miss:
                _text.text = ActQuality.Miss.ToString() + " " + _actCount;
                _text.faceColor = Color.red;
                break;
            case ActQuality.Weak:
                _text.text = ActQuality.Weak.ToString() + " " + _actCount;
                _text.faceColor = Color.yellow;
                break;
            case ActQuality.Good:
                _text.text = ActQuality.Good.ToString() + " " + _actCount;
                _text.faceColor = Color.green;
                break;
            case ActQuality.Great:
                _text.text = ActQuality.Great.ToString() + " " + _actCount;
                _text.faceColor = Color.blue;
                break;
        }
    }

    private void AnalyseSampleThird(float sampleThird)
    {
        if(sampleThird < 0.3f)
            UpdateText(ActQuality.Weak);
        else if(sampleThird < 0.6f)
            UpdateText(ActQuality.Good);
        else if(sampleThird < 1)
            UpdateText(ActQuality.Great);
    }

    public void MoveToNextSample()
    {
        return;
    }

    public void UpdateCurrentSampleState(float sampleState)
    {
        return;
    }

    public void SubscibeToUpdateSampleEvents()
    {
        ServiceProvider.SubscribeToBeatStart(MoveToNextSample);
        ServiceProvider.SubscribeToBeatUpdate(UpdateCurrentSampleState);
    }

    private enum ActQuality
    {
        Miss, Weak, Good, Great, ClearingAct
    }
}
