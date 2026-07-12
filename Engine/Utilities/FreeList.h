#pragma once
#include "Common/CommonHeaders.h"

// Explicit include: this container uses memset (<cstring>). MSVC pulls it in
// transitively, but libc++/libstdc++ do not — include it directly for portability.
#include <cstring>

namespace fluxion::utl {

#if USE_STL_VECTOR
#pragma message("WARNING: using utl::free_list with std::vector results in duplicate calls to class constructor!")
#endif

	template<typename T>
	class free_list
	{
		static_assert(sizeof(T) >= sizeof(flu32));
	public:
		free_list() = default;
		explicit free_list(flu32 count)
		{
			_array.reserve(count);
		}

		~free_list()
		{
			assert(!_size);
#if USE_STL_VECTOR
			memset(_array.data(), 0, _array.size() * sizeof(T));
#endif
		}

		template<class... params>
		constexpr flu32 add(params&&... p)
		{
			flu32 id{ flu32_invalid_id };
			if (_next_free_index == flu32_invalid_id)
			{
				id = (flu32)_array.size();
				_array.emplace_back(std::forward<params>(p)...);
			}
			else
			{
				id = _next_free_index;
				assert(id < _array.size() && already_removed(id));
				_next_free_index = *(const flu32 *const)std::addressof(_array[id]);
				new (std::addressof(_array[id])) T(std::forward<params>(p)...);
			}
			++_size;
			return id;
		}

		constexpr void remove(flu32 id)
		{
			assert(id < _array.size() && !already_removed(id));
			T& item{ _array[id] };
			item.~T();
			DEBUG_OP(memset(std::addressof(_array[id]), 0xcc, sizeof(T)));
			*(flu32 *const)std::addressof(_array[id]) = _next_free_index;
			_next_free_index = id;
			--_size;
		}

		constexpr flu32 size() const
		{
			return _size;
		}

		constexpr flu32 capacity() const
		{
			return _array.size();
		}

		constexpr bool empty() const
		{
			return _size == 0;
		}

		[[nodiscard]] constexpr T& operator[](flu32 id)
		{
			assert(id < _array.size() && !already_removed(id));
			return _array[id];
		}

		[[nodiscard]] constexpr const T& operator[](flu32 id) const
		{
			assert(id < _array.size() && !already_removed(id));
			return _array[id];
		}

	private:
		constexpr bool already_removed(flu32 id)
		{
			// NOTE: when sizeof(T) == sizeof(flu32) we can't test if the item was already removed!
			if constexpr (sizeof(T) > sizeof(flu32))
			{
				flu32 i{ sizeof(flu32) }; // skip the first 4 bytes.
				const flu8 *const p{ (const flu8 *const)std::addressof(_array[id]) };
				while ((p[i] == 0xcc) && (i < sizeof(T))) ++i;
				return i == sizeof(T);
			}
			else
			{
				return true;
			}
		}
#if USE_STL_VECTOR
		utl::vector<T>          _array;
#else
		utl::vector<T, false>   _array;
#endif
		flu32                   _next_free_index{ flu32_invalid_id };
		flu32                   _size{ 0 };
	};
}
