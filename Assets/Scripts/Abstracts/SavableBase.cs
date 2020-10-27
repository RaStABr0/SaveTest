using System;
using UnityEngine;

public abstract class SavableBase : MonoBehaviour
{
    [SerializeField] private string _id;
    
    public string Id { get; private set; }

    public abstract void Save();
    
    public abstract void Load();
    
    public abstract void SetData();
    
    private void OnValidate()
    {
        if (Id == string.Empty)
        {
            Id = Guid.NewGuid().ToString();
        }
    }

    private abstract class SavableData
    {
        
    }
}
