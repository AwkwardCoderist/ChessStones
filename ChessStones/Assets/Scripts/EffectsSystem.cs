using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Pool;
using DG.Tweening;

public class EffectsSystem : MonoBehaviour
{
    public static EffectsSystem Instance;

    private ObjectPool<TextMeshPro> _textPool;

    [SerializeField] private Color _damageColor;
    [SerializeField] private Color _defaultColor;
    [SerializeField] private Color _healColor;
    [SerializeField] private float _yDistance = 2;
    [SerializeField] private float _duration = 2;
    [SerializeField] private float _fontSize = 10;
    [SerializeField] private Vector2 _size;

    [SerializeField] private float _shakeForce;
    [SerializeField] private int _shakeVibrato;
    [SerializeField] private Ease _moveEase;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        _textPool = new ObjectPool<TextMeshPro>(
            () => {
                TextMeshPro text = new GameObject().AddComponent<TextMeshPro>();
                text.alignment = TextAlignmentOptions.Center;
                text.fontSize = _fontSize;
                text.rectTransform.sizeDelta = _size;
                text.gameObject.AddComponent<BillboardObject>().Clamp = new Vector3(0, 360, 0);
                return text;
            }
            );


    }

    public void PlayDamage(Vector3 damagePos, int changeAmount)
    {
        TextMeshPro text = _textPool.Get();

        text.color = _damageColor;
        text.text = '-' + changeAmount.ToString();

        Sequence mySequence = DOTween.Sequence();

        text.transform.position = damagePos;
        text.transform.localScale = Vector3.one;

        mySequence.Append(text.transform.DOShakePosition(_duration, _shakeForce, _shakeVibrato));
        mySequence.Join(text.transform.DOMoveY(damagePos.y + _yDistance, _duration).SetEase(_moveEase));
        mySequence.Append(text.transform.DOScale(Vector3.zero, _duration / 2)).onComplete += () => _textPool.Release(text);

        mySequence.Play();

    }

}
