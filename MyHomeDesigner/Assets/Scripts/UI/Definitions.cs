[System.Serializable]
public class EmailOnlyPayload
{
    public string email;
}

[System.Serializable]
public class ChangePasswordPayload
{
    public string oldPassword;
    public string newPassword;
}

[System.Serializable]
public class ChangePasswordResponse
{
    public string message;
    public string token;
}

[System.Serializable]
public class UpdateEmailResponse
{
    public string message;
    public string token;
}

