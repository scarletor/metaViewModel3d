using UnityEngine;

public static class StringUtils
{
    public static Vector3 StringToVector3(string vector3Str)
    {
        string[] coordinatesValue = vector3Str.Split('/');
        for (var index = 0; index < coordinatesValue.Length; index++)
        {
            if (string.IsNullOrEmpty(coordinatesValue[index])) coordinatesValue[index] = "0";
        }
        return new Vector3(float.Parse(coordinatesValue[0].Replace(',','.')), float.Parse(coordinatesValue[1].Replace(',', '.')), float.Parse(coordinatesValue[2].Replace(',', '.')));
    }
}