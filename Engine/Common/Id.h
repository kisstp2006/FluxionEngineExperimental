#pragma once
#include "CommonHeaders.h"

namespace fluxion::common::id {
	using id_type = flu32;

	namespace internal
	{
		constexpr flu32 generation_bits(10);
		constexpr flu32 index_bits(sizeof(id_type)*8 -generation_bits);
		constexpr id_type index_mask{ (id_type(1) << index_bits) -1 };
		constexpr id_type generation_mask{ (id_type(1) << generation_bits) - 1 };
	} //internal namespace
	constexpr id_type invalid_id{ (id_type)-1 };


	using generation_type = std::conditional_t<internal::generation_bits <= 16, std::conditional_t<internal::generation_bits <= 8, flu8, flu16>, flu32>;
	static_assert(sizeof(generation_type) * 8 >= internal::generation_bits);
	static_assert(sizeof(id_type) - sizeof(generation_type) > 0);


	inline bool isValid(id_type id) {
		return id != invalid_id;
	}

	inline id_type index(id_type id) {
		id_type index = { id & internal::index_mask };
		assert(index != internal::index_mask);
		return index;
	}
	
	inline id_type generation(id_type id) {
		return (id >> internal::index_bits) & internal::generation_mask;
	}
	inline id_type new_generation(id_type id) {
		const id_type generation(id::generation(id) + 1);
		assert(generation < ((flu64)1<<internal::generation_bits)-1);
		return index(id) | (generation << internal::index_bits);
	}
#if _DEBUG
	namespace internal {
		struct id_base {
			constexpr explicit id_base(id_type id): _id{id}{}
			constexpr operator id_type() const { return _id; }

		private:
			id_type _id;
		};
	}
#define DEFINE_TYPED_ID(name)                                          \
	struct name final : fluxion::common::id::internal::id_base         \
	{                                                                   \
		constexpr explicit name(fluxion::common::id::id_type id)       \
			: id_base{ id } {}                                          \
		constexpr name() : id_base{ fluxion::common::id::invalid_id } {}  \
	};

#else
#define DEFINE_TYPED_ID(name) using name = fluxion::common::id::id_type;
#endif
}

namespace fluxion {
	// Short alias: engine code in any fluxion namespace (ecs, utl, ...)
	// can write id::... without spelling out common::id::.
	namespace id = common::id;
}