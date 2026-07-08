#pragma once
#include "Common/CustomTypes.h"

#if defined(_WIN64)
#include <DirectXMath.h>
#endif

namespace fluxion::math {
	constexpr float pi = 3.1415926535897932384626433832795f;
	constexpr float epsilon = 1e-5f;

#if defined(_WIN64)
	// Windows: DirectXMath storage types
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
#else
	// Linux (and any other non-Win64 platform): plain storage types with
	// the same size, alignment and row-major layout as the DirectXMath
	// ones, so component data stays bit-identical across platforms.
	struct v2 { flf32 x, y; };
	struct alignas(16) v2a { flf32 x, y; };
	struct v3 { flf32 x, y, z; };
	struct alignas(16) v3a { flf32 x, y, z; };
	struct v4 { flf32 x, y, z, w; };
	struct alignas(16) v4a { flf32 x, y, z, w; };

	struct u32v2 { flu32 x, y; };
	struct u32v3 { flu32 x, y, z; };
	struct u32v4 { flu32 x, y, z, w; };

	struct s32v2 { fls32 x, y; };
	struct s32v3 { fls32 x, y, z; };
	struct s32v4 { fls32 x, y, z, w; };

	struct m3x3 { flf32 m[3][3]; };
	struct m4x4 { flf32 m[4][4]; };
	struct alignas(16) m4x4a { flf32 m[4][4]; };
#endif
}
