using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;

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
    public UnityEvent OnSaveOperationBegan;
    
    /// <summary>
    /// Вызывается при успешном завершении операции сохранения.
    /// </summary>
    public UnityEvent OnSaveOperationCompleted;

    /// <summary>
    /// Вызывается при непредвиденном прерывании операции сохранения.
    /// </summary>
    public UnityEvent OnSaveOperationInterrupted;
    
    /// <summary>
    /// Вызывается в начале операции загрузки.
    /// </summary>
    public UnityEvent OnLoadOperationBegan;
    
    /// <summary>
    /// Вызывается при успешном завершении операции загрузки.
    /// </summary>
    public UnityEvent OnLoadOperationCompleted;

    /// <summary>
    /// Вызывается при непредвиденном прерывании операции загрузки.
    /// </summary>
    public UnityEvent OnLoadOperationInterrupted;

    /// <summary>
    /// Сохраняет данные объектов.
    /// </summary>
    public void Save()
    {
        OnSaveOperationBegan?.Invoke();

        var objectsToSave = FindObjectsOfType<SavableBase>();
        var json = string.Empty;

        foreach (var savable in objectsToSave)
        {
            json += $"{savable.Save()}\n";
        }
        json += END_OF_FILE_LABEL;

        new Thread(() =>
        {
            try
            {
                File.WriteAllText(_mainSaveFilePath, json);
                OnSaveOperationCompleted?.Invoke();
            }
            catch
            {
                OnSaveOperationInterrupted?.Invoke();
            }        
        }).Start();
        
        new Thread(() =>
        {
            File.WriteAllText(_saveCopyFilePath, json);
        }).Start();
    }

    /// <summary>
    /// Загружает данные объектов.
    /// </summary>
    public void Load() => StartCoroutine(LoadAsync());

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

    /// <summary>
    /// Реализует асинхронную загрузку.
    /// </summary>
    private IEnumerator LoadAsync()
    {
        var isDataLoaded = false;
        
        OnLoadOperationBegan?.Invoke();

        var json = new List<string>();

        try
        {
            new Thread(() =>
            {
                json = File.ReadAllLines(_mainSaveFilePath).ToList();

                if (!json.Contains(END_OF_FILE_LABEL))
                {
                    json = File.ReadAllLines(_saveCopyFilePath).ToList();
                }

                isDataLoaded = true;
            }).Start();
        }
        catch
        {
            OnLoadOperationInterrupted?.Invoke();
        }
        
        var objectsToLoad = FindObjectsOfType<SavableBase>();

        while (!isDataLoaded)
        {
            yield return null;
        }
        
        foreach (var savable in objectsToLoad)
        {
            var objectData = json.First(s => s.Contains(savable.Id));
            json.Remove(objectData);
            savable.Load(objectData);
        }
            
        OnLoadOperationCompleted?.Invoke();
    }
}
