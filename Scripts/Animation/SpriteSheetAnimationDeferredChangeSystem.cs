using Unity.Entities;

public class SpriteSheetAnimationDeferredChangeSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var commands = this.GetSingletonBuffer<AnimationChangeCommandBufferElement>();

        foreach (var command in commands)
            SpriteSheetManager.SetAnimation(command.Target, command.RenderGroupToSet, command.KeepProgress);
        
        commands.Clear();
    }
}
