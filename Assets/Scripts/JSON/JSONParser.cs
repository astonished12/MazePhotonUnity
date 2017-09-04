using System.Collections;
using System.Collections.Generic;
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

}
