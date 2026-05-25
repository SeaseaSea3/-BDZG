using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ContactDatabase", menuName = "BBPhone/Contact Database")]
public class ContactDatabase : ScriptableObject
{
    public List<ContactProfile> contacts = new List<ContactProfile>();

    public int Count => contacts != null ? contacts.Count : 0;

    public ContactProfile Get(int index)
    {
        if (contacts == null || index < 0 || index >= contacts.Count)
            return null;
        return contacts[index];
    }
}
