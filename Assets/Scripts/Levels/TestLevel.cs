using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestLevel : LevelController
{
    [SerializeField] protected RoomBase room1;
    [SerializeField] protected RoomBase room2;

    protected override IEnumerator PlayStory()
    {
        yield return StartCoroutine(StoryControls.DisplayText(new StoryControls.TextboxData[]
        {
            StoryControls.GenTextboxData("Billy Buttcheeks", "Wow! I'm a textbox now!", ToneTagTypes.pos),
            StoryControls.GenTextboxData("Billy Buttcheeks", "Guess it's time for you to walk for me! WASD to move :)", ToneTagTypes.neu),
        }));

        /*yield return StartCoroutine(StoryControls.WaitForPlayerEntry(room1));
        yield return StartCoroutine(StoryControls.DisplayText(new StoryControls.TextboxData[]
        {
            StoryControls.GenTextboxData("Billy Buttcheeks", "Look at you go! You already made it to room 2!!", ToneTag.pos),
            StoryControls.GenTextboxData("Billy Buttcheeks", "You definitely won't get past this room... >:)", ToneTag.neg)
        }));*/

        yield return StartCoroutine(StoryControls.WaitForPlayerEntry(room2));
        yield return StartCoroutine(StoryControls.DisplayText(new StoryControls.TextboxData[]
        {
            StoryControls.GenTextboxData("Billy Buttcheeks", "And here it is! The third room!", ToneTagTypes.pos),
        }));
    }
}
