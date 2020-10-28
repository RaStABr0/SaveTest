using System;
using System.IO;
using System.Linq;
using UnityEngine;

//TODO: решить, как имплементировать в основной проект.
/// <summary>
/// Система сохранения.
/// </summary>
public class SaveSystem : MonoBehaviour
{
    /// <summary>
    /// Метка, означающая окончание файла сохранения и, следственно, его успешную запись.
    /// </summary>
    private const string END_OF_FILE_LABEL = "EOF";
    
    /// <summary>
    /// Имя директории с файлами сохранения.
    /// </summary>
    private const string SAVE_DIRECTORY_NAME = "SaveData";
    
    /// <summary>
    /// Имя основного файла сохранения.
    /// </summary>
    private const string MAIN_SAVE_NAME = "Save1";
    
    /// <summary>
    /// Имя копии файла сохранения.
    /// </summary>
    private const string SAVE_COPY_NAME = "SaveCopy1";
    
    /// <summary>
    /// Путь к основному файлу сохранения.
    /// </summary>
    private string _mainSaveFilePath;
    
    /// <summary>
    /// Путь к резервному файлу сохранения.
    /// </summary>
    private string _saveCopyFilePath;
    
    /// <summary>
    /// Вызывается в начале операции сохранения.
    /// </summary>
    public EventHandler OnSaveOperationBegan;
    
    /// <summary>
    /// Вызывается при успешном завершении операции сохранения.
    /// </summary>
    public EventHandler OnSaveOperationCompleted;

    /// <summary>
    /// Вызывается при непредвиденном прерывании операции сохранения.
    /// </summary>
    public EventHandler OnSaveOperationInterrupted;
    
    /// <summary>
    /// Вызывается в начале операции загрузки.
    /// </summary>
    public EventHandler OnLoadOperationBegan;
    
    /// <summary>
    /// Вызывается при успешном завершении операции загрузки.
    /// </summary>
    public EventHandler OnLoadOperationCompleted;

    /// <summary>
    /// Вызывается при непредвиденном прерывании операции загрузки.
    /// </summary>
    public EventHandler OnLoadOperationInterrupted;
    
    /// <summary>
    /// Сохраняет данные объектов.
    /// </summary>
    public void Save()
    {
        OnSaveOperationBegan?.Invoke(this, EventArgs.Empty);
        
        try
        {
            var objectsToSave = FindObjectsOfType<SavableBase>();
            var json = string.Empty;
            
            foreach (var savable in objectsToSave)
            {
                json += $"{savable.Save()}\n";
            }

            json += END_OF_FILE_LABEL;
            
            File.WriteAllText(_mainSaveFilePath, json);

            OnSaveOperationCompleted?.Invoke(this, EventArgs.Empty);

            File.WriteAllText(_saveCopyFilePath, json);

        }
        catch (Exception e)
        {
            OnSaveOperationInterrupted?.Invoke(this, EventArgs.Empty);
        }
    }

    /// <summary>
    /// Загружает данные объектов.
    /// </summary>
    public void Load()
    {
        OnLoadOperationBegan?.Invoke(this, EventArgs.Empty);
        
        try
        {
            var json = File.ReadAllLines(_mainSaveFilePath).ToList();

            if (!json.Contains(END_OF_FILE_LABEL))
            {
                json = File.ReadAllLines(_saveCopyFilePath).ToList();
            }
               
            var objectsToLoad = FindObjectsOfType<SavableBase>();

            foreach (var savable in objectsToLoad)
            {
                var objectData = json.First(s => s.Contains(savable.Id));
                json.Remove(objectData);
                savable.Load(objectData);
            }
            
            OnLoadOperationCompleted?.Invoke(this, EventArgs.Empty);

        }
        catch (Exception e)
        {
            OnLoadOperationInterrupted?.Invoke(this, EventArgs.Empty);
        }
    }

    private void Awake() => InitSaveDataPath();

    //TODO: выпилить
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            Save();
        }
        
        if (Input.GetKeyDown(KeyCode.L))
        {
            Load();
        }
    }

    /// <summary>
    /// Инициализирует пути к файлам сохранения.
    /// </summary>
    private void InitSaveDataPath()
    {
        var saveDirectoryPath = Path.Combine(Application.persistentDataPath, SAVE_DIRECTORY_NAME);

        if (!Directory.Exists(saveDirectoryPath))
        {
            Directory.CreateDirectory(saveDirectoryPath);
        }

        _mainSaveFilePath = Path.Combine(saveDirectoryPath, MAIN_SAVE_NAME);
        _saveCopyFilePath = Path.Combine(saveDirectoryPath, SAVE_COPY_NAME);
    }
}
