using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InputFieldControl : MonoBehaviour
{
    [SerializeField] private Transform parentObj;

    public enum InputFieldType
    {
        SE,
        BGM,
        Light
    }

    Dictionary<InputFieldType, Action> inputFieldDictDict = new Dictionary<InputFieldType, Action>();

    private void Start()
    {
        inputFieldDictDict[InputFieldType.SE] = () => TitleUIManager.Instance.SETextChange();
        inputFieldDictDict[InputFieldType.BGM] = () => TitleUIManager.Instance.BGMTextChange();
        inputFieldDictDict[InputFieldType.Light] = () => TitleUIManager.Instance.LightTextChange();

        foreach (var type in parentObj.GetComponentsInChildren<InputFieldTypeSet>())
        {
            var id = type.type;

            type.GetComponent<TMP_InputField>().onEndEdit.AddListener((string s) => InputFieldChange(id));
        }
    }

    public void InputFieldChange(InputFieldType type)
    {
        if (inputFieldDictDict.TryGetValue(type, out Action value))
        {
            value.Invoke();
        }
    }
}
