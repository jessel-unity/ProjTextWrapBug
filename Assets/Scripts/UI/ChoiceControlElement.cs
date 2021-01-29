using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class ChoiceControlElement : VisualElement
{
    private ChoiceManager<VisualElement> choiceManager;
    private bool exclusive { get; set; } // This needs to be a property declaration to show its properly in UIBuilder https://forum.unity.com/threads/ui-builder-and-custom-elements.785129/#post-6713953

    public new class UxmlFactory : UxmlFactory<ChoiceControlElement, UxmlTraits> { }
    public new class UxmlTraits : VisualElement.UxmlTraits
    {
        UxmlBoolAttributeDescription m_Exclusive = new UxmlBoolAttributeDescription { name = "exclusive", defaultValue = false };

        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        {
            base.Init(ve, bag, cc);
            var cce = ve as ChoiceControlElement;
            cce.exclusive = m_Exclusive.GetValueFromBag(bag, cc);
        }
    }

    public ChoiceControlElement()
    {
        RegisterCallback<GeometryChangedEvent>(OnGeometryChange);
    }

    private void OnGeometryChange(GeometryChangedEvent evt)
    {
        var choices = new List<VisualElement>();
        this.Query<VisualElement>().Class("choice").ToList(choices);
        choiceManager = new ChoiceManager<VisualElement>(choices);

        for (int i = 0; i < choiceManager.choices.Count; i++)
        {
            int index = i;
            VisualElement element = choiceManager.choices[index];
            element.RegisterCallback((ClickEvent _evt) => { Choose(index); });
        }

        UnregisterCallback<GeometryChangedEvent>(OnGeometryChange);
    }

    public void Destroy()
    {
        for (int i = 0; i < choiceManager.choices.Count; i++)
        {
            int index = i;
            VisualElement element = choiceManager.choices[index];
            element.UnregisterCallback((ClickEvent _evt) => { Choose(index); });
        }
    }

    private void Select(VisualElement element)
    {
        element.AddToClassList("choice--selected");
        element.Query<Label>()
            .Class("choice__label")
            .ForEach((element) => { element.AddToClassList("choice__label--selected"); });
        element.Query<Label>()
            .Class("choice__secondary-label")
            .ForEach((element) => { element.AddToClassList("choice__secondary-label--selected"); });
        element.Query<VisualElement>()
            .Class("choice__icon")
            .ForEach((element) => { element.AddToClassList("choice__icon--selected"); });
    }

    private void Deselect(VisualElement element)
    {
        element.RemoveFromClassList("choice--selected");
        element.Query<Label>()
            .Class("choice__label")
            .ForEach((element) => { element.RemoveFromClassList("choice__label--selected"); });
        element.Query<Label>()
            .Class("choice__secondary-label")
            .ForEach((element) => { element.RemoveFromClassList("choice__secondary-label--selected"); });
        element.Query<VisualElement>()
            .Class("choice__icon")
            .ForEach((element) => { element.RemoveFromClassList("choice__icon--selected"); });
    }

    private void Choose(int index)
    {
        if (exclusive || !choiceManager.chosenIndices.Contains(index))
        {
            choiceManager.AddChoice(index, exclusive);
        }
        else
        {
            choiceManager.RemoveChoice(index);
        }
        
        UpdateUI();        
    }

    private void UpdateUI()
    {
        for (int i = 0; i < choiceManager.choices.Count; i++)
        {
            VisualElement element = choiceManager.choices[i];

            if (choiceManager.chosenIndices.Contains(i))
            {
                Select(element);
                continue;
            }

            Deselect(element);
        }
    }
}
