cmake_minimum_required(VERSION 3.20)
project(VulkanMemoryAllocator)

add_library(${PROJECT_NAME} SHARED ../VulkanMemoryAllocator/include/vk_mem_alloc.h vma.cpp)
target_include_directories(${PROJECT_NAME} PUBLIC ../VulkanMemoryAllocator/include)

set_target_properties(${PROJECT_NAME} PROPERTIES LINKER_LANGUAGE CXX)

#if (MSVC)
#add_compile_definitions(VMA_CALL_PRE=__declspec(dllexport) VMA_CALL_POST=__cdecl)
    #endif()
