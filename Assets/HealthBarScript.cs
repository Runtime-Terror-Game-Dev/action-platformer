using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine;

public class HealthBarScript : MonoBehaviour
{
    public Image HealthBar;

    public int damage; 

    private void LoseLife(int damage, float Duration)
    {
        if (damage > 0)
        {
            HealthBar.DOFade(0, Duration);
        }
    }
}
