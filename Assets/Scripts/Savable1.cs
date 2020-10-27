using System;
using UnityEngine;

public class Savable1 : SavableBase
{
    private struct Savable1Data
    {
        public TransformData Transform;
        
    }

    public override void Save()
    {
        throw new NotImplementedException();
    }

    public override void Load()
    {
        throw new NotImplementedException();
    }

    public override void SetData()
    {
        throw new NotImplementedException();
    }
}