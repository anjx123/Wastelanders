using System;
using System.Collections.Generic;
using UnityEngine;

namespace DialogueScripts
{
    [Serializable]
    public class DialogueEntryInUnityEditor
    {
        [SerializeField] [TextArea(1, 5)] private string content;
        [SerializeField] private string speaker;
        [SerializeField] private string sfxName;
        [SerializeField] private Sprite picture;
        [SerializeReference] internal List<DialogueEvents> events;

        public DialogueEntry Into() => new(
            content: content,
            speaker: speaker,
            sfxName: sfxName,
            picture: picture,
            events: events
        );
    }

    public record DialogueEntry(
        string content,
        string speaker,
        string sfxName,
        Sprite picture,
        List<DialogueEvents> events
        );

    public static class DialogueExtensions
    {
        public static DialogueEntry[] Into(this DialogueEntryInUnityEditor[] entries)
        {
            return Array.ConvertAll(entries, e => e.Into());
        }
    }
}