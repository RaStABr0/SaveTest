using System;
using UnityEngine;

/// <summary>
/// Родитель для компонентов всех сохраняемых объектов.
/// </summary>
public abstract class SavableBase : MonoBehaviour
{
    /// <summary>
    /// Идентификатор сохраняемого компонента.
    /// </summary>
    [SerializeField] private string _id;

    /// <summary>
    /// Идентификатор сохраняемого компонента.
    /// </summary>
    public string Id => _id;

    /// <summary>
    /// Сохранить данные.
    /// </summary>
    /// <returns>Json-строка.</returns>
    public abstract string Save();

    /// <summary>
    /// Загрузить данные.
    /// </summary>
    /// <param name="json">Json-строка.</param>
    public abstract void Load(string json);

    private void OnValidate()
    {
        if (_id == string.Empty)
        {
            _id = Guid.NewGuid().ToString();
        }
    }
}