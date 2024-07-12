using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
/*
    [Header("#BGM")]
    public AudioClip bgmClip;
    public float bgmVolume;
    AudioSource bgmPlayer;
*/
    [Header("#SFX")] //ȿ����
    public AudioClip[] sfxClips;
    public AudioClip[] atkClips;
    public float sfxVolume;
    public int channels;
    public int chs;
    AudioSource[] sfxPlayers;
    AudioSource[] attack;
    int channelIndex;
    int ch;

    public enum Sfx { Attack, KiHap, Shot, Windmill}
    public enum Atk { Attack}

    void Awake()
    {
        instance = this;
        Init();
    }

    void Init()
    {/*
        //����� �÷��̾� �ʱ�ȭ
        GameObject bgmObject = new GameObject("BgmPlayer");
        bgmObject.transform.parent = transform;
        bgmPlayer = bgmObject.AddComponent<AudioSource>();
        bgmPlayer.playOnAwake = false;
        bgmPlayer.loop = true;
        bgmPlayer.volume = bgmVolume;
        bgmPlayer.clip = bgmClip;
*/
        //ȿ���� �ʱ�ȭ
        GameObject sfxObject = new GameObject("SfxPlayer");
        sfxObject.transform.parent = transform;
        sfxPlayers = new AudioSource[channels];


        for (int index = 0; index < sfxPlayers.Length; index++)
        {
            sfxPlayers[index] = sfxObject.AddComponent<AudioSource>();
            sfxPlayers[index].playOnAwake = false;
            sfxPlayers[index].volume = sfxVolume;
        }
        ///////////////////////////////////
        ///
        GameObject AttackObject = new GameObject("AttackPlayer");
        AttackObject.transform.parent = transform;
        
        attack = new AudioSource[chs];

        for (int index = 0; index < attack.Length; index++)
        {
            attack[index] = AttackObject.AddComponent<AudioSource>();
            attack[index].playOnAwake = false;
            attack[index].volume = sfxVolume;
        }
    }
/*
    public void PlayBgm(bool isPlay)
    {
        if(isPlay)
        {
            bgmPlayer.Play();
        }
        else
        {
            bgmPlayer.Stop();
        }
    }
*/
    public void PlaySfx(Sfx sfx)
    {
        for (int index = 0; index < sfxPlayers.Length; index++)
        {
            int loopIndex = (index + channelIndex) % sfxPlayers.Length;

            if (sfxPlayers[loopIndex].isPlaying)
            {
                continue;
            }
            channelIndex = loopIndex;
            sfxPlayers[loopIndex].clip = sfxClips[(int)sfx];
            sfxPlayers[loopIndex].Play();
            break;
        }
    }

    public void Attack(Atk atk, bool sound)
    {
        if (!sound)
        {
            for (int index = 0; index < attack.Length; index++)
            {
                int loopIndex = (index + ch) % attack.Length;

                if (attack[loopIndex].isPlaying)
                {
                    continue;
                }
                ch = loopIndex;
                attack[loopIndex].clip = atkClips[(int)atk];
                attack[loopIndex].Play();
                break;
            }
        }
    }
}
