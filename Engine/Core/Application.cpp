#include "Application.h"

namespace Engine {

int Application::Run(int /*argc*/, char** /*argv*/)
{
    OnStart();

    // Simple fixed-timestep loop — replace with your own timing later.
    while (true)
    {
        OnUpdate(0.016f); // ~60 FPS placeholder
        // TODO: add proper frame timing + quit condition
    }

    OnShutdown();
    return 0;
}

} // namespace Engine
