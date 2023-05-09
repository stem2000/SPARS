using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class DashStatus
{
    [SerializeField] private Image _dashStroke;

    protected void Start()
    {
        _dashStroke.fillClockwise = true;
    }

    public void ResetDashStroke(float fillDuration)
    {
        _dashStroke.fillAmount = 0f;
        ObjectServiceProvider.RunCoroutine(DrawStroke(fillDuration));
    }

    private IEnumerator DrawStroke(float fillDuration)
    {
        var elapsedTime = 0f;

        while (elapsedTime < fillDuration) 
        {
            elapsedTime += Time.deltaTime;
            _dashStroke.fillAmount = elapsedTime / fillDuration;
            yield return null;
        }

        _dashStroke.fillAmount = 1f;
    }
}
