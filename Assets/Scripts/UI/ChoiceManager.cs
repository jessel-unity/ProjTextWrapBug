using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChoiceManager<T>
{
    //public List<T> choices { get; private set; }
    public List<T> choices;
    public List<int> chosenIndices { get; private set; }

    public ChoiceManager(List<T> choices)
    {
        this.choices = choices;
        chosenIndices = new List<int>();
    }

    public ChoiceManager(List<T> choices, List<int> chosenIndices)
    {
        this.choices = choices;
        this.chosenIndices = chosenIndices;
    }

    public void AddChoice(int index, bool exclusive = false)
    {
        var isChosen = chosenIndices.IndexOf(index) != -1;

        if (index > -1 && !isChosen)
        {
            if (exclusive)
            {
                Clear();
            }
            chosenIndices.Add(index);
        }
    }

    public void AddChoice(T choice, bool exclusive = false)
    {
        var index = choices.IndexOf(choice);
        AddChoice(index, exclusive);
    }

    public void RemoveChoice(int index)
    {
        chosenIndices.Remove(index);
    }

    public void RemoveChoice(T choice)
    {
        var index = choices.IndexOf(choice);
        RemoveChoice(index);
    }

    public void Clear()
    {
        chosenIndices.Clear();
    }
}
