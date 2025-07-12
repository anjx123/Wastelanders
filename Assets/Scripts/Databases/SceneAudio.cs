using UnityEngine;


[CreateAssetMenu(fileName = "Scene Audio Database", menuName = "Scene Audio Database")]
public class SceneAudio : ScriptableObject
{
#nullable enable
    public AudioClip? backgroundMusicPrimary;
    public AudioClip? backgroundMusicIntro;
    public AudioClip? combatMusicPrimary;
    public AudioClip? combatMusicIntro;
    public AudioClip? backgroundMusicDeath;
}
