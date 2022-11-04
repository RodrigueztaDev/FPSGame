using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{

    public enum ScoreLevel
    {
        kF,
        kD,
        kC,
        kB,
        kA,
        kS,
        kSS,
        kSSS
    }

    public float comboLoseRate_;

    private ScoreLevel scoreLevel_;
    private int totalScore_;

    void Start()
    {
        scoreLevel_ = ScoreLevel.kF;
        UIManager.Instance.ChangeComboText(scoreLevel_);
    }

    void Update()
    {
        UIManager.Instance.AddComboValue(-(comboLoseRate_ * Time.deltaTime));
        if(UIManager.Instance.GetComboValue() <= 0.0f)
        {
            DowngradeCombo();
        }
        else if(UIManager.Instance.GetComboValue() >= 1.0f)
        {
            UpgradeCombo();
        }
    }

    public void AddScore(int score)
    {
        totalScore_ += score;

        UIManager.Instance.AddComboValue(score / (10000.0f * (float)scoreLevel_));
    }

    private void UpgradeCombo()
    {
        if(scoreLevel_ != ScoreLevel.kSSS)
        {
            scoreLevel_++;
            UIManager.Instance.SetComboValue(0.01f);
            UIManager.Instance.ChangeComboText(scoreLevel_);
        }
    }

    private void DowngradeCombo()
    {
        if (scoreLevel_ != ScoreLevel.kD)
        {
            scoreLevel_--;
            UIManager.Instance.SetComboValue(0.99f);
            UIManager.Instance.ChangeComboText(scoreLevel_);
        }
    }
}
