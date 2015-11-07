//by Bob Berkebile : Pixelplacement : http://www.pixelplacement.com

using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(iTweenPath))]
public class iTweenPathEditor : Editor
{
	iTweenPath _target;
	GUIStyle style = new GUIStyle();
	public static int count = 0;
	
	void OnEnable(){
		//languageIndex like bold handle labels since I'm getting old:
		style.fontStyle = FontStyle.Bold;
		style.normal.textColor = Color.white;
		_target = (iTweenPath)target;
		
		//lock in a default path firstIndexString:
		if(!_target.initialized){
			_target.initialized = true;
			_target.pathName = "New Path " + ++count;
			_target.initialName = _target.pathName;
		}
	}
	
	public override void OnInspectorGUI(){		
		//path firstIndexString:
        _target.pathName = EditorGUILayout.TextField("Path Name", _target.pathName);
		
        if(_target.pathName == ""){
            _target.pathName = _target.initialName;
        }
		
        //path color:
        _target.pathColor = EditorGUILayout.ColorField("Path Color",_target.pathColor);

        //Relative or absolute
        _target.isRelative = EditorGUILayout.Toggle("Relative",_target.isRelative);

        //keep to ground?
        _target.keepToGround = EditorGUILayout.Toggle("Keep to ground", _target.keepToGround);
        
        if (_target.keepToGround)
        {
            ++(EditorGUI.indentLevel);
            _target.distanceToGround = EditorGUILayout.FloatField("Distance ",_target.distanceToGround);
            --(EditorGUI.indentLevel);
        }

		
		//exploration segment count control:
		_target.nodeCount =  Mathf.Clamp(EditorGUILayout.IntSlider("Node Count",_target.nodeCount, 0, 30), 2,100);
		
		//add node?
		if(_target.nodeCount > _target.nodes.Count){
			for (int i = 0; i < _target.nodeCount - _target.nodes.Count; i++) {
				_target.nodes.Add(_target.isRelative ? new Vector3(1f,1f,1f) : Vector3.zero);	
			}
		}
	
		//remove node?
		if(_target.nodeCount < _target.nodes.Count){
			if(EditorUtility.DisplayDialog("Remove path node?","Shortening the node list will permantently destory parts of your path. This operation cannot be undone.", "OK", "Cancel")){
				int removeCount = _target.nodes.Count - _target.nodeCount;
				_target.nodes.RemoveRange(_target.nodes.Count-removeCount,removeCount);
			}else{
				_target.nodeCount = _target.nodes.Count;	
			}
		}
				
		//node display:
		//EditorGUI.indentLevel = 1;
        for (int i = 0; i < _target.nodes.Count; i++) {
			_target.nodes[i] = EditorGUILayout.Vector3Field("Node " + (i+1), _target.nodes[i]);
		}
		
		//update and redraw:
		if(GUI.changed){
			EditorUtility.SetDirty(_target);			
		}
	}
	
	void OnSceneGUI(){
		if(_target.enabled) { // dkoontz
			if(_target.nodes.Count > 0){
				//allow path adjustment undo:
				Undo.SetSnapshotTarget(_target,"Adjust iTween Path");
				
				//path begin and end labels:
                if (_target.isRelative)
                {
                    Handles.Label(_target.transform.TransformPoint(_target.nodes[0]), "'" + _target.pathName + "' Begin", style);
                    Handles.Label(_target.transform.TransformPoint(_target.nodes[_target.nodes.Count - 1]), "'" + _target.pathName + "' End", style);
                }
                else
                {
                    Handles.Label(_target.nodes[0], "'" + _target.pathName + "' Begin", style);
                    Handles.Label(_target.nodes[_target.nodes.Count - 1], "'" + _target.pathName + "' End", style);
                }
				
				//node handle display:
				for (int i = 0; i < _target.nodes.Count; i++) {
                    Vector3 nodeGlobal;
                    if (_target.isRelative)
                        nodeGlobal = Handles.PositionHandle(_target.transform.TransformPoint(_target.nodes[i]), Quaternion.identity);
                    else
                        nodeGlobal = Handles.PositionHandle(_target.nodes[i], Quaternion.identity);
                    if (_target.keepToGround)
                    {
                        RaycastHit hit;
                        int mask = 1 << 9;
                        if (Physics.Raycast(nodeGlobal, -Vector3.up, out hit, 500f, mask))
                        {
                            nodeGlobal.y = _target.distanceToGround + hit.point.y;
                        }
                    }
                    if (_target.isRelative)
                        _target.nodes[i] = _target.transform.InverseTransformPoint(nodeGlobal);
                    else
                        _target.nodes[i] = nodeGlobal;
				}	
			}
		} // dkoontz
	}
}
