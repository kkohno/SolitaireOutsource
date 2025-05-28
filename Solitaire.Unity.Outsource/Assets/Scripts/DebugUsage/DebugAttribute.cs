using System;
using NUnit.Framework.Internal;

namespace Scripts.DebugLog
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Interface)]
    public sealed class DebugAttribute : Attribute
    {
        public string Text { get; set; }

        public DebugAttribute(string text)
        {
            Text = text;
        }
    }
}