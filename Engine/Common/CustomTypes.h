#pragma once

#include<stdint.h>

using flu64 = uint64_t;
using flu32 = uint32_t;
using flu16 = uint16_t;
using flu8 = uint8_t;


using fls64 = int64_t;
using fls32 = int32_t;
using fls16 = int16_t;
using fls8 = int8_t;

constexpr flu64 flu64_invalid_id{ 0xffff'ffff'ffff'ffffULL };
constexpr flu32 flu32_invalid_id{ 0xffff'ffff };
constexpr flu16 flu16_invalid_id{ 0xffff };
constexpr flu8  flu8_invalid_id { 0xff };

using flf32 = float;