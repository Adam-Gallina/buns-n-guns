using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ToneTagTypes { neg, pos, neu, none };
public class Textbox : MonoBehaviour
{
    public static Textbox instance;

    [Header("Text box")]
    public float defaultTextSpeed = 0.05f;

    [SerializeField] private GameObject textBox;
    [SerializeField] private Transform nameText;
    [SerializeField] private Text mainText;
    [SerializeField] private Transform tagText;

    private void Awake()
    {
        instance = this;
        
        HideBox();
    }

    public void ShowBox()
    {
        textBox.SetActive(true);
    }

    public void HideBox()
    {
        textBox.SetActive(false);
    }


    public Coroutine SetText(string name, string text, ToneTagTypes tag)
    {
        return SetText(name, text, tag, true, defaultTextSpeed);
    }
    public Coroutine SetText(string name, string text, ToneTagTypes tag, float textSpeed)
    {
        return SetText(name, text, tag, true, textSpeed);
    }
    public Coroutine SetText(string name, string text, ToneTagTypes tag, bool promptToClose, float textSpeed = -1)
    {
        if (textSpeed == -1)
            textSpeed = defaultTextSpeed;

        return StartCoroutine(AddText(name, text, tag, promptToClose, textSpeed));
    }

    private IEnumerator AddText(string name, string text, ToneTagTypes tag, bool promptToClose, float textSpeed)
    {
        ShowBox();
        MyDebug.SetFancyText(nameText, name);

        tagText.gameObject.SetActive(SaveData.instance.toneTags == ToneTags.Enabled);
        MyDebug.SetFancyText(tagText, "/" + tag.ToString());

        string currText = "";
        float nextCharTime = 0;
        //skipText.text = "Skip";

        while (currText != text)
        {
            if (LevelController.instance.paused)
                yield return null;

            if (Time.time >= nextCharTime)
            {
                nextCharTime = Time.time + textSpeed;
                currText += text.Substring(currText.Length, 1);
                mainText.text = currText;
            }

            //if (skipButton.Pressed())
            if (Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.Space))
            {
                promptToClose = true;
                currText = text;
                mainText.text = currText;
                yield return new WaitForEndOfFrame();
            }

            yield return null;
        }

        //skipText.text = "Next";

        if (promptToClose)
        {
            //yield return new WaitUntil(() => skipButton.Pressed());
            yield return new WaitUntil(() => Input.anyKeyDown);
            yield return new WaitForEndOfFrame();
        }
        else
        {
            yield return new WaitForSeconds(0.75f);
        }
    }
}
