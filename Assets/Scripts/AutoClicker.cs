using System.Threading;
using UnityEngine;

public class AutoClicker : MonoBehaviour
{
    public int ClicksCount = 10;
    public float ClickDelay = 0.05f;
    public bool IsAvailableWhenGranny = false;
    public ScoreKeeper ScoreKeeper;

    private bool _isActive = false;
    private float _timer = 0;
    private int _clicksCompleteCount = 0;

    private void Start()
    {
        _isActive = false;
        _timer = 0;
        _clicksCompleteCount = 0;
    }

    [ContextMenu("StartClicks")]
    private void StartClicks()
    {
        if (_isActive)
        {
            return;
        }

        _isActive = true;
        _timer = 0;
        _clicksCompleteCount = 0;
    }

    private void Update()
    {
        DoClicks();
    }

    private void DoClicks()
    {
        if (!_isActive)
        {
            return;
        }

        _timer += Time.deltaTime;

        if (_timer >= ClickDelay)
        {
            _timer -= ClickDelay;

            if (!IsAvailableWhenGranny && ScoreKeeper._grannyVisible)
            {
                return;
            }

            ScoreKeeper.OnMouseDown();
            _clicksCompleteCount++;
        }

        if (_clicksCompleteCount >= ClicksCount)
        {
            _isActive = false;
        }
    }
}
