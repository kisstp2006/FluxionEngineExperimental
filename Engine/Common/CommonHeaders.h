#pragma once

//C/CPP
#include<stdint.h>
#include<assert.h>
#include<typeinfo>
#include<memory>

#if defined(_WIN64)
#include<DirectXMath.h>
#endif

// Debug-only operation wrapper: DEBUG_OP(x) expands to x in debug builds
// and to nothing otherwise. Used by utl::free_list and other utilities.
#ifdef _DEBUG
#define DEBUG_OP(x) x
#else
#define DEBUG_OP(x)
#endif

// Order matters (with #pragma once guarding the cyclic includes):
// types first, then math types, then the math helpers and containers that
// build on them.
#include "../Common/CustomTypes.h"
#include "Utilities/MathTypes.h"
#include "Utilities/Math.h"
#include "Utilities/Utilities.h"