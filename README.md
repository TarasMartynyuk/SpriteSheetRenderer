# SpriteSheetRenderer
Original 

##### How to use

* 1- Create Animation/Static Sprite ScriptableObject assert in editor:
* 2 - Init Renderer in your entry point:
```        
SpriteSheetRendererInit.Init(m_spriteSheetShader);
```    
* 3 - Record your spritesheet (bakes spritesheet texture, once for each asset)
```
var renderSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystem<SpriteSheetRenderSystem>();
renderSystem.RecordAnimator(animator); // another overload for static sprite
animator.RenderGroup // now stores a runtime render group that will be used to render all entities that use this spritesheet. It is used to identify animation in unmanaged ECS, and contains non-instanced, constant animation definition data 
```
* 3- Create animated entity

Add required components to your entity:

    // SpriteSheetRenderer - static Sprite
    - SpriteIndex
    - SpriteSheetColor 
    - SpriteSheetRenderGroupHookComponent
    
    // for animation - 
    - SpriteSheetAnimationComponent
        
    // 3D positioning:
    - LocalToWorld    
    (You will most likely use Translation, Rotation & NonUniformScale to work with LocalToWorld. It is required for SpriteSheetFactory.Init(), but you can init manually and work with LTW only.

    These component lists are defined and stored in SpriteSheetFactory.

Add entity to render group:
```
SpriteSheetFactory.InitAnimatedSprite(entity, animation);
```

* 4 Working with animated sprite
* 
You can work with entity as you would with any other 3D entity - modifying LocalToWorld or it's components, using Parent + LocalToParent + Child for hierarchy etc.

**To Change animation**:
```
static SpriteSheetAnimationSystem.SetAnimation(Entity e, Entity animationRenderGroup, bool keepProgress = false);
```
It is burst-compatible and does not incur a structural change, so you could do this operation inside a job, however the job-version is not provided since it is: 
1) too verbose
2) widely used, so would add a write dependency on RenderGroupHookCmp to all your jobs, probably not allowing rendering jobs to run in parallel with simulation.

Instead, inside the jobs, you should add a deferred animation change command - they are applied in batch each frame:
```
// animChangeCommands is a singleton created in Init
GetBuffer<AnimationChangeCommandBufferElement>(animChangeCommands).
                        Add(new AnimationChangeCommandBufferElement {Target = entity, RenderGroupToSet = attackAnim});
```

**Check if animation event was triggered** (e.g hit frame for attack animation):
```
// in SpriteSheetAnimationComponent
// true for the first frame when the animation event sprite(keyframe) is rendered
public bool IsAnimationEventTriggeredThisFrame;
```


##### Fork changes compared to original 

