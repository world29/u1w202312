using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unity1Week
{
    public enum FORMAT
    {
        BINARY,
        JSON,
    }

    public interface ISerializer
    {
        string magic { get; }
        int version { get; }
        string filePath { get; }
        FORMAT format { get; }
        System.Type type { get; }
        bool encrypt { get; }
        bool backup { get; }

        bool UpdateVersion(int oldVersion);
        ISerializer Clone();
    }
}
