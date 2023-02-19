using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using System;

public class DB_Manager : MonoBehaviour
{
    public string DBurl = "https://molegameproject-default-rtdb.firebaseio.com/";
    DatabaseReference d_reference;

    public List<Dictionary<string, object>> scoreList = new List<Dictionary<string, object>>();

    // Start is called before the first frame update
    void Start()
    {
        FirebaseApp.DefaultInstance.Options.DatabaseUrl = new Uri(DBurl);
        d_reference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    public void WriteDB(string name, int score)
    {

        ScoreData Data1 = new ScoreData(name, score);
        string jsondata1 = JsonUtility.ToJson(Data1);

        string key = d_reference.Child("Score").Push().Key;

        d_reference.Child("Score").Child(key).SetRawJsonValueAsync(jsondata1);
    }

    public void ReadDB()
    {
        d_reference = FirebaseDatabase.DefaultInstance.GetReference("Score");
        d_reference.OrderByChild("score").GetValueAsync().ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;

                foreach (DataSnapshot data in snapshot.Children)
                {
                    Dictionary<string, object> ScoreData = (Dictionary<string, object>)data.Value;

                    scoreList.Add(ScoreData);

                }
                scoreList.Reverse();
            }
        }
        );
    }

}

public class ScoreData
{
    public string name = "";
    public int score = 0;

    public ScoreData(string Name, int Score)
    {
        name = Name;
        score = Score;
    }
}
