#pragma once

#include <stdexcept>
#include <string>

#define NULL_CHECK(value) if (value == nullptr) { \
    throw std::runtime_error(std::string(__FILE__) + ":" + std::to_string(__LINE__) + ": '" + #value + "' was null."); \
}
