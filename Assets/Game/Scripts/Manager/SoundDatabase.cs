using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HAVIGAME;
using HAVIGAME.Audios;

[CreateAssetMenu(fileName = "SoundDatabase", menuName = "Database/SoundDatabase")]
public class SoundDatabase : Database<SoundDatabase>
{
    [SerializeField] private Audio correctAudio;
    [SerializeField] private Audio wrongAudio;
    [SerializeField] private Audio winAudio;

    [Header("[Default]")]
    [SerializeField] private Audio buttonClickAudio;

    public Audio CorrectAudio => correctAudio;
    public Audio WrongAudio => wrongAudio;
    public Audio WinAudio => winAudio;
    public Audio ButtonClickAudio => buttonClickAudio;
}
