using System;
using System.Collections.Generic;
using UnityEngine;

namespace QMarketPlugin.Utils;

public class ScriptableObjectListManager<T, TKey> where T : ScriptableObject {

    private readonly IDictionary<TKey, ScriptableObjectInfo<T>> infos = new Dictionary<TKey, ScriptableObjectInfo<T>>();

    private readonly Func<T, TKey> idGetter;

    private readonly Action<T, TKey> idSetter;

    public int Count => infos.Count;

    public ICollection<TKey> Keys => infos.Keys;

    private bool isSetupDone;

    public ScriptableObjectListManager(Func<T, TKey> idGetter, Action<T, TKey> idSetter) {
        this.idGetter = idGetter;
        this.idSetter = idSetter;
    }

    public void AddInfo(TKey id, Action<T> filler) {
        AddInfo(id, new ScriptableObjectInfo<T>(filler));
    }

    public void AddInfo(TKey id, ScriptableObjectInfo<T> info) {
        if (isSetupDone) {
            throw new InvalidOperationException("Cannot add info after setup is done.");
        }

        infos[id] = info;
    }

    public void Setup(List<T> vanillaObjects) {
        if (isSetupDone) {
            return;
        }

        foreach (var id in infos.Keys) {
            var scriptableObject = FindObject(vanillaObjects, id) ?? CreateExtraObject(id);

            infos[id].AssignInstance(scriptableObject);
        }

        isSetupDone = true;
    }

    private T CreateExtraObject(TKey id) {
        var scriptableObject = ScriptableObject.CreateInstance<T>();

        idSetter(scriptableObject, id);

        return scriptableObject;
    }

    public void PatchList(List<T> objects) {
        objects.RemoveAll(IsInstanceUnused);

        foreach (var id in infos.Keys) {
            EnsureObjectExists(objects, id);
        }
    }

    private bool IsInstanceUnused(T existingObject) {
        return !infos.ContainsKey(idGetter(existingObject));
    }

    private void EnsureObjectExists(List<T> objects, TKey id) {
        if (CanAddExtraObject(objects, id)) {
            AddExtraObject(objects, id);
        }
    }

    private bool CanAddExtraObject(List<T> objects, TKey id) {
        return !FindObject(objects, id)
               && infos.ContainsKey(id);
    }

    private void AddExtraObject(ICollection<T> objects, TKey id) {
        objects.Add(infos[id].Instance);
    }

    private T FindObject(List<T> objects, TKey id) {
        return objects.Find(scriptableObject => idGetter(scriptableObject).Equals(id));
    }

    public TReturn GetInfo<TReturn>(TKey id) where TReturn : ScriptableObjectInfo<T> {
        return (TReturn) infos[id];
    }
}

public class ScriptableObjectInfo<T> where T : ScriptableObject {

    public T Instance { get; private set; }

    private Action<T> Filler { get; }

    public ScriptableObjectInfo(Action<T> filler) {
        Filler = filler;
    }

    public void AssignInstance(T instance) {
        Instance = instance;
        Filler(instance);
    }
}
