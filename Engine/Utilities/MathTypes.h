#pragma once
#include "Common/CustomTypes.h"

// ── Platform detection ─────────────────────────────────────────────
// Each platform gets an explicit slot so a specialized math backend
// can replace the generic one later (NEON, wasm SIMD, ...).
// NOTE: __ANDROID__ must be checked before __linux__ (Android defines both).
#if defined(_WIN64)
	#define FLUXION_MATH_DIRECTXMATH 1
#elif defined(__EMSCRIPTEN__)
	#define FLUXION_MATH_GENERIC 1 // web / wasm (TODO: wasm SIMD compute backend)
#elif defined(__ANDROID__)
	#define FLUXION_MATH_GENERIC 1 // Android (TODO: ARM NEON compute backend)
#elif defined(__APPLE__)
	#define FLUXION_MATH_GENERIC 1 // macOS / iOS (TODO: NEON or simd.h compute backend)
#elif defined(__linux__)
	#define FLUXION_MATH_GENERIC 1 // Linux
#elif defined(_WIN32)
	#define FLUXION_MATH_GENERIC 1 // 32-bit Windows
#else
	#error "MathTypes.h: unsupported platform"
#endif

#if defined(FLUXION_MATH_DIRECTXMATH)
#include <DirectXMath.h>
#endif

namespace fluxion::math {
	constexpr float pi = 3.1415926535897932384626433832795f;
	constexpr float epsilon = 1e-5f;

#if defined(FLUXION_MATH_DIRECTXMATH)
	// DirectXMath storage types
	using v2 = DirectX::XMFLOAT2;
	using v2a = DirectX::XMFLOAT2A;
	using v3 = DirectX::XMFLOAT3;
	using v3a = DirectX::XMFLOAT3A;
	using v4 = DirectX::XMFLOAT4;
	using v4a = DirectX::XMFLOAT4A;

	using u32v2 = DirectX::XMUINT2;
	using u32v3 = DirectX::XMUINT3;
	using u32v4 = DirectX::XMUINT4;

	using s32v2 = DirectX::XMINT2;
	using s32v3 = DirectX::XMINT3;
	using s32v4 = DirectX::XMINT4;

	using m3x3 = DirectX::XMFLOAT3X3; // NOTE: DirectXMath doesn't have aligned 3x3 matrices
	using m4x4 = DirectX::XMFLOAT4X4;
	using m4x4a = DirectX::XMFLOAT4X4A;
#elif defined(FLUXION_MATH_GENERIC)
	// Plain storage types with the same size, alignment, row-major layout
	// and constructor set as the DirectXMath ones, so component data and
	// engine code stay identical across platforms.
	//TODO: replacethem with something more professional like glm

	// Generates the member set of a 2/3/4 component vector type.
#define FLUXION_VEC2_BODY(name, T)                                              \
		T x, y;                                                                 \
		name() = default;                                                       \
		constexpr name(T _x, T _y) : x{ _x }, y{ _y } {}                        \
		constexpr explicit name(const T* a) : x{ a[0] }, y{ a[1] } {}
#define FLUXION_VEC3_BODY(name, T)                                              \
		T x, y, z;                                                              \
		name() = default;                                                       \
		constexpr name(T _x, T _y, T _z) : x{ _x }, y{ _y }, z{ _z } {}         \
		constexpr explicit name(const T* a) : x{ a[0] }, y{ a[1] }, z{ a[2] } {}
#define FLUXION_VEC4_BODY(name, T)                                              \
		T x, y, z, w;                                                           \
		name() = default;                                                       \
		constexpr name(T _x, T _y, T _z, T _w)                                  \
			: x{ _x }, y{ _y }, z{ _z }, w{ _w } {}                             \
		constexpr explicit name(const T* a)                                     \
			: x{ a[0] }, y{ a[1] }, z{ a[2] }, w{ a[3] } {}

	struct v2 { FLUXION_VEC2_BODY(v2, flf32) };
	struct alignas(16) v2a { FLUXION_VEC2_BODY(v2a, flf32) };
	struct v3 { FLUXION_VEC3_BODY(v3, flf32) };
	struct alignas(16) v3a { FLUXION_VEC3_BODY(v3a, flf32) };
	struct v4 { FLUXION_VEC4_BODY(v4, flf32) };
	struct alignas(16) v4a { FLUXION_VEC4_BODY(v4a, flf32) };

	struct u32v2 { FLUXION_VEC2_BODY(u32v2, flu32) };
	struct u32v3 { FLUXION_VEC3_BODY(u32v3, flu32) };
	struct u32v4 { FLUXION_VEC4_BODY(u32v4, flu32) };

	struct s32v2 { FLUXION_VEC2_BODY(s32v2, fls32) };
	struct s32v3 { FLUXION_VEC3_BODY(s32v3, fls32) };
	struct s32v4 { FLUXION_VEC4_BODY(s32v4, fls32) };

#undef FLUXION_VEC2_BODY
#undef FLUXION_VEC3_BODY
#undef FLUXION_VEC4_BODY

	struct m3x3 {
		flf32 m[3][3];
		m3x3() = default;
		explicit m3x3(const flf32* a) {
			for (int r = 0; r < 3; ++r)
				for (int c = 0; c < 3; ++c)
					m[r][c] = a[r * 3 + c];
		}
	};
	struct m4x4 {
		flf32 m[4][4];
		m4x4() = default;
		explicit m4x4(const flf32* a) {
			for (int r = 0; r < 4; ++r)
				for (int c = 0; c < 4; ++c)
					m[r][c] = a[r * 4 + c];
		}
	};
	struct alignas(16) m4x4a {
		flf32 m[4][4];
		m4x4a() = default;
		explicit m4x4a(const flf32* a) {
			for (int r = 0; r < 4; ++r)
				for (int c = 0; c < 4; ++c)
					m[r][c] = a[r * 4 + c];
		}
	};
#endif
}
