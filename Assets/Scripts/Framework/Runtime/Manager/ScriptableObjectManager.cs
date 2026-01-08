using UnityEngine;

public class ScriptableObjectModelBase<T> where T : ScriptableObject
{
    public T scriptableObject { get; private set; }



}


public class ScriptableObjectManager : Singleton<ScriptableObjectManager>, IManager
{
    public class ScriptableEntry
    {
        public string scriptableName;
        public ScriptableObject scriptableObj;

    }
}