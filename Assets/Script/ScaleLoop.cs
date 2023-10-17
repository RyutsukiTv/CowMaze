using UnityEngine;
using TMPro;
using DG.Tweening; 

public class ScaleLoop : MonoBehaviour
{
    public TextMeshProUGUI textMeshPro;
    public float scaleDuration = 1.0f;

    private void Start()
    {
        ScaleUp();
        int score = 0;
        PlayerPrefs.SetInt("Score", score);
        PlayerPrefs.Save();
    }

    private void ScaleUp()
    {
        textMeshPro.transform.DOScale(Vector3.one * 2, scaleDuration)
            .SetEase(Ease.OutQuad) 
            .OnComplete(ScaleDown);
    }

    private void ScaleDown()
    {
        textMeshPro.transform.DOScale(Vector3.one, scaleDuration)
            .SetEase(Ease.InQuad) 
            .OnComplete(ScaleUp);
    }
}
