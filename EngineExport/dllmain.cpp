// dllmain.cpp : DLL entry point (Windows only).
//
// On non-Windows platforms a shared object (.so/.dylib) needs no explicit
// entry point, so this whole translation unit compiles to nothing there.
#if defined(_WIN32)

#define WIN32_LEAN_AND_MEAN
#include <windows.h>

#if defined(_MSC_VER) && defined(_DEBUG)
#include <crtdbg.h>
#endif

BOOL APIENTRY DllMain( HMODULE hModule,
                       DWORD  ul_reason_for_call,
                       LPVOID lpReserved
                     )
{
    switch (ul_reason_for_call)
    {
    case DLL_PROCESS_ATTACH:
#if defined(_MSC_VER) && defined(_DEBUG)
        _CrtSetDbgFlag(_CRTDBG_ALLOC_MEM_DF | _CRTDBG_LEAK_CHECK_DF);
#endif
        break;

    case DLL_THREAD_ATTACH:
    case DLL_THREAD_DETACH:
    case DLL_PROCESS_DETACH:
        break;
    }
    return TRUE;
}

#endif // _WIN32
