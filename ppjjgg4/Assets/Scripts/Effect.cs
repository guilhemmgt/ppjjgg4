using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Interactions;

[Serializable]
public class Effect
{
    public enum Flag { SnailProtection, EffectCancelled, Score1, Score5, Score10, Score20, Score40, Score50, Score100, Score250, ScoreM10, ScoreM5}
    public Flag flag;
    public ISkill owner;
    public string ownerName;
    public string ownerPlant;
    public string ownerPlot;
    private List<Plot> dependencies;

    public Effect(Flag flag, ISkill owner, List<Plot> dependencies)
    {
        this.flag = flag;
        this.owner = owner;
        this.dependencies = dependencies;
        ownerName = owner.Name;
        ownerPlant = owner.GetOwner().name;
        ownerPlot = owner.GetOwner().plot.name;
        foreach (Plot dep in dependencies)
        {
            dep.OnPlantExit += (_) => owner.RemoveEffect(this);
        }
    }

    public static int GetFlagScore(Flag flag)
    {
        return flag switch
        {
            Flag.Score1 => 1,
            Flag.Score5 => 5,
            Flag.Score10 => 10,
            Flag.Score20 => 20,
            Flag.Score40 => 40,
            Flag.Score50 => 50,
            Flag.Score100 => 100,
            Flag.Score250 => 250,
            Flag.ScoreM10 => -10,
            Flag.ScoreM5 => -5,
            _ => 0
        };
    }

}
