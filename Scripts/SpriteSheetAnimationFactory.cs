
using System;
using Unity.Entities;

public class SpriteSheetAnimationFactory : SingletonBase<SpriteSheetAnimationFactory>
{
    public EntityDefinition SpriteSheetAnimationDefinition = new(new ComponentType[] { typeof(SpriteSheetAnimationDefinitionComponent) });
}
