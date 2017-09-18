using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class JSONParser{

    public string RegisterDataToJson(string username, string password, string email)
    {
        return string.Format(@"{{""username"":""{0}"",""password"":""{1}"",""email"":""{2}""}}", username, password, email);
    }

    public string LoginDataToJson(string username, string password)
    {
        return string.Format(@"{{""username"":""{0}"",""password"":""{1}""}}", username, password);
    }

    public string LoginUserAndUrlPhotoToJSON(string username,string url)
    {
        return string.Format(@"{{""username"":""{0}"",""photourl"":""{1}""}}", username, url);
    }

    public string[] ElementFromJsonToString(string target)
    {
        string[] newString = Regex.Split(target, "\"");
        return newString;
    }

    
}
