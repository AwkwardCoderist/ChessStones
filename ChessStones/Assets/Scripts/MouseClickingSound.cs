using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseClickingSound : MonoBehaviour
{
    [SerializeField] private List<AudioClip> _audioClips = new List<AudioClip>();
    [SerializeField] [Range(0,1)] private float _volume = 1;

    private AudioClip _selectedClip;
    private void Update()
    {
        for(int i = 0; i < _audioClips.Count; i++)
        {
            if (Input.GetMouseButtonDown(i))
            {
                SoundsSystem.Instance.Play(_audioClips[i], _volume);
            }
        }        
    }

}
