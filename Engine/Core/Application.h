#pragma once
#include <memory>

namespace Engine {

/// <summary>
/// Base class for game applications. The game project subclasses this
/// and returns an instance from the free function CreateApplication().
/// The engine's EntryPoint.cpp owns the main() and calls Run().
/// </summary>
class Application
{
public:
    Application() = default;
    virtual ~Application() = default;

    // Non-copyable
    Application(const Application&) = delete;
    Application& operator=(const Application&) = delete;

    /// <summary>Entry point called once after construction.</summary>
    virtual void OnStart() {}

    /// <summary>Called every frame. dt is delta time in seconds.</summary>
    virtual void OnUpdate(float dt) {}

    /// <summary>Called before destruction / shutdown.</summary>
    virtual void OnShutdown() {}

    /// <summary>
    /// Runs the main loop. Override for custom loop logic.
    /// Default: calls OnStart(), then loops calling OnUpdate(), then OnShutdown().
    /// </summary>
    virtual int Run(int argc, char** argv);
};

/// <summary>
/// Must be defined by the game project. Returns a heap-allocated
/// Application instance. The engine takes ownership and calls delete.
/// </summary>
extern std::unique_ptr<Application> CreateApplication();

} // namespace Engine
