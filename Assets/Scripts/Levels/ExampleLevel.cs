using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExampleLevel : LevelController
{
    protected override IEnumerator PlayStory()
    {
        yield return StartCoroutine(StoryControls.DisplayText(new StoryControls.TextboxData[]
        {
            StoryControls.GenTextboxData("Billy Buttcheeks", "Using Example LevelController...", ToneTagTypes.neg)
        }));
    }
}
