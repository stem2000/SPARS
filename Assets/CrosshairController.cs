using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CrosshairController : MonoBehaviour
{
    [SerializeField] private Image _rightScopePrefab;
    [SerializeField] private Image _leftScopePrefab;
    [SerializeField] private RectTransform _leftSpawnPosition;
    [SerializeField] private RectTransform _rightSpawnPosition;
    [SerializeField] private RectTransform _leftEndPosition;
    [SerializeField] private RectTransform _rightEndPosition;

    private Image[] _leftScopes;
    private Image[] _rightScopes;
    private const int _scopesCount = 4;
    private Color _spawnColor;
    [SerializeField] private Color _endColor;
    private float _intervalLenght;
    private Vector3 _extremePointDistance;
    
    private int _currentScopes = 0;

    protected void Start()
    {
        _spawnColor = _rightScopePrefab.color;
        _spawnColor.a = 0.05f;

        _leftScopes = new Image[_scopesCount];
        _rightScopes = new Image[_scopesCount];

        _extremePointDistance = _leftEndPosition.position - _leftSpawnPosition.position;

        for (int i = 0; i < _scopesCount; i++)
        {
            _leftScopes[i] = (Instantiate(_leftScopePrefab, transform));
            _rightScopes[i] = (Instantiate(_rightScopePrefab, transform));
            _rightScopes[i].color = _leftScopes[i].color = _spawnColor;
            _rightScopes[i].rectTransform.position = _rightSpawnPosition.position;
            _leftScopes[i].rectTransform.position = _leftSpawnPosition.position;
        }
    }


    public void GetBeat(float intervalLenght, float intervalPart)
    {
        _intervalLenght = intervalLenght;
        UpdateBeatState(0);
        ScopesReset(_currentScopes);
        _currentScopes = _currentScopes < _scopesCount - 1 ? _currentScopes + 1 : 0;
    }


    public void UpdateBeatState(float sampleShift)
    {
        var colorUpdate = _leftScopes[_currentScopes].color;
        colorUpdate = Color.Lerp(_spawnColor, _endColor, sampleShift);

        var shiftPosition = _extremePointDistance * sampleShift;

        _leftScopes[_currentScopes].rectTransform.position = _leftSpawnPosition.position + shiftPosition;
        _rightScopes[_currentScopes].rectTransform.position = _rightSpawnPosition.position - shiftPosition;
 
        _leftScopes[_currentScopes].color = colorUpdate;
        _rightScopes[_currentScopes].color = colorUpdate;
    }

    private void ScopesReset(int scopesNumber)
    {
        _leftScopes[scopesNumber].color = _rightScopes[scopesNumber].color = _spawnColor;
        _leftScopes[scopesNumber].rectTransform.position = _leftSpawnPosition.position;
        _rightScopes[scopesNumber].rectTransform.position = _rightSpawnPosition.position;
    }

}
