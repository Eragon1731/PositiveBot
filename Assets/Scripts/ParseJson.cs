using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Serialization;
using System;
using TMPro;

/* Class ParseJson reads .json and stores information. Information is used as
   assets are triggered */
public class ParseJson : MonoBehaviour
{

    //temp
    public string filepath;
    public string imagepath;

    public Canvas poster;
    public SpriteRenderer bk_image;
    
    public float poster_width;
    public float poster_length;

    public TextMeshProUGUI quote_box;
    public TextMeshProUGUI date_box;

    //Protected and Private Containers
    protected List<int> indices;
    protected List<string> quotes;

    private UnityEngine.Object[] images;

    string date;
    SaveData data;
    bool isNewTransforms = false;

    // Read file and set init structs
    void Start()
    {
        ReadStoreJsonValues(filepath);
        LoadResourceImages(imagepath);

        GetDate();

        System.IO.File.Exists(Application.persistentDataPath + "/ObjectData.json");
    }

    /* Read and store json values */
    public void ReadStoreJsonValues(string filepath)
    {
        string file = LoadResourceTextfile(filepath);
        Scene sceneInJson = JsonUtility.FromJson<Scene>(file);

        //init info structs for scene/act 
        indices = new List<int>();
        quotes = new List<string>();

        foreach (Messages test_scene in sceneInJson.messages)
        {
            indices.Add(test_scene.index);
            quotes.Add(test_scene.quote);
        }
    }


    /* Get and check RT date*/
    private void GetDate() {
        var curr_date =  System.DateTime.Now;

        date = curr_date.ToLongDateString();

        if (!PlayerPrefs.HasKey("date") || PlayerPrefs.GetString("date") != date)
        {
            PlayerPrefs.SetString("date", date);

            UpdateScene(date);
            PlayerPrefs.Save();
            isNewTransforms = true;
        }
        else {
            isNewTransforms = false;
        }
        
        AddImage(PlayerPrefs.GetInt("num_images"));

        date_box.text = PlayerPrefs.GetString("date");
        SetTextBlock(PlayerPrefs.GetInt("quote_index"));
        ChangeBKImage(PlayerPrefs.GetFloat("red"), PlayerPrefs.GetFloat("green"), PlayerPrefs.GetFloat("blue"));
    }

    /* update components in scene */
    private void UpdateScene(string date) {

        PlayerPrefs.SetString("date", date);

        var rand_int = UnityEngine.Random.Range(0, indices.Count - 1);
        PlayerPrefs.SetInt("quote_index", rand_int);

        var rand_image = UnityEngine.Random.Range(1, images.Length);
        PlayerPrefs.SetInt("num_images", rand_image);

        var red = UnityEngine.Random.Range(1f, 255.0f);
        var green = UnityEngine.Random.Range(1f, 255.0f);
        var blue = UnityEngine.Random.Range(1f, 255.0f);
        PlayerPrefs.SetFloat("red", red);
        PlayerPrefs.SetFloat("green", green);
        PlayerPrefs.SetFloat("blue", blue);
    }


    private void AddImage(int rand_image)
    {
      //  isNewTransforms = true;

        Objects[] temp = new Objects[rand_image]; 
        if (!isNewTransforms)
        {
            string info = System.IO.File.ReadAllText(Application.persistentDataPath + "/ObjectData.json");
            Debug.Log("info" + info);

            temp = SaveData.GetData(info);
            Debug.Log("temp " + temp.Length);

            rand_image = temp.Length;
        }

        Objects[] data_objs = new Objects[rand_image];
        for (int i = 0; i < rand_image; i++)
        {
            GameObject NewObj = new GameObject(); //Create the GameObject
            SpriteRenderer NewImage = NewObj.AddComponent<SpriteRenderer>(); //Add the Image Component script
            NewImage.sprite = (Sprite)images[i]; //Set the Sprite of the Image Component on the new GameObject

            if (isNewTransforms)
            {
                var path = System.IO.Path.Combine(Application.persistentDataPath, "ObjectData.json");
                System.IO.File.AppendAllText(path, "");

                var width = UnityEngine.Random.Range(-(poster_width / 2), poster_width / 2);
                var length = UnityEngine.Random.Range(-(poster_length / 2), poster_length / 2);
                SaveData data = new SaveData(i, width, length, 1);
                NewObj.transform.position = new Vector3(data.x_pos, data.y_pos, 0);
                data_objs[i] = data.obj;
                Debug.Log("data_objs i" + data_objs[i].index);
            }
            else {
                var width = temp[i].effect.x;
                var length = temp[i].effect.y;
                NewObj.transform.position = new Vector3(width, length, 0);
            }

            NewObj.layer = 4;
            NewObj.SetActive(true);
        }

        if (isNewTransforms)
        {
            Debug.Log("data_objs" + data_objs.Length);
            SaveData.SavetoJson(data_objs);
        }
    }

    /* set Story block text */
    public void SetTextBlock(int index)
    {
        if (quotes[index] != null)
        {
            Debug.Log("quote is " + quotes[index]);
            quote_box.text = quotes[index];
        }
        else
        {
            print("quote doesn't exist");
        }
    }

    private void ChangeBKImage(float red, float green, float blue) {
        Debug.Log("color is "+ new Color32((byte)red, (byte)green, (byte)blue, 255));
        var new_color = new Color32((byte)red, (byte)green, (byte)blue, 255);
        bk_image.color = new_color;
        // Camera.main.backgroundColor = (Color)new_color;
    }

    public static string LoadResourceTextfile(string path)
    {
        TextAsset targetFile = Resources.Load<TextAsset>(path);
        return targetFile.text;
    }

    private void LoadResourceImages(string path) {
        images = Resources.LoadAll(path, typeof(Sprite));
    }
}

/* JSON Structs */

[System.Serializable]
public class Scene
{
    public Messages[] messages;
}

[System.Serializable]
public class Messages
{
    public int index;
    public string quote; 
}

