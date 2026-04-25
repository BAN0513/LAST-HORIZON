using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonControl : MonoBehaviour
{
    [SerializeField] private Transform parentObj;

    private Button button;
    private TitleUIManager t;

    public enum ButtonType
    {
        NewGame,
        Continue,
        System,
        Load_Slot,
        Loab_Back,
        System_Back,
        Characetr_Sword,
        Character_GreatSword,
        Character_Wizard,
        Character_SelectCheckNo,
        Character_SelectCheckYes
    }

    Dictionary<ButtonType, Action> buttonDict = new Dictionary<ButtonType, Action>();

    private void Start()
    {
        t = TitleUIManager.Instance;

        buttonDict[ButtonType.NewGame]                  = () => StartCoroutine(t.FadeInOutControl(t.groupCharacterSelect, t.groupStart, t.screenCharaSelect));
        buttonDict[ButtonType.Continue]                 = () => StartCoroutine(t.FadeInOutControl(t.groupLoad, t.groupStart, t.screenLoad));
        buttonDict[ButtonType.System]                   = () => StartCoroutine(t.FadeInOutControl(t.groupSystem, t.groupStart, t.screenSystem));
        buttonDict[ButtonType.Loab_Back]                = () => StartCoroutine(t.FadeInOutControl(t.groupStart, t.groupLoad, t.screenStart));
        buttonDict[ButtonType.System_Back]              = () => StartCoroutine(t.FadeInOutControl(t.groupStart, t.groupSystem, t.screenStart));
        buttonDict[ButtonType.Characetr_Sword]          = () => t.CharacterSelect(ButtonType.Characetr_Sword);
        buttonDict[ButtonType.Character_GreatSword]     = () => t.CharacterSelect(ButtonType.Character_GreatSword);
        buttonDict[ButtonType.Character_Wizard]         = () => t.CharacterSelect(ButtonType.Character_Wizard);
        buttonDict[ButtonType.Character_SelectCheckNo]  = () => StartCoroutine(t.FadeInOutControl(null, t.groupCharacterSelectCheck, t.screenCharaSelect, t.groupCharacterSelect));
        buttonDict[ButtonType.Character_SelectCheckYes] = () => t.CharacterSelectCheck_YES();

        foreach (var type in parentObj.GetComponentsInChildren<ButtonTypeSet>())
        {
            var id = type.type;
            int index = type.index;

            type.GetComponent<Button>().onClick.AddListener(() => PlayButton(id, index));
        }
    }

    public void PlayButton(ButtonType type, int index)
    {
        if (type == ButtonType.Load_Slot)
        {
            t.Slot(index);
            return;
        }

        if (buttonDict.TryGetValue(type, out Action value))
        {
            value.Invoke();
        }
    }
}
