using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CrosshairController : MonoBehaviour, BeatReactor
{
    [SerializeField] private Image _rightScopePrefab;
    [SerializeField] private Image _leftScopePrefab;
    [SerializeField] private Image _staticLeftScope;
    [SerializeField] private Image _staticRightScope;
    [SerializeField] private RectTransform _leftSpawnPosition;
    [SerializeField] private RectTransform _rightSpawnPosition;
       

    [SerializeField] private Color _afterHitColorS;
    [SerializeField] private Color _defaultColorS;

    [SerializeField] private Color _startPosColorD;
    [SerializeField] private Color _endPosColorD;

    [SerializeField] private float _rescaleSize = 0.7f;
    private Vector3 _defaultScale;

    private Image[] _leftScopes;
    private Image[] _rightScopes;

    private RectTransform _endPosition;

    private const int _scopesCount = 3;    
    private int _currentScopes = 0;
    private bool _hitInBeatFixed = false;
    private bool _staticScopesChanged = false;

    private Vector3 _extremePointDistance;

    #region CROSSHAIR_CONTROLLER_METHODS
    public void MoveToNextSample()
    {
        UpdateCurrentSampleState(0);

        if(!_hitInBeatFixed)
            ResetDynamicScopes(_currentScopes);
        else 
            _hitInBeatFixed = false;

        _currentScopes = _currentScopes < _scopesCount - 1 ? _currentScopes + 1 : 0;
    }


    public void UpdateCurrentSampleState(float sampleShift)
    {
        if (!_hitInBeatFixed)
        {
            ShiftPosition(sampleShift);
            ColorShift(sampleShift);
        }
        else ResetDynamicScopes(_currentScopes);

        if (_staticScopesChanged && !_hitInBeatFixed && sampleShift > 0.2f)
        {
            ResetStaticScopes();
            _staticScopesChanged = false;
        }
    }


    private void ResetDynamicScopes(int scopesNumber)
    {
        _leftScopes[scopesNumber].rectTransform.position = _leftSpawnPosition.position;
        _rightScopes[scopesNumber].rectTransform.position = _rightSpawnPosition.position;

        _rightScopes[scopesNumber].color = _leftScopes[scopesNumber].color = _startPosColorD;
    }


    private void ResetStaticScopes()
    {
        _staticLeftScope.color = _staticRightScope.color = _defaultColorS;
        _staticLeftScope.transform.localScale = _staticRightScope.transform.localScale = _defaultScale;
    }
    

    private void ShiftPosition(float sampleShift)
    {
        var shiftPosition = _extremePointDistance * sampleShift;

        _leftScopes[_currentScopes].rectTransform.position = _leftSpawnPosition.position + shiftPosition;
        _rightScopes[_currentScopes].rectTransform.position = _rightSpawnPosition.position - shiftPosition;
    }


    private void ColorShift(float sampleShift)
    {
        var colorUpdate = Color.Lerp(_startPosColorD, _endPosColorD, sampleShift);

        _leftScopes[_currentScopes].color = colorUpdate;
        _rightScopes[_currentScopes].color = colorUpdate;
    }


    public void HitInBeat()
    {
        var scaleUpdate = new Vector3(_rescaleSize, _rescaleSize, 1);

        _staticLeftScope.transform.localScale = _staticRightScope.transform.localScale = scaleUpdate;
        _staticLeftScope.color = _afterHitColorS;
        _staticRightScope.color = _afterHitColorS;

        _hitInBeatFixed = true;
        _staticScopesChanged = true;
    }


    private void InitializeComponents()
    {
        _startPosColorD.a = 0f;
        _endPosColorD.a = 1f;

        _leftScopes = new Image[_scopesCount];
        _rightScopes = new Image[_scopesCount];

        InitializeEndPosition();

        _extremePointDistance = _endPosition.position - _leftSpawnPosition.position;
        _defaultScale = _leftScopePrefab.transform.localScale;

        FillScopesArrays();
    }


    private void InitializeEndPosition()
    {
        _endPosition = _staticLeftScope.rectTransform;
    }


    private void FillScopesArrays()
    {
        for (int i = 0; i < _scopesCount; i++)
        {
            _leftScopes[i] = (Instantiate(_leftScopePrefab, transform));
            _rightScopes[i] = (Instantiate(_rightScopePrefab, transform));

            _rightScopes[i].color = _leftScopes[i].color = _startPosColorD;

            _rightScopes[i].rectTransform.position = _rightSpawnPosition.position;
            _leftScopes[i].rectTransform.position = _leftSpawnPosition.position;
        }
    }
    #endregion

    #region MONOBEHAVIOUR_METHODS
    protected void Start()
    {
        InitializeComponents();
    }
    #endregion
}
