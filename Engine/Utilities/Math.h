#pragma once

#include "Common/CommonHeaders.h"
#include "MathTypes.h"

namespace fluxion::math {

	template<typename T>
	constexpr T clamp(T value, T min, T max)
	{
		return (value < min) ? min : (value > max) ? max : value;
	}

	template<flu32 bits>
	constexpr flu32 pack_unit_float(flf32 f)
	{
		static_assert(bits <= sizeof(flu32) * 8);
		assert(f >= 0.f && f <= 1.f);
		// NOTE: flu32{1} (not the MSVC-only 1ui32 suffix) keeps this portable
		// across gcc/clang on Linux/macOS/Android/iOS/Web.
		constexpr flf32 intervals{ (flf32)((flu32{ 1 } << bits) - 1) };
		return (flu32)(intervals * f + 0.5f);
	}

	template<flu32 bits>
	constexpr flf32 unpack_to_unit_float(flu32 i)
	{
		static_assert(bits <= sizeof(flu32) * 8);
		assert(i < (flu32{ 1 } << bits));
		constexpr flf32 intervals{ (flf32)((flu32{ 1 } << bits) - 1) };
		return (flf32)i / intervals;
	}

	template<flu32 bits>
	constexpr flu32 pack_float(flf32 f, flf32 min, flf32 max)
	{
		assert(min < max);
		assert(f <= max && f >= min);
		const flf32 distance{ (f - min) / (max - min) };
		return pack_unit_float<bits>(distance);
	}

	template<flu32 bits>
	constexpr flf32 unpack_to_float(flu32 i, flf32 min, flf32 max)
	{
		assert(min < max);
		return unpack_to_unit_float<bits>(i) * (max - min) + min;
	}
}
