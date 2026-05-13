using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterPortraitDatabase", menuName = "Dialogue/Character Portrait Database")]
public class CharacterPortraitDatabase : ScriptableObject
{
    [Serializable]
    public class Entry
    {
        public string characterId;
        public Sprite portrait;
    }

    [SerializeField] private List<Entry> entries = new List<Entry>();

    private Dictionary<string, Sprite> _map;

    public Sprite GetPortrait(string characterId)
    {
        if (string.IsNullOrWhiteSpace(characterId))
            return null;
        BuildMapIfNeeded();
        return _map.TryGetValue(characterId.Trim(), out var s) ? s : null;
    }

    private void BuildMapIfNeeded()
    {
        if (_map != null)
            return;
        _map = new Dictionary<string, Sprite>(StringComparer.OrdinalIgnoreCase);
        foreach (var e in entries)
        {
            if (e == null || string.IsNullOrWhiteSpace(e.characterId))
                continue;
            _map[e.characterId.Trim()] = e.portrait;
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        _map = null;
    }
#endif
}
