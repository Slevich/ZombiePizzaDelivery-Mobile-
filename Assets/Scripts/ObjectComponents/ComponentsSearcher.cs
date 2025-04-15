using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public static class ComponentsSearcher
{
    /// <summary>
    /// Get all components from GameObject.
    /// </summary>
    /// <param name="ComponentHolder">GameObject with components.</param>
    /// <returns>Array of all GameObject components.</returns>
    public static Component[] GetAllComponentsFromObject(GameObject ComponentHolder)
    {
        return ComponentHolder.GetComponents(typeof(Component));
    }

    /// <summary>
    /// Find component of type on the object and return it.
    /// </summary>
    /// <typeparam name="T">Type of component.</typeparam>
    /// <param name="ComponentHolder">GameObject with components.</param>
    /// <returns>Found component of type or null.</returns>
    public static T GetComponentFromObject<T>(GameObject ComponentHolder)
        where T : class
    {
        if (ComponentHolder.TryGetComponent<T>(out T comp))
            return comp;
        else
            return null;
    }

    /// <summary>
    /// Find component of type on the object and return it.
    /// </summary>
    /// <param name="ComponentHolder">GameObject with components.</param>
    /// <param name="ComponentType">Type of component.</param>
    /// <returns>Found component of type or null.</returns>
    public static Component GetComponentFromObject (GameObject ComponentHolder, Type ComponentType)
    {
        Component searchingComponent = ComponentHolder.GetComponent(ComponentType);

        return searchingComponent;
    }

    /// <summary>
    /// Find components of types on the object and return them.
    /// </summary>
    /// <param name="ComponentHolder">GameObject with components.</param>
    /// <param name="SearchingTypes">Types of components.</param>
    /// <returns>Array of founding components.</returns>
    public static Component[] GetComponentsFromObject (GameObject ComponentHolder, Type[] SearchingTypes)
    {
        List<Component> components = new List<Component>();

        foreach(Type searchingType in SearchingTypes)
        {
            Component component = GetComponentFromObject(ComponentHolder, searchingType);

            if(component != null)
                components.Add(component);
        }

        return components.ToArray();
    }

    /// <summary>
    /// Find components of types on the object and all it's children and return them.
    /// </summary>
    /// <param name="ParentComponentHolder">Parent GameObject to search components.</param>
    /// <param name="SearchingTypes">Types of components.</param>
    /// <returns>Array of founding components.</returns>
    public static Component[] GetComponentsFromObjectAndAllChildren(GameObject ParentComponentHolder, Type[] SearchingTypes)
    {
        return GetComponentsInChildrenRecursively(ParentComponentHolder, SearchingTypes);
    }

    private static Component[] GetComponentsInChildrenRecursively (GameObject ParentComponentHolder, Type[] searchingTypes)
    {
        List<Component> findingComponents = new List<Component>();

        Component[] componentsOfTypes = GetComponentsFromObject(ParentComponentHolder, searchingTypes);

        if (componentsOfTypes != null && componentsOfTypes.Length > 0)
            findingComponents.AddRange(componentsOfTypes);

        int childsCount = ParentComponentHolder.transform.childCount;

        if (childsCount == 0)
            return findingComponents.ToArray();

        for (int i = 0; i < childsCount; i++)
        {
            Transform child = ParentComponentHolder.transform.GetChild(i);
            Component[] componentsInChild = GetComponentsInChildrenRecursively(child.gameObject, searchingTypes);

            if (componentsInChild != null && componentsInChild.Length > 0)
                findingComponents.AddRange(componentsInChild);
        }

        return findingComponents.ToArray();
    }

    private static Component[] FindAllComponentOfTypes (Component[] Components, Type[] FindingTypes)
    {
        IEnumerable<Component> componentsOfTypes = Components.Where(component => component.GetType() == FindingTypes.Any().GetType());
        return componentsOfTypes.ToArray();
    }
}
