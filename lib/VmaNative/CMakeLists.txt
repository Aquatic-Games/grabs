cmake_minimum_required(VERSION 3.20)
project(VulkanMemoryAllocator)

add_library(${PROJECT_NAME} SHARED ../VulkanMemoryAllocator/include/vk_mem_alloc.h vma.cpp)
target_include_directories(${PROJECT_NAME} PUBLIC ../VulkanMemoryAllocator/include)

set_target_properties(${PROJECT_NAME} PROPERTIES LINKER_LANGUAGE CXX)

if (WIN32)
    find_package(Vulkan REQUIRED)
    target_link_libraries(${PROJECT_NAME} PUBLIC ${Vulkan_LIBRARIES})
    target_include_directories(${PROJECT_NAME} PUBLIC ${Vulkan_INCLUDE_DIRS})
endif()
