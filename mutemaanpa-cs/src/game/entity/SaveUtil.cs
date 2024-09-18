using Godot;

namespace Mutemaanpa;

public class SaveUtil
{
    public static SaveList SaveTransform(Transform3D transform)
    {
        var result = new SaveList();
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                result.Add(transform[i][j]);
            }
        }
        return result;
    }

    public static Transform3D LoadTransform(SaveList list)
    {
        var result = new Transform3D();
        for (int i = 0; i < 4; i++)
        {
            var vector = new Vector3();
            for (int j = 0; j < 3; j++)
            {
                vector[j] = (float)list[i * 3 + j];
            }
            result[i] = vector;
        }
        return result;
    }
}