using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New foostep collection", menuName = "Create new footstep collection")]
public class FootstepCollection : ScriptableObject
{
    public List<AudioClip> footstepSounds = new List<AudioClip>();
    public AudioClip jumpSound;
    public AudioClip landsound;
}
