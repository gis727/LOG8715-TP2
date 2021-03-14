//#define BAD_PERF // TODO CHANGEZ MOI. Mettre en commentaire pour utiliser votre propre structure

using System;
using UnityEngine;

#if BAD_PERF
using InnerType = System.Collections.Generic.Dictionary<uint, IComponent>;
using AllComponents = System.Collections.Generic.Dictionary<uint, System.Collections.Generic.Dictionary<uint, IComponent>>;
#else
//using InnerType = ...; // Non necessaire
using AllComponents = System.Collections.Generic.Dictionary<uint, IComponent[]>;
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
            for(int i = 0; i < type.Value.Length; i++)
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
            _allComponents[TypeRegistry<T>.typeID] = new IComponent[entitiesCount];
        }
        _allComponents[TypeRegistry<T>.typeID][entityID] = component;   
    }
    public void RemoveComponent<T>(EntityComponent entityID) where T : IComponent
    {
        _allComponents[TypeRegistry<T>.typeID][entityID] = null;
    }
    public T GetComponent<T>(EntityComponent entityID) where T : IComponent
    {
        return (T)_allComponents[TypeRegistry<T>.typeID][entityID];
    }
    public bool TryGetComponent<T>(EntityComponent entityID, out T component) where T : IComponent
    {
        if (_allComponents.ContainsKey(TypeRegistry<T>.typeID))
        {
            if (_allComponents[TypeRegistry<T>.typeID][entityID] != null)
            {
                component = (T)_allComponents[TypeRegistry<T>.typeID][entityID];
                return true;
            }
        }
        component = default;
        return false;
    }

    public bool EntityContains<T>(EntityComponent entity) where T : IComponent
    {
        return _allComponents[TypeRegistry<T>.typeID][entity.id] != null;
    }

    public void ClearComponents<T>() where T : IComponent
    {
        if (!_allComponents.ContainsKey(TypeRegistry<T>.typeID))
        {
            _allComponents.Add(TypeRegistry<T>.typeID, new IComponent[entitiesCount]);
        }
        else
        {
           _allComponents[TypeRegistry<T>.typeID] = new IComponent[entitiesCount];
        }
    }

    public void ForEach<T1>(Action<EntityComponent, T1> lambda) where T1 : IComponent
    {
        IComponent[] t1s = _allComponents[TypeRegistry<T1>.typeID];
        IComponent[] entities = _allComponents[TypeRegistry<EntityComponent>.typeID];

        for(uint i = 0; i < t1s.Length; i++)
        {
            if (t1s[i] == null)
            {
                continue;
            }
            var entity = (EntityComponent) entities[i];
            lambda(entity, (T1) t1s[i]);
        }
    }

    public void ForEach<T1, T2>(Action<EntityComponent, T1, T2> lambda) where T1 : IComponent where T2 : IComponent
    {
        IComponent[] t1s = _allComponents[TypeRegistry<T1>.typeID];
        IComponent[] t2s = _allComponents[TypeRegistry<T2>.typeID];
        IComponent[] entities = _allComponents[TypeRegistry<EntityComponent>.typeID];

        for(uint i = 0; i < t1s.Length; i++)
        {
            if (t1s[i] == null || t2s[i] == null)
            {
                continue;
            }
            var entity = (EntityComponent) entities[i];
            lambda(entity, (T1) t1s[i], (T2) t2s[i]);
        }
    }

    public void ForEach<T1, T2, T3>(Action<EntityComponent, T1, T2, T3> lambda) where T1 : IComponent where T2 : IComponent where T3 : IComponent
    {
        IComponent[] t1s = _allComponents[TypeRegistry<T1>.typeID];
        IComponent[] t2s = _allComponents[TypeRegistry<T2>.typeID];
        IComponent[] t3s = _allComponents[TypeRegistry<T3>.typeID];
        IComponent[] entities = _allComponents[TypeRegistry<EntityComponent>.typeID];

        for(uint i = 0; i < t1s.Length; i++)
        {
            if (t1s[i] == null || t2s[i] == null || t3s[i] == null)
            {
                continue;
            }
            var entity = (EntityComponent) entities[i];
            lambda(entity, (T1) t1s[i], (T2) t2s[i], (T3) t3s[i]);
        }
    }

    public void ForEach<T1, T2, T3, T4>(Action<EntityComponent, T1, T2, T3, T4> lambda) where T1 : IComponent where T2 : IComponent where T3 : IComponent where T4 : IComponent
    {
        IComponent[] t1s = _allComponents[TypeRegistry<T1>.typeID];
        IComponent[] t2s = _allComponents[TypeRegistry<T2>.typeID];
        IComponent[] t3s = _allComponents[TypeRegistry<T3>.typeID];
        IComponent[] t4s = _allComponents[TypeRegistry<T4>.typeID];
        IComponent[] entities = _allComponents[TypeRegistry<EntityComponent>.typeID];

        for(uint i = 0; i < t1s.Length; i++)
        {
            if (t1s[i] == null || t2s[i] == null || t3s[i] == null || t4s[i] == null)
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
