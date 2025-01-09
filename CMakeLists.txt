cmake_minimum_required(VERSION 3.30)
project(grabs)

set(CMAKE_CXX_STANDARD 20)

add_library(grabs STATIC
        include/grabs/Instance.h
        src/Instance.cpp
        include/grabs/Common.h
        src/Vulkan/VulkanInstance.cpp
        src/Vulkan/VulkanInstance.h
        src/Vulkan/VulkanUtils.h)

target_include_directories(grabs PUBLIC include)

find_package(Vulkan REQUIRED)
target_link_libraries(grabs PUBLIC ${Vulkan_LIBRARY})