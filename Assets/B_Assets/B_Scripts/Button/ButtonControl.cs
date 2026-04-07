using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonControl : MonoBehaviour
{
    [SerializeField] private Transform parentObj;

    private Button button;

    public enum ButtonType
    {
        NewGame,
        Continue,
        System,
        Load_Slot,
        Loab_Back,
        System_Back
    }

    Dictionary<ButtonType, Action> buttonDict = new Dictionary<ButtonType, Action>();

    private void Start()
    {
        buttonDict[ButtonType.NewGame] = () => TitleUIManager.Instance.NewGame();
        buttonDict[ButtonType.Continue] = () => TitleUIManager.Instance.Continue();
        buttonDict[ButtonType.System] = () => TitleUIManager.Instance.System();
        buttonDict[ButtonType.Loab_Back] = () => TitleUIManager.Instance.LoadBack();
        buttonDict[ButtonType.System_Back] = () => TitleUIManager.Instance.SystemBack();

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
            TitleUIManager.Instance.Slot(index);
            return;
        }

        if (buttonDict.TryGetValue(type, out Action value))
        {
            value.Invoke();
        }
    }
}
