using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CrosshairController : MonoBehaviour, BeatReactor
{
    [SerializeField] private Image _rightScopePrefab;
    [SerializeField] private Image _leftScopePrefab;

    [SerializeField] private RectTransform _leftSpawnPosition;
    [SerializeField] private RectTransform _rightSpawnPosition;
    [SerializeField] private RectTransform _leftEndPosition;
    [SerializeField] private RectTransform _rightEndPosition;

    [SerializeField] private Color _endColor;
    [SerializeField] private Color _spawnColor;
    [SerializeField] private Color _hitInBeatColor;

    [SerializeField] private float _newScale = 0.7f;
    private Vector3 _originalScale;

    private Image[] _leftScopes;
    private Image[] _rightScopes;

    private const int _scopesCount = 4;    
    private int _currentScopes = 0;
    private bool _hitInBeatFixed = false;

    private Vector3 _extremePointDistance;

    protected void Start()
    {
        _spawnColor = _rightScopePrefab.color;
        _spawnColor.a = 0f;

        _leftScopes = new Image[_scopesCount];
        _rightScopes = new Image[_scopesCount];

        _extremePointDistance = _leftEndPosition.position - _leftSpawnPosition.position;
        _originalScale = _leftScopePrefab.transform.localScale;

        for (int i = 0; i < _scopesCount; i++)
        {
            _leftScopes[i] = (Instantiate(_leftScopePrefab, transform));
            _rightScopes[i] = (Instantiate(_rightScopePrefab, transform));

            _rightScopes[i].color = _leftScopes[i].color = _spawnColor;

            _rightScopes[i].rectTransform.position = _rightSpawnPosition.position;
            _leftScopes[i].rectTransform.position = _leftSpawnPosition.position;
        }
    }


    public void SetNewBeatState()
    {
        UpdateBeatState(0);
        ScopesReset(_currentScopes);
        _currentScopes = _currentScopes < _scopesCount - 1 ? _currentScopes + 1 : 0;
        _hitInBeatFixed = false;
    }


    public void UpdateBeatState(float sampleShift)
    {
        ShiftPosition(sampleShift);
        ColorShift(sampleShift);
    }

    private void ScopesReset(int scopesNumber)
    {
        _leftScopes[scopesNumber].color = _rightScopes[scopesNumber].color = _spawnColor;

        _leftScopes[scopesNumber].transform.localScale = _rightScopes[scopesNumber].transform.localScale = _originalScale;

        _leftScopes[scopesNumber].rectTransform.position = _leftSpawnPosition.position;
        _rightScopes[scopesNumber].rectTransform.position = _rightSpawnPosition.position;
    }
    

    private void ShiftPosition(float sampleShift)
    {
        if (!_hitInBeatFixed)
        {
            var shiftPosition = _extremePointDistance * sampleShift;

            _leftScopes[_currentScopes].rectTransform.position = _leftSpawnPosition.position + shiftPosition;
            _rightScopes[_currentScopes].rectTransform.position = _rightSpawnPosition.position - shiftPosition;
        }
    }


    private void ColorShift(float sampleShift)
    {

        if (!_hitInBeatFixed)
        {
            var colorUpdate = Color.Lerp(_spawnColor, _endColor, sampleShift);

            _leftScopes[_currentScopes].color = colorUpdate;
            _rightScopes[_currentScopes].color = colorUpdate;
        }
    }


    public void HitInBeat()
    {
        var scaleUpdate = new Vector3(_newScale, _newScale, 1);

        _leftScopes[_currentScopes].transform.localScale = _rightScopes[_currentScopes].transform.localScale = scaleUpdate;
        _leftScopes[_currentScopes].color = _hitInBeatColor;
        _rightScopes[_currentScopes].color = _hitInBeatColor;

        _hitInBeatFixed = true;
    }
}
