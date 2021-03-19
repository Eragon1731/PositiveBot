using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveData : MonoBehaviour
{
    public float x_pos;
    public float y_pos;
    public float i;
    public Objects obj;

    public SaveData(int index, float x, float y, int scale) {

        x_pos = x;
        y_pos = y;
        i = index;
        obj = new Objects
        {
            index = index,
            effect = new Transforms {
                x = x,
                 y = y,
                scale = scale
            }
        };

       // Debug.Log("obj" + obj.effect.scale);
       // SavetoJson(obj);
    }
    

    static public void SavetoJson(Objects[] objs) {
        string curr_obj = JsonHelper.ToJson(objs, true);
//        Debug.Log("curr" + curr_obj);

        var path = System.IO.Path.Combine(Application.persistentDataPath, "ObjectData.json");
        System.IO.File.WriteAllText(path, curr_obj);
    }

    static public Objects[] GetData(string json) {
        Objects[] objs = JsonHelper.FromJson<Objects>(json);

        return objs;
    }
   
}

[System.Serializable]
public class Objects
{
    public int index; 
    public Transforms effect;
}

[System.Serializable]
public class Transforms
{
    public float x;
    public float y;
    public int scale;
}



