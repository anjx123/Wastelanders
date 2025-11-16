using System;
using UnityEngine;

namespace DialogueScripts
{
    [Serializable]
    public class DialogueEntryInUnityEditor
    {
        [SerializeField] [TextArea(1, 5)] private string content;
        [SerializeField] private string speaker;
        [SerializeField] private string sfxName;
        [SerializeField] private string eventId;
        [SerializeField] private Sprite picture;
        [SerializeField] private Layout verticalLayout;

        public DialogueEntry Into() => new(
            content: content,
            speaker: speaker,
            sfxName: sfxName,
            eventId: eventId,
            picture: picture,
            verticalLayout: verticalLayout
        );
    }

    public record DialogueEntry(
        string content,
        string speaker,
        string sfxName,
        string eventId,
        Sprite picture,
        Layout verticalLayout
    );

    [Serializable]
    public enum Layout
    {
        LOWER,
        UPPER
    }

    public static class DialogueExtensions
    {
        public static DialogueEntry[] Into(this DialogueEntryInUnityEditor[] entries)
        {
            return Array.ConvertAll(entries, e => e.Into());
        }
    }
}