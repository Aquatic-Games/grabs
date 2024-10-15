cmake_minimum_required(VERSION 3.29)
project(grabs)

set(CMAKE_CXX_STANDARD 20)

add_library(grabs STATIC
        include/grabs/Instance.h
        src/Instance.cpp
        include/grabs/Common.h
        src/Vk/VulkanInstance.cpp
        src/Vk/VulkanInstance.h
        src/Common.h
        src/Vk/VkUtils.h
        src/Vk/VkUtils.cpp)

target_include_directories(grabs PUBLIC include/)

# ---------- Vulkan ----------
find_package(Vulkan REQUIRED)
target_link_libraries(grabs PUBLIC vulkan)
target_include_directories(grabs PUBLIC ${Vulkan_INCLUDE_DIR})