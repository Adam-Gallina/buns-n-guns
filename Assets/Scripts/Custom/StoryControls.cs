using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StoryControls
{
    public static IEnumerator WaitForPlayerEntry(RoomBase targetRoom)
    {
        while (!targetRoom.PlayerInRoom())
            yield return null;
    }

    public static IEnumerator DisplayText(TextboxData[] text)
    {
        LevelController.instance.SetMovement(true);
        foreach (TextboxData data in text) {
            yield return Textbox.instance.SetText(data.name, data.text, data.tag, data.promptToClose, data.textSpeed);
        }
        Textbox.instance.HideBox();
        LevelController.instance.SetMovement(false);
    }

    public static TextboxData GenTextboxData(string name, string text, ToneTagTypes tag)
    {
        return GenTextboxData(name, text, tag, true, -1);
    }
    public static TextboxData GenTextboxData(string name, string text, ToneTagTypes tag, bool promptToClose, float textSpeed)
    {
        TextboxData data = new TextboxData();
        data.name = name;
        data.text = text;
        data.tag = tag;
        data.promptToClose = promptToClose;
        data.textSpeed = textSpeed;
        return data;
    }


    public struct TextboxData
    {
        public string name;
        public string text;
        public ToneTagTypes tag;
        public bool promptToClose;
        public float textSpeed;
    }
}
