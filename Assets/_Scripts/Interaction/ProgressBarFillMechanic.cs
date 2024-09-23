using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBarFillMechanic
{

    // Use this mechanic for anything that requires a progressBar fill.
    // Pass on progressBar image, Class using the coroutine and a duration of the fill.
    // Use methods below to control the coroutine.
    private float _duration;
    private float _currentTime;
    private Image _progressBar;
    private Action _onComplete;
    private Coroutine _progressCoroutine;

    private MonoBehaviour _coroutineOwner;

    public ProgressBarFillMechanic (Image progressBar, MonoBehaviour coroutineOwner, float duration)
    {
        this._progressBar = progressBar;
        this._duration = duration;
        this._coroutineOwner = coroutineOwner;
        this._currentTime = 0f;
    }

    public void StartProgress(Action onComplete)
    {
        this._onComplete = onComplete;
        ResetProgress();
        _progressCoroutine = _coroutineOwner.StartCoroutine(UpdateProgress());
    }

    public void StopAndResetProgress()
    {
        if (_progressCoroutine != null)
        {
            _coroutineOwner.StopCoroutine(_progressCoroutine);
            _progressCoroutine = null;
        }
        ResetProgress();
    }

    private void ResetProgress()
    {
        _currentTime = 0f;
        _progressBar.fillAmount = 0f;
    }

    private IEnumerator UpdateProgress()
    {
        while (_currentTime < _duration)
        {
            if (InputManager.Instance.CleanAction.ReadValue<float>() == 0f)
            {
                StopAndResetProgress();
                yield break;
            }
            _currentTime += Time.deltaTime;
            _progressBar.fillAmount = Mathf.Clamp01(_currentTime / _duration);

            if (_currentTime >= _duration)
            {
                _onComplete?.Invoke();
                yield break;
            }

            yield return null;
        }
    }

}
