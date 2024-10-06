﻿using Game;
using System;

[Serializable]
public class StatisticDynamic<Type> : StatisticSerializedIdentity
{
    protected Type currentValue;

    public StatisticDynamic()
    {

    }

    public StatisticDynamic(string name = null, string definition = null)
        : base(name, definition)
    {

    }

    public virtual void Modify(Type value)
    {
        currentValue = value;
    }

    public override bool TryGetValue<T>(out T value)
    {
        value = StatisticUtility.ConvertGeneric<T, Type>(currentValue);
        return true;
    }
}
