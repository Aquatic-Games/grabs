#ifdef _MSC_VER
#define VMA_CALL_PRE __declspec(dllexport)
#define VMA_CALL_POST __cdecl
#endif

#define VMA_IMPLEMENTATION
#include <vk_mem_alloc.h>
