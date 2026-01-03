using ModFrameworkNs.Publish;
using ProjectSchoolModNs.Publish;
using AcademySmith;

// Mark script entry point
[assembly:ModEntry]
namespace AcademySmith
{
    // There must be a module feature class, which is responsible for registering various game features
    public class Main : BootableModFunctionBase
    {
        // Game context interface
        M_IGameContext gameContext;

        protected override void OnInit()
        {
            // Gets the game context interface from the module context
            context.RunnerContext.TryGetContext(out gameContext);
        }

        protected override void OnEnable()
        {
            // Register custom modules
            gameContext.ModuleContext.RegisterGameModule<Modules>(context);
        }
        protected override void OnDisable()
        {
            // Deregister the custom module
            gameContext.ModuleContext.UnregisterGameModule<Modules>(context);
        }

        protected override void OnDispose()
        {
        }
    }
}
