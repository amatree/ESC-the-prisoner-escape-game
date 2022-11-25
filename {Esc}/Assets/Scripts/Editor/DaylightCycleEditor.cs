using System;
using System.Linq;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;


[CustomEditor(typeof(DaylightCycle))]
public class DaylightCycleEditor : Editor
{
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        DaylightCycle daylight_cycle = (DaylightCycle) target;

        if (GUI.changed)
            daylight_cycle.UpdateCall();

        GUILayout.Space(15);
        if (GUILayout.Button("End Editor Test"))
        {
            daylight_cycle.Reset();
        }

    }
}