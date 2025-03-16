using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public static class TweenUIAnimationsLibrary
{
    public static Tween ChangeUIImageColor(Image UIImage, Color EndColor, float duration)
    {
        return DOTweenModuleUI.DOColor(UIImage, EndColor, duration);
    }
}
