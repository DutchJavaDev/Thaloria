using Thaloria.Game.ECS.Class;
using Thaloria.Game.Helpers;

namespace Thaloria.Game.Npc
{
  public sealed class NpcInfo(string textureName, int frameWidth, int frameHeight, int hitBoxWidth, int hitBoxHeight, Animation[] animations)
  {
    private static readonly Dictionary<ThaloriaNpc, NpcInfo> Npcs = new()
    {
     {ThaloriaNpc.Skeleton, new("skeleton_swordless", 48, 48, 13, 21,
    [
        new (AnimationTypes.Idle,5,0),
        new (AnimationTypes.Walking_Right,5,4),
        new (AnimationTypes.Walking_Left,5,4,true), // Flip option to go left
        new (AnimationTypes.Walking_Up,5,5),
        new (AnimationTypes.Idle_Up,5,3),
        new (AnimationTypes.Walking_Down,5,3),
        new (AnimationTypes.Jumping_Right,5,4,true) // Flip option to go left
    ])},
     {ThaloriaNpc.SkeletonSword, new("skeleton", 48, 48, 13, 21,
    [
        new (AnimationTypes.Idle,5,0),
        new (AnimationTypes.Walking_Right,5,4),
        new (AnimationTypes.Walking_Left,5,4,true), // Flip option to go left
        new (AnimationTypes.Walking_Up,5,5),
        new (AnimationTypes.Idle_Up,5,3),
        new (AnimationTypes.Walking_Down,5,3),
        new (AnimationTypes.Jumping_Right,5,4,true) // Flip option to go left
    ])},
     {ThaloriaNpc.Slime, new("slime", 32, 32, 14, 10,
    [
        new (AnimationTypes.Idle,3,0),
        new (AnimationTypes.Walking_Right,6,4),
        new (AnimationTypes.Walking_Left,6,4,true), // Flip option to go left
        new (AnimationTypes.Walking_Up,6,5),
        new (AnimationTypes.Idle_Up,6,3),
        new (AnimationTypes.Walking_Down,6,3),
        new (AnimationTypes.Jumping_Right,6,4,true) // Flip option to go left
    ])}
    };

    public readonly string TextureName = textureName;
    public readonly int FrameWidth = frameWidth;
    public readonly int FrameHeight = frameHeight;
    public readonly int HitBoxWidth = hitBoxWidth;
    public readonly int HitBoxHeight = hitBoxHeight;
    public readonly Animation[] Animations = animations;

    public static NpcInfo GetNpcInfo(ThaloriaNpc npc)
    {
      return Npcs[npc];
    }

  }
}
