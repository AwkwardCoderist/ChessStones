using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundSystemPlayer : MonoBehaviour
{
    [SerializeField] private List<AudioClip> _audioClips = new List<AudioClip>();
    [SerializeField][Range(0, 1)] private float _volume = 1;

    [SerializeField] private float _playDelay;
    private float _lastPlayTime;

    private AudioClip _selectedClip;

    private void Start()
    {
        Button btn = GetComponentInParent<Button>();
        if (btn) btn.onClick.AddListener(PlaySound);
    }

    private void Update()
    {
        if (_playDelay > 0)
        {
            if(Time.time > _lastPlayTime + _playDelay)
            {
                PlaySound();
            }
        }
    }

    public void PlaySound()
    {
        if (_audioClips.Count > 1) _selectedClip = _audioClips[Random.Range(0, _audioClips.Count)];
        else if (_audioClips.Count > 0) _selectedClip = _audioClips[0];

        SoundsSystem.Instance.Play(_selectedClip, _volume);

        _lastPlayTime = Time.time;
    }
}
