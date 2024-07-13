using System.Collections.Generic;
using Godot;

namespace Mutemaanpa;

public static class Extension
{
    public static List<KinematicCollision3D> GetSlideCollisions(this CharacterBody3D body)
    {
        var collisionCount = body.GetSlideCollisionCount();
        List<KinematicCollision3D> nodes = [];
        for (int i = 0; i < collisionCount; i++)
        {
            nodes.Add(body.GetSlideCollision(i));
        }
        return nodes;
    }
}
