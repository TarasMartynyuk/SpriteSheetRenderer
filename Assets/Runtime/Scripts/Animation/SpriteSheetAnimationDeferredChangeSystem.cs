using Unity.Entities;
using UnityEngine;

public partial class SpriteSheetAnimationDeferredChangeSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var commands = this.GetSingletonBuffer<AnimationChangeCommandBufferElement>();

        foreach (var command in commands)
        {
            SpriteSheetAnimationSystem.SetAnimation(command.Target, command.RenderGroupToSet, command.KeepProgress);
            // DebugExtensions.LogVar(new { tgt = command.Target.Stringify(), anim = command.RenderGroupToSet.Stringify() }, "Deferred Anim change: ",
                // addCurrFrame: true);
        }
        
        commands.Clear();
    }
}
