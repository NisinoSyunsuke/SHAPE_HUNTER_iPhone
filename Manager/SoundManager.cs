using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [System.Serializable]
    public class SoundData
    {
        public string name;
        public AudioClip audioClip;
        public float playedTime;    //�O��Đ���������
    }

    [SerializeField]
    private SoundData[] soundDatas;

    //AudioSource�i�X�s�[�J�[�j�𓯎��ɖ炵�������̐������p��
    private AudioSource[] audioSourceList = new AudioSource[20];

    //�ʖ�(name)���L�[�Ƃ����Ǘ��pDictionary
    private Dictionary<string, SoundData> soundDictionary = new Dictionary<string, SoundData>();

    //��x�Đ����Ă���A���Đ��o����܂ł̊Ԋu(�b)
    [SerializeField]
    private float playableDistance = 0.2f;

    public void StartProcess()
    {
        //auidioSourceList�z��̐�����AudioSource���������g�ɐ������Ĕz��Ɋi�[
        for (var i = 0; i < audioSourceList.Length; ++i)
        {
            audioSourceList[i] = gameObject.AddComponent<AudioSource>();
        }

        //soundDictionary�ɃZ�b�g
        foreach (var soundData in soundDatas)
        {
            soundDictionary.Add(soundData.name, soundData);
        }

        //bgmDictionary�ɃZ�b�g
        foreach (var bgmData in bgmData)
        {
            bgmDictionary.Add(bgmData.name, bgmData);
        }
    }
    //���g�p��AudioSource�̎擾 �S�Ďg�p���̏ꍇ��null��ԋp
    private AudioSource GetUnusedAudioSource()
    {
        for (var i = 0; i < audioSourceList.Length; ++i)
        {
            if (audioSourceList[i].isPlaying == false) return audioSourceList[i];
        }

        return null; //���g�p��AudioSource�͌�����܂���ł���
    }

    //�w�肳�ꂽAudioClip�𖢎g�p��AudioSource�ōĐ�
    public void Play(AudioClip clip, float pitch, float volume)
    {
        /*if (stageType == StageType.level)
        {
            if (GameManager.gameManager.applicationQuit || GameManager.gameManager.sceneUnloaded) return;
        }*/

        var audioSource = GetUnusedAudioSource();
        if (audioSource == null) return; //�Đ��ł��܂���ł���
        audioSource.clip = clip;
        audioSource.pitch = pitch;
        audioSource.volume = volume * DataBase.MasterVolume * 2;
        audioSource.Play();
    }

    //�w�肳�ꂽ�ʖ��œo�^���ꂽAudioClip���Đ�
    public void Play(string name, float pitch, float volume)
    {
        if (soundDictionary.TryGetValue(name, out var soundData)) //�Ǘ��pDictionary ����A�ʖ��ŒT��
        {
            if (Time.realtimeSinceStartup - soundData.playedTime < playableDistance) return;    //�܂��Đ�����ɂ͑���
            soundData.playedTime = Time.realtimeSinceStartup;//����p�ɍ���̍Đ����Ԃ̕ێ�
            Play(soundData.audioClip, pitch, volume); //����������A�Đ�
        }
        else
        {
            Debug.LogWarning($"���̕ʖ��͓o�^����Ă��܂���:{name}");
        }
    }
    #region//BGM
    [System.Serializable]
    public class BGMdata
    {
        public string name;
        public AudioClip audioClip;
    }

    [SerializeField] BGMdata[] bgmData;
    Dictionary<string, BGMdata> bgmDictionary = new Dictionary<string, BGMdata>();

    [SerializeField] AudioSource bgmAudioSource;

    float record_volume;

    public void BgmPlay(string name, float volume, float pitch = 1f)
    {
        if (bgmDictionary.TryGetValue(name, out var BGMdata))
        {
            float bgm_volume = volume * DataBase.MasterVolume * 2;
            
            bgmAudioSource.volume = bgm_volume; record_volume = volume;
            bgmAudioSource.pitch = pitch;

            if (bgmAudioSource.clip == BGMdata.audioClip) return; //���łɃZ�b�g����Ă�����return

            bgmAudioSource.clip = BGMdata.audioClip;
            bgmAudioSource.Play();
        }
        else
        {
            Debug.LogWarning($"���̕ʖ��͓o�^����Ă��܂���:{name}");
        }
    }
    public void BgmPlay(float volume, float pitch = 1f)
    {
        bgmAudioSource.volume = volume * DataBase.MasterVolume * 2;
        bgmAudioSource.pitch = pitch;
    }
    public void BGMStop()
    {
        bgmAudioSource.Stop();
    }
    #endregion
}
