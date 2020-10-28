using System;
using UnityEngine;

/// <summary>
/// Тестовый сохраняемый компонент
/// </summary>
public class Savable : SavableBase
{
    /// <summary>
    /// Сериализует необходимые данные в json.
    /// </summary>
    /// <returns>Json-строка.</returns>
    public override string Save()
    {
        var savableData = new SavableData
        {
            Id = Id,
            Transform = new TransformData
            {
                Position = transform.position,
                Rotation = transform.rotation,
                Scale = transform.localScale
            }
        };

        var json = JsonUtility.ToJson(savableData);

        return json;
    }

    /// <summary>
    /// Десериализует и присваивает значения из json-строки.
    /// </summary>
    /// <param name="json">Json-строка.</param>
    public override void Load(string json)
    {
        var data = JsonUtility.FromJson<SavableData>(json);
        var transformData = data.Transform;
        
        transform.position = transformData.Position;
        transform.rotation = transformData.Rotation;
        transform.localScale = transformData.Scale;
    }

    /// <summary>
    /// Данные, которые необходимо сохранить.
    /// </summary>
    [Serializable]
    private class SavableData : SavableDataBase 
    {
        public TransformData Transform;
    }
}