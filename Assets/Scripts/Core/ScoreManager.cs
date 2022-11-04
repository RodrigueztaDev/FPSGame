using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{

    public enum ScoreLevel
    {
        kF = 1,
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

    static private ScoreManager instance_;
    static public ScoreManager Instance
    {
        get { return instance_; }
    }

    void Start()
    {
        if (instance_ == null) instance_ = this;
        scoreLevel_ = ScoreLevel.kF;
    }

    void LateUpdate()
    {
        UIManager.Instance.AddComboValue(-(comboLoseRate_ * Time.deltaTime));
        if(UIManager.Instance.GetComboValue() <= 0.0f)
        {
            DowngradeCombo();
        }
    }

    public void AddScore(int score)
    {
        totalScore_ += score;

        float scoreToAdd = score / (10000.0f * (float)scoreLevel_);

        if (UIManager.Instance.GetComboValue() + scoreToAdd >= 1.0f)
        {
            UpgradeCombo();
            scoreToAdd -= UIManager.Instance.GetComboValue();
            return;
        }
        UIManager.Instance.AddComboValue(scoreToAdd);
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
        if (scoreLevel_ != ScoreLevel.kF)
        {
            scoreLevel_--;
            UIManager.Instance.SetComboValue(0.99f);
            UIManager.Instance.ChangeComboText(scoreLevel_);
        }
    }
}
