using System;
using UnityEngine;
using UnityEngine.Events;

namespace BennyKok.RuntimeDebug.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Field | AttributeTargets.Property)]
    public class DebugActionAttribute : Attribute
    {
        public string name;
        public string group;
        public string id;
        public string description;
        public string shortcutKey;
        public bool closePanelAfterTrigger = false;
    }
}