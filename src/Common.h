#pragma once

#include <stdexcept>
#include <string>
#include <format>

#define GS_NULL_CHECK(value) if (value == nullptr) { \
    throw std::runtime_error(std::string(__FILE__) + ":" + std::to_string(__LINE__) + ": '" + #value + "' was null."); \
}

#define GS_TODO throw std::runtime_error(std::format("{}:{}: {} - Not implemented.", __FILE__, __LINE__, __func__));
