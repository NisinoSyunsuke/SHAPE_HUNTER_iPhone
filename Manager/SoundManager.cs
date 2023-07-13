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
        public float playedTime;    //前回再生した時間
    }

    [SerializeField]
    private SoundData[] soundDatas;

    //AudioSource（スピーカー）を同時に鳴らしたい音の数だけ用意
    private AudioSource[] audioSourceList = new AudioSource[20];

    //別名(name)をキーとした管理用Dictionary
    private Dictionary<string, SoundData> soundDictionary = new Dictionary<string, SoundData>();

    //一度再生してから、次再生出来るまでの間隔(秒)
    [SerializeField]
    private float playableDistance = 0.2f;

    public void StartProcess()
    {
        //auidioSourceList配列の数だけAudioSourceを自分自身に生成して配列に格納
        for (var i = 0; i < audioSourceList.Length; ++i)
        {
            audioSourceList[i] = gameObject.AddComponent<AudioSource>();
        }

        //soundDictionaryにセット
        foreach (var soundData in soundDatas)
        {
            soundDictionary.Add(soundData.name, soundData);
        }

        //bgmDictionaryにセット
        foreach (var bgmData in bgmData)
        {
            bgmDictionary.Add(bgmData.name, bgmData);
        }
    }
    //未使用のAudioSourceの取得 全て使用中の場合はnullを返却
    private AudioSource GetUnusedAudioSource()
    {
        for (var i = 0; i < audioSourceList.Length; ++i)
        {
            if (audioSourceList[i].isPlaying == false) return audioSourceList[i];
        }

        return null; //未使用のAudioSourceは見つかりませんでした
    }

    //指定されたAudioClipを未使用のAudioSourceで再生
    public void Play(AudioClip clip, float pitch, float volume)
    {
        /*if (stageType == StageType.level)
        {
            if (GameManager.gameManager.applicationQuit || GameManager.gameManager.sceneUnloaded) return;
        }*/

        var audioSource = GetUnusedAudioSource();
        if (audioSource == null) return; //再生できませんでした
        audioSource.clip = clip;
        audioSource.pitch = pitch;
        audioSource.volume = volume * DataBase.MasterVolume * 2;
        audioSource.Play();
    }

    //指定された別名で登録されたAudioClipを再生
    public void Play(string name, float pitch, float volume)
    {
        if (soundDictionary.TryGetValue(name, out var soundData)) //管理用Dictionary から、別名で探索
        {
            if (Time.realtimeSinceStartup - soundData.playedTime < playableDistance) return;    //まだ再生するには早い
            soundData.playedTime = Time.realtimeSinceStartup;//次回用に今回の再生時間の保持
            Play(soundData.audioClip, pitch, volume); //見つかったら、再生
        }
        else
        {
            Debug.LogWarning($"その別名は登録されていません:{name}");
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

            if (bgmAudioSource.clip == BGMdata.audioClip) return; //すでにセットされていたらreturn

            bgmAudioSource.clip = BGMdata.audioClip;
            bgmAudioSource.Play();
        }
        else
        {
            Debug.LogWarning($"その別名は登録されていません:{name}");
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
