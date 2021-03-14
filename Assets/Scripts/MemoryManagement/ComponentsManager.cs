//#define BAD_PERF // TODO CHANGEZ MOI. Mettre en commentaire pour utiliser votre propre structure

using System;
using UnityEngine;

#if BAD_PERF
using InnerType = System.Collections.Generic.Dictionary<uint, IComponent>;
using AllComponents = System.Collections.Generic.Dictionary<uint, System.Collections.Generic.Dictionary<uint, IComponent>>;
#else
using InnerType = Array<IComponent>;
using AllComponents = System.Collections.Generic.Dictionary<uint, Array<IComponent>>;
#endif

// Appeler GetHashCode sur un Type est couteux. Cette classe sert a precalculer le hashcode
public static class TypeRegistry<T> where T : IComponent
{
    public static uint typeID = (uint)Mathf.Abs(default(T).GetRandomNumber()) % ComponentsManager.maxEntities;
}

public class Singleton<V> where V : new()
{
    private static bool isInitiated = false;
    private static V _instance;
    public static V Instance
    {
        get
        {
            if (!isInitiated)
            {
                isInitiated = true;
                _instance = new V();
            }
            return _instance;
        }
    }
    protected Singleton() { }
}

internal class ComponentsManager : Singleton<ComponentsManager>
{
    private AllComponents _allComponents = new AllComponents();

    private int entitiesCount = ECSManager.Instance.Config.numberOfShapesToSpawn;
    public const int maxEntities = 2000;

    public void DebugPrint()
    {
        string toPrint = "";
        var allComponents = Instance.DebugGetAllComponents();
        foreach (var type in allComponents)
        {
            toPrint += $"{type}: \n";
#if !BAD_PERF
            for(uint i = 0; i < type.Value.Length; i++)
#else
            foreach (var component in type.Value)
#endif
            {
#if BAD_PERF
                toPrint += $"\t{component.Key}: {component.Value}\n";
#else
                var component = type.Value[i];
                toPrint += $"\t{component}: {component}\n";
#endif
            }
            toPrint += "\n";
        }
        Debug.Log(toPrint);
    }

    // CRUD
    public void SetComponent<T>(EntityComponent entityID, IComponent component) where T : IComponent
    {
        if (!_allComponents.ContainsKey(TypeRegistry<T>.typeID))
        {
            _allComponents[TypeRegistry<T>.typeID] = new InnerType(entitiesCount);
        }
        _allComponents[TypeRegistry<T>.typeID].Set(entityID, component);   
    }
    public void RemoveComponent<T>(EntityComponent entityID) where T : IComponent
    {
        _allComponents[TypeRegistry<T>.typeID].Remove(entityID);
    }
    public T GetComponent<T>(EntityComponent entityID) where T : IComponent
    {
        return (T)_allComponents[TypeRegistry<T>.typeID].Get(entityID);
    }
    public bool TryGetComponent<T>(EntityComponent entityID, out T component) where T : IComponent
    {
        if (_allComponents.ContainsKey(TypeRegistry<T>.typeID))
        {
            if (_allComponents[TypeRegistry<T>.typeID].Contains(entityID))
            {
                component = (T)_allComponents[TypeRegistry<T>.typeID].Get(entityID);
                return true;
            }
        }
        component = default;
        return false;
    }

    public bool EntityContains<T>(EntityComponent entity) where T : IComponent
    {
        return _allComponents[TypeRegistry<T>.typeID].Contains(entity);
    }

    public void ClearComponents<T>() where T : IComponent
    {
        if (!_allComponents.ContainsKey(TypeRegistry<T>.typeID))
        {
            _allComponents.Add(TypeRegistry<T>.typeID, new InnerType(entitiesCount));
        }
        else
        {
           _allComponents[TypeRegistry<T>.typeID].Clear();
        }
    }

    public void ForEach<T1>(Action<EntityComponent, T1> lambda) where T1 : IComponent
    {
        var t1s = _allComponents[TypeRegistry<T1>.typeID];
        var entities = _allComponents[TypeRegistry<EntityComponent>.typeID];

        for(uint i = 0; i < t1s.Length; i++)
        {
            if (!t1s.Contains(i))
            {
                continue;
            }
            var entity = (EntityComponent) entities[i];
            lambda(entity, (T1) t1s[i]);
        }
    }

    public void ForEach<T1, T2>(Action<EntityComponent, T1, T2> lambda) where T1 : IComponent where T2 : IComponent
    {
        var t1s = _allComponents[TypeRegistry<T1>.typeID];
        var t2s = _allComponents[TypeRegistry<T2>.typeID];
        var entities = _allComponents[TypeRegistry<EntityComponent>.typeID];

        for(uint i = 0; i < t1s.Length; i++)
        {
            if (!t1s.Contains(i) || !t2s.Contains(i))
            {
                continue;
            }
            var entity = (EntityComponent) entities[i];
            lambda(entity, (T1) t1s[i], (T2) t2s[i]);
        }
    }

    public void ForEach<T1, T2, T3>(Action<EntityComponent, T1, T2, T3> lambda) where T1 : IComponent where T2 : IComponent where T3 : IComponent
    {
        var t1s = _allComponents[TypeRegistry<T1>.typeID];
        var t2s = _allComponents[TypeRegistry<T2>.typeID];
        var t3s = _allComponents[TypeRegistry<T3>.typeID];
        var entities = _allComponents[TypeRegistry<EntityComponent>.typeID];

        for(uint i = 0; i < t1s.Length; i++)
        {
            if (!t1s.Contains(i) || !t2s.Contains(i) || !t3s.Contains(i))
            {
                continue;
            }
            var entity = (EntityComponent) entities[i];
            lambda(entity, (T1) t1s[i], (T2) t2s[i], (T3) t3s[i]);
        }
    }

    public void ForEach<T1, T2, T3, T4>(Action<EntityComponent, T1, T2, T3, T4> lambda) where T1 : IComponent where T2 : IComponent where T3 : IComponent where T4 : IComponent
    {
        var t1s = _allComponents[TypeRegistry<T1>.typeID];
        var t2s = _allComponents[TypeRegistry<T2>.typeID];
        var t3s = _allComponents[TypeRegistry<T3>.typeID];
        var t4s = _allComponents[TypeRegistry<T4>.typeID];
        var entities = _allComponents[TypeRegistry<EntityComponent>.typeID];

        for(uint i = 0; i < t1s.Length; i++)
        {
            if (!t1s.Contains(i) || !t2s.Contains(i) || !t3s.Contains(i) || !t4s.Contains(i))
            {
                continue;
            }
            var entity = (EntityComponent) entities[i];
            lambda(entity, (T1) t1s[i], (T2) t2s[i], (T3) t3s[i], (T4) t4s[i]);
        }
    }

    public AllComponents DebugGetAllComponents()
    {
        return _allComponents;
    }
}
