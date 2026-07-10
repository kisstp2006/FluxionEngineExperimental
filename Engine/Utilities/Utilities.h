#pragma once
// 1 = use std::vector (default, safe with existing code).
// 0 = use the custom fluxion::utl::vector from Vector.h.
#define USE_STL_VECTOR 1
#define USE_STL_DEQUE 1

#if USE_STL_VECTOR
#include <vector>
#include <algorithm>
namespace fluxion::utl {
	template<typename T>
	using vector = std::vector<T>;

	template<typename T>
	void erase_unordered(std::vector<T>& v, size_t index)
	{
		if (v.size() > 1) {
			std::iter_swap(v.begin() + index, v.end() - 1);
			v.pop_back();
		}
		else
		{
			v.clear();
		}
	}
}
#else
#include "Vector.h"
namespace fluxion::utl {
	template<typename T>
	void erase_unordered(T& v, size_t index)
	{
		v.erase_unordered(index);
	}
}
#endif

#if USE_STL_DEQUE
#include <deque>
namespace fluxion::utl {
	template<typename T>
	using deque = std::deque<T>;
}
#endif

namespace fluxion::utl {
	// TODO: implement our own containers, etc.
}

#include "FreeList.h"
