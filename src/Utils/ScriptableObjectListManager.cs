using System;
using System.Collections.Generic;
using UnityEngine;

namespace QMarketPlugin.Utils;

/// <summary>
/// Этот класс позволяет упростить процесс изменения существующих и создания новых <see cref="ScriptableObject"/>'ов.
/// <list type="table">
/// <listheader>Шаблон использования:</listheader>
/// <item>Создать экземпляр <see cref="ScriptableObjectListManager{T,TKey}"/></item>
/// <item>Заполнить список скриптовых объектов, используя метод <see cref="AddInfo"/></item>
/// <item>
/// Завершить настройку этого менеджера, вызвав метод <see cref="Setup"/>.
/// <br />Этот шаг <b>изменяет</b> уже существующие в игре скриптовые объекты.
/// </item>
/// <item>
/// В каждом месте, где игра использует <see cref="List{T}"/> скриптовых объектов <typeparamref name="T"/>,
/// нужно добавить вызов <see cref="PatchList"/>, который приведет список в соответствующий вид.
/// </item>
/// </list>
/// </summary>
/// <typeparam name="T">Тип скриптового объекта.</typeparam>
/// <typeparam name="TKey">Тип поля в этом объекте, которое используется в качестве идентификатора.</typeparam>
public class ScriptableObjectListManager<T, TKey> where T : ScriptableObject {

    private readonly IDictionary<TKey, ScriptableObjectInfo<T>> infos = new Dictionary<TKey, ScriptableObjectInfo<T>>();

    private readonly Func<T, TKey> idGetter;

    private readonly Action<T, TKey> idSetter;

    public int Count => infos.Count;

    public ICollection<TKey> Keys => infos.Keys;

    private bool isSetupDone;

    /// <param name="idGetter">Функция для извлечения идентифицирующего поля из <typeparamref name="T"/>.</param>
    /// <param name="idSetter">Действие для установки идентифицирующего поля в <typeparamref name="T"/>.</param>
    public ScriptableObjectListManager(Func<T, TKey> idGetter, Action<T, TKey> idSetter) {
        this.idGetter = idGetter;
        this.idSetter = idSetter;
    }

    /// <summary>
    /// Добавляет информацию о <typeparamref name="T"/> с указанным идентификатором.
    /// <br />Упрощенный вариант метода, который самостоятельно создает экземпляр <see cref="ScriptableObjectInfo{T}"/>.
    /// </summary>
    /// <param name="id">Уникальный идентификатор объекта <typeparamref name="T"/>.</param>
    /// <param name="filler">Филлер, который наполнит <typeparamref name="T"/> новыми данными.</param>
    /// <exception cref="InvalidOperationException">
    /// Выбрасывается, если информация добавляется после завершения настройки.
    /// </exception>
    public void AddInfo(TKey id, Action<T> filler) {
        AddInfo(id, new ScriptableObjectInfo<T>(filler));
    }

    /// <summary>
    /// Добавляет информацию о <typeparamref name="T"/> с указанным идентификатором.
    /// </summary>
    /// <param name="id">Уникальный идентификатор объекта <typeparamref name="T"/>.</param>
    /// <param name="info">Обертка с дополнительной информацией о <typeparamref name="T"/>.</param>
    /// <exception cref="InvalidOperationException">
    /// Выбрасывается, если информация добавляется после завершения настройки.
    /// </exception>
    public void AddInfo(TKey id, ScriptableObjectInfo<T> info) {
        if (isSetupDone) {
            throw new InvalidOperationException("Cannot add info after setup is done.");
        }

        infos[id] = info;
    }

    /// <summary>
    /// Завершает настройку и привязывает информацию к объектам.
    /// <br /><b>Вносит изменения</b> в уже существующие в игре объекты.
    /// </summary>
    /// <param name="vanillaObjects">Список уже существующих в игре объектов <typeparamref name="T"/>.</param>
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

    /// <summary>
    /// Обновляет список <paramref name="objects"/>, чтобы он соответствовал изменениям:
    /// <br />Все объекты, для которых не был добавлен <see cref="ScriptableObjectInfo{T}"/>, будут удалены.
    /// <br />Новые объекты, для которых добавлен <see cref="ScriptableObjectInfo{T}"/>, но которые отсутствуют
    /// в <paramref name="objects"/>, будут в него добавлены.
    /// </summary>
    /// <param name="objects">Список объектов <typeparamref name="T"/>, который требуется обновить.</param>
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

    /// <summary>
    /// Возвращает информацию о <typeparamref name="T"/> с указанным идентификатором.
    /// </summary>
    /// <typeparam name="TReturn">
    /// Тип, к которому нужно преобразовать возвращаемый <see cref="ScriptableObjectInfo{T}"/>.
    /// </typeparam>
    /// <param name="id">Уникальный идентификатор объекта <typeparamref name="T"/>.</param>
    /// <returns>Информация о <typeparamref name="T"/>.</returns>
    public TReturn GetInfo<TReturn>(TKey id) where TReturn : ScriptableObjectInfo<T> {
        return (TReturn) infos[id];
    }
}

/// <summary>
/// Класс-обертка, хранящий дополнительную информацию о скриптовом объекте <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">Тип скриптового объекта.</typeparam>
public class ScriptableObjectInfo<T> where T : ScriptableObject {

    /// <summary>
    /// Экземпляр скриптового объекта <typeparamref name="T"/>.
    /// </summary>
    public T Instance { get; private set; }

    private Action<T> Filler { get; }

    /// <param name="filler">Делегат, который заполнит <typeparamref name="T"/> новыми данными.</param>
    public ScriptableObjectInfo(Action<T> filler) {
        Filler = filler;
    }

    /// <summary>
    /// Присваивает этому <see cref="ScriptableObjectInfo{T}"/> указанный экземпляр <typeparamref name="T"/>
    /// и заполняет его новыми данными, используя переданный в конструктор филлер.
    /// </summary>
    /// <param name="instance">Экземпляр скриптового объекта <typeparamref name="T"/>.</param>
    public void AssignInstance(T instance) {
        Instance = instance;
        Filler(instance);
    }
}
