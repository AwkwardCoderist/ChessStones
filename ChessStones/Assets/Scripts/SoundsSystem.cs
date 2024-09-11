using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class SoundsSystem : MonoBehaviour
{
    public static SoundsSystem Instance;

    private ObjectPool<AudioSource> _audioPool;

    private AudioSource _audio;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        _audioPool = new ObjectPool<AudioSource>(
            () => {
                AudioSource src = new GameObject().AddComponent<AudioSource>();
                return src; 
            },
            (onGet) => { onGet.Stop(); },
            (onRelease) => { onRelease.Stop(); },
            (onDestroy) => { onDestroy.Stop(); }
            );


    }

    public void Play(AudioClip clip, float volume = 1)
    {
        _audio = _audioPool.Get();
        _audio.clip = clip;
        _audio.volume = volume;
        _audio.Play();
        StartCoroutine(OnEndAudio(_audio));
    }

    private IEnumerator OnEndAudio(AudioSource source)
    {
        yield return new WaitUntil(() => source.isPlaying == false);

        _audioPool.Release(source);
    }

}
