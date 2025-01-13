#pragma once

#include <cstdint>

#define GS_HAS_FLAG(Enum, Value) (Enum & Value) == Value

namespace grabs
{
    enum class Backend
    {
        Unknown = 0,
        Vulkan = 1 << 0
    };

    enum class DebugSeverity
    {
        Verbose,
        Info,
        Warning,
        Error
    };

    inline std::string DebugSeverityToString(DebugSeverity severity)
    {
        switch (severity)
        {
            case DebugSeverity::Verbose:
                return "Verbose";
            case DebugSeverity::Info:
                return "Info";
            case DebugSeverity::Warning:
                return "Warning";
            case DebugSeverity::Error:
                return "Error";
            default:
                return "Unknown";
        }
    }

    enum class DebugType
    {
        General,
        Validation
    };

    inline std::string DebugTypeToString(DebugType type)
    {
        switch (type)
        {
            case DebugType::General:
                return "General";
            case DebugType::Validation:
                return "Validation";
            default:
                return "Unknown";
        }
    }

    typedef void (*GrabsDebugCallback)(DebugSeverity severity, DebugType type, const std::string& message, void* userData);

    enum class AdapterType
    {
        Unknown,
        Software,
        Integrated,
        Discrete
    };

    inline std::string AdapterTypeToString(AdapterType type)
    {
        switch (type)
        {
            case AdapterType::Unknown:
                return "Unknown";
            case AdapterType::Software:
                return "Software";
            case AdapterType::Integrated:
                return "Integrated";
            case AdapterType::Discrete:
                return "Discrete";
            default:
                return "Unknown";
        }
    }

    struct Adapter
    {
        int Index;
        std::string Name;
        AdapterType Type;
        uint64_t DedicatedMemory;
    };
}
