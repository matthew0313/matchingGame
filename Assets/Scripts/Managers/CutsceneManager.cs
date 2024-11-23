using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using UnityEngine;
using UnityEngine.UI;

public class CutsceneManager : MonoBehaviour
{
    public static CutsceneManager Instance { get; private set; }
    public CutsceneManager() => Instance = this;

    [SerializeField] Animator anim;
    [SerializeField] GameObject cutsceneObject;

    [Header("Talk")]
    [SerializeField] float talkProgressRate = 0.05f;
    [SerializeField][Tooltip("Used for . , ? !")] float talkPauseDuration = 0.25f;
    [SerializeField] Image showImageImage, leftTalkerImage, rightTalkerImage;
    [SerializeField] Color notTalkingColor, talkingColor;
    [SerializeField] Text talkerText;
    [SerializeField] Text dialogueText;

    public void PlayCutscene(Cutscene cutscene, Action onCutsceneFinish = null)
    {
        cutsceneObject.SetActive(true);
        if (cutscene.elements.Length == 0)
        {
            cutsceneObject.SetActive(false);
            onCutsceneFinish?.Invoke();
        }
        else StartCoroutine(CutsceneProgress(cutscene, 0, onCutsceneFinish));
    }
    IEnumerator CutsceneProgress(Cutscene cutscene, int progress, Action onCutsceneFinish)
    {
        CutsceneElement element = cutscene.elements[progress];
        bool progressing = true;
        if(element.type == CutsceneElementType.Talk)
        {
            StartCoroutine(Talk(element.talk, () => progressing = false));
        }
        else if(element.type == CutsceneElementType.MoveCamera)
        {
            MoveCamera(element.moveCamera, () => progressing = false);
        }
        else
        {
            progressing = false;
        }
        while (progressing) yield return null;
        if (progress == cutscene.elements.Length - 1)
        {
            cutsceneObject.SetActive(false);
            onCutsceneFinish?.Invoke();
        }
        else StartCoroutine(CutsceneProgress(cutscene, progress + 1, onCutsceneFinish));
    }
    readonly int talkID = Animator.StringToHash("Talk");
    readonly int talkingID = Animator.StringToHash("Talking");
    readonly int leftTalkerID = Animator.StringToHash("LeftTalker");
    readonly int rightTalkerID = Animator.StringToHash("RightTalker");
    IEnumerator Talk(CutsceneTalk content, Action onTalkFinish)
    {
        if (content.defaultLeftTalker != null)
        {
            anim.SetBool(leftTalkerID, true);
            leftTalkerImage.sprite = content.defaultLeftTalker;
        }
        else
        {
            anim.SetBool(leftTalkerID, false);
        }
        if (content.defaultRightTalker != null)
        {
            anim.SetBool(rightTalkerID, true);
            rightTalkerImage.sprite = content.defaultRightTalker;
        }
        else
        {
            anim.SetBool(rightTalkerID, false);
        }
        leftTalkerImage.color = notTalkingColor;
        rightTalkerImage.color = notTalkingColor;

        anim.SetBool(talkID, true);
        while (anim.GetBool(talkingID) == false) yield return null;

        for(int i = 0; i < content.elements.Length; i++)
        {
            CutsceneTalkElement element = content.elements[i];
            if (element.leftTalkerImage != null)
            {
                anim.SetBool(leftTalkerID, true);
                leftTalkerImage.sprite = element.leftTalkerImage;
            }
            else
            {
                anim.SetBool(leftTalkerID, false);
            }
            if (element.rightTalkerImage != null)
            {
                anim.SetBool(rightTalkerID, true);
                rightTalkerImage.sprite = element.rightTalkerImage;
            }
            else
            {
                anim.SetBool(rightTalkerID, false);
            }
            if((element.talkerLocation & CutsceneTalkerLocation.Left) > 0)
            {
                leftTalkerImage.color = talkingColor;
            }
            else
            {
                leftTalkerImage.color = notTalkingColor;
            }
            if((element.talkerLocation & CutsceneTalkerLocation.Right) > 0)
            {
                rightTalkerImage.color = talkingColor;
            }
            else
            {
                rightTalkerImage.color = notTalkingColor;
            }
            Language lang = GlobalManager.Instance.save.settings.language;
            talkerText.text = element.talker.Content(lang);
            dialogueText.text = "";
            IEnumerator talkProgress = TalkProgress(element, () => talkProgress = null, lang);
            StartCoroutine(talkProgress);
            while(talkProgress != null)
            {
                if (InputManager.IsTouchDown())
                {
                    StopCoroutine(talkProgress);
                    dialogueText.text = element.FilteredDialogue(lang);
                    break;
                }
                yield return null;
            }
            yield return null;
            while (!InputManager.IsTouchDown()) yield return null;
            yield return null;
        }
        anim.SetBool(talkID, false);
        while (anim.GetBool(talkingID) == true) yield return null;
        onTalkFinish?.Invoke();
    }
    IEnumerator TalkProgress(CutsceneTalkElement content, Action onTalkProgressFinish, Language language)
    {
        string dialogue = content.dialogue.Content(language);
        for(int i = 0; i < dialogue.Length; i++)
        {
            if (dialogue[i] == '[')
            {
                int number = 0;
                while (dialogue[++i] != ']')
                {
                    number *= 10;
                    number += (dialogue[i] - '0');
                }
                TalkEvent(content.events[number]);
                continue;
            }
            dialogueText.text += dialogue[i];
            if (dialogue[i] == '.' || dialogue[i] == ',' || dialogue[i] == '?' || dialogue[i] == '!')
            {
                yield return new WaitForSeconds(talkPauseDuration);
            }
            else
            {
                yield return new WaitForSeconds(talkProgressRate);
            }
        }
        onTalkProgressFinish?.Invoke();
    }
    readonly int showImageID = Animator.StringToHash("ShowImage");
    readonly int showingImageID = Animator.StringToHash("ShowingImage");
    readonly int stopShowingImageID = Animator.StringToHash("StopShowingImage");
    void TalkEvent(CutsceneTalkEvent content)
    { 
        if(content.type == CutsceneTalkEventType.ShowImage)
        {
            showImageImage.sprite = content.showImage.image;
            anim.SetTrigger(showImageID);
        }
        else if(content.type == CutsceneTalkEventType.StopShowingImage)
        {
            if (anim.GetBool(showingImageID))
            {
                anim.SetTrigger(stopShowingImageID);
            }
        }
        else if(content.type == CutsceneTalkEventType.MoveCamera)
        {
            MoveCamera(content.moveCamera);
        }
    }
    IEnumerator movingCamera = null;
    void MoveCamera(CutsceneMoveCamera content, Action onMoveFinish = null)
    {
        if (movingCamera != null) StopCoroutine(movingCamera);
        movingCamera = MovingCamera(Camera.main.transform.position, content, onMoveFinish);
        StartCoroutine(movingCamera);
    }
    IEnumerator MovingCamera(Vector2 startPos, CutsceneMoveCamera content, Action onMoveFinish)
    {
        float counter = 0.0f;
        float moveTime = content.moveCurve.keys[content.moveCurve.length - 1].time;
        while(counter < moveTime)
        {
            Vector2 origin = Camera.main.transform.position;
            Vector2 target = Vector2.Lerp(startPos, content.destination == null ? content.destination.position : content.destinationAlt, content.moveCurve.Evaluate(counter));
            Camera.main.transform.position = new Vector3(content.controlX ? target.x : origin.x, content.controlY ? target.y : origin.y, -10.0f);
            Camera.main.transform.position += new Vector3(0, 0, -10.0f);
            counter += Time.deltaTime;
            yield return null;
        }
        onMoveFinish?.Invoke();
    }
}
[System.Serializable]
public struct Cutscene
{
    [SerializeField] CutsceneElement[] m_elements;
    public CutsceneElement[] elements => m_elements;
}
[System.Serializable]
public struct CutsceneElement
{
    [SerializeField] CutsceneElementType m_type;
    [SerializeField] CutsceneTalk m_talk;
    [SerializeField] CutsceneMoveCamera m_moveCamera;
    public CutsceneElementType type => m_type;
    public CutsceneTalk talk => m_talk;
    public CutsceneMoveCamera moveCamera => m_moveCamera;
}
[System.Serializable]
public enum CutsceneElementType
{
    Talk,
    MoveCamera
}
[System.Serializable]
public struct CutsceneTalk
{
    [SerializeField] CutsceneTalkElement[] m_elements;
    [SerializeField] Sprite m_defaultLeftTalker, m_defaultRightTalker;
    public CutsceneTalkElement[] elements => m_elements;
    public Sprite defaultLeftTalker => m_defaultLeftTalker;
    public Sprite defaultRightTalker => m_defaultRightTalker;
}
[System.Serializable]
public struct CutsceneTalkElement
{
    [SerializeField] LanguageString m_talker;
    [SerializeField] Sprite m_leftTalkerImage, m_rightTalkerImage;
    [SerializeField] CutsceneTalkerLocation m_talkerLocation;
    [SerializeField] LanguageString m_dialogue;
    [SerializeField][Tooltip("Use [0]. [1] to play events during the talk.")] CutsceneTalkEvent[] m_events;
    public LanguageString talker => m_talker;
    public Sprite leftTalkerImage => m_leftTalkerImage;
    public Sprite rightTalkerImage => m_rightTalkerImage;
    public CutsceneTalkerLocation talkerLocation => m_talkerLocation;
    public LanguageString dialogue => m_dialogue;
    public string FilteredDialogue(Language language)
    {
        string result = "";
        string dialogue = this.dialogue.Content(language);
        for(int i = 0; i < dialogue.Length; i++)
        {
            if (dialogue[i] == '[')
            {
                while (dialogue[++i] != ']') { }
                continue;
            }
            result += dialogue[i];
        }
        return result;
    }
    public CutsceneTalkEvent[] events => m_events;
}
[System.Serializable]
[Flags]
public enum CutsceneTalkerLocation
{
    Left = 1<<0,
    Right = 1<<1
}
[System.Serializable]
public struct CutsceneTalkEvent
{
    [SerializeField] CutsceneTalkEventType m_type;
    [SerializeField] CutsceneTalkEvent_ShowImage m_showImage;
    [SerializeField] CutsceneMoveCamera m_moveCamera;
    public CutsceneTalkEventType type => m_type;
    public CutsceneTalkEvent_ShowImage showImage => m_showImage;
    public CutsceneMoveCamera moveCamera => m_moveCamera;
}
[System.Serializable]
public enum CutsceneTalkEventType
{
    ShowImage,
    StopShowingImage,
    MoveCamera
}
[System.Serializable]
public struct CutsceneTalkEvent_ShowImage
{
    [SerializeField] Sprite m_image;
    public Sprite image => m_image;
}
[System.Serializable]
public struct CutsceneMoveCamera
{
    [SerializeField] Transform m_destination;
    [SerializeField] Vector2 m_destinationAlt;
    [SerializeField] bool m_controlX, m_controlY;
    [SerializeField] AnimationCurve m_moveCurve;
    public Transform destination => m_destination;
    public Vector2 destinationAlt => m_destinationAlt;
    public bool controlX => m_controlX;
    public bool controlY => m_controlY;
    public AnimationCurve moveCurve => m_moveCurve;
}