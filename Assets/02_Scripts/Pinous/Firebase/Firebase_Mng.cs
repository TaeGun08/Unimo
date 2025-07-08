using Firebase.Auth;
using Firebase;
using UnityEngine;
using Firebase.Extensions;
using Firebase.Database;

public partial class Firebase_Mng
{
    public string UserID = "";
    public string UserName;
    public bool isSetFirebase = false;
    FirebaseAuth auth;
    DatabaseReference reference;
    
    public void Init()
    {
        // Firebase �ʱ�ȭ �� �ν��Ͻ� ����
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            FirebaseApp app = FirebaseApp.DefaultInstance;
            auth = FirebaseAuth.DefaultInstance;
            reference = FirebaseDatabase.DefaultInstance.RootReference;
        });
    }

    public void Login(EAccountType type)
    {
        FirebaseInit(type);
    }
}
