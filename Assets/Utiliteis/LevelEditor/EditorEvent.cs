using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace NDR.Utilities
{
    [CustomEditor(typeof(LevelEditor))]
    public class EditorEvent : Editor
    {
        LevelEditor lvl;

        void OnSceneGUI()
        {
            if (lvl == null)
            {
                //Debug.LogWarning("set instance of Level editor ... ");
                lvl = LevelEditor.instance;
            }

            if (lvl == null)
            {
                Debug.LogWarning("Level is not set a instance ... ");
                return;
            }
          
           
            Event e = Event.current;
            HandleKeys(e);

        }

        void HandleMouse(Event e)
        {
            if (!lvl.editMode)
                return;


            Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
            RaycastHit hit;

            if(Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                bool isRight = (e.button == 1);
                Debug.Log(hit.collider.name);

                // edit the nodes.
                lvl.ChangeNodeStatusOnPosition(hit.point, isRight);
            }
        }

        void HandleKeys(Event e )
        {
            Vector3 mousePos = Vector3.up * -500;

            Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                mousePos = hit.point;
            }

            if (e.keyCode == lvl.hotKeys.editMode)
            {
                Debug.Log("Edit Mode");
                lvl.editMode = !lvl.editMode;
            }
            if(e.keyCode == lvl.hotKeys.initLevel)
            {
                lvl.InitializeLevel();
                Debug.Log("Init Level");
            }
            if(e.keyCode == lvl.hotKeys.saveLevel)
            {
                lvl.SaveLevel();
                Debug.Log("Save Level.");
            }
            if (e.keyCode == lvl.hotKeys.clearLevel)
            {
                lvl.ClearLevel();
                Debug.Log("Clear Level.");
            }
            if(e.keyCode == lvl.hotKeys.canWalk)
            {
                lvl.ChangeNodeStatusOnPosition(mousePos, true);
            }
            if(e.keyCode == lvl.hotKeys.dontWalk)
            {
                lvl.ChangeNodeStatusOnPosition(mousePos, false);
            }
        }
    }
}