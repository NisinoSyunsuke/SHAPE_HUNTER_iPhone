using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEvent : MonoBehaviour
{
    public void SoundPlay(AnimationEvent animationEvent)
    {
        string name = animationEvent.stringParameter;
        float pitch = animationEvent.intParameter * 0.1f;
        float volume = animationEvent.floatParameter;
        GameManager.gameManager.soundManager.Play(name, pitch, volume);
    }
}
