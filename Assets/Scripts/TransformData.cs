using System;
using UnityEngine;

/// <summary>
/// Тестовая сериализуемая структура для Transform.
/// </summary>
[Serializable]
public struct TransformData
{
    public Vector3 Position;
    public Quaternion Rotation;
    public Vector3 Scale;
}
