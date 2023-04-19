using AvatarModel;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class UIDebugLog : MonoBehaviour
{
    public TextMeshProUGUI text;
    public AvatarState AvatarState;

    public void Update()
    {
        text.text = "JumpCharges:" + AvatarState.MutableStats.JumpCharges;
    }
}
