using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InputFieldControl : MonoBehaviour
{
    [SerializeField] private Transform parentObj;
    private TitleUIManager t;
    public enum InputFieldType
    {
        SE,
        BGM,
        Light
    }

    Dictionary<InputFieldType, Action> inputFieldDictDict = new Dictionary<InputFieldType, Action>();

    private void Start()
    {
        t = TitleUIManager.Instance;

        inputFieldDictDict[InputFieldType.SE]    = () => t.SliderMove(t.textSE, t.sliderSE);
        inputFieldDictDict[InputFieldType.BGM]   = () => t.SliderMove(t.textBGM, t.sliderBGM);
        inputFieldDictDict[InputFieldType.Light] = () => t.SliderMove(t.textLight, t.sliderLight);

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
