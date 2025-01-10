cmake_minimum_required(VERSION 3.30)
project(GRABS)

set(CMAKE_CXX_STANDARD 20)

add_library(GRABS STATIC
        include/grabs/Instance.h
        src/Instance.cpp
        include/grabs/Common.h
        src/Vulkan/VulkanInstance.cpp
        src/Vulkan/VulkanInstance.h
        src/Vulkan/VulkanUtils.h
)

target_include_directories(GRABS PUBLIC include)

# ---------- VULKAN ----------
find_package(Vulkan REQUIRED)
target_link_libraries(GRABS PUBLIC ${Vulkan_LIBRARY})

if (UNIX)
find_package(X11 REQUIRED)
target_link_libraries(GRABS PUBLIC ${X11_LIBRARIES})
target_include_directories(GRABS PUBLIC ${X11_INCLUDE_DIR})
endif ()


# ----- TEST -----
add_executable(GRABStest test/main.cpp)
target_link_libraries(GRABStest PRIVATE GRABS)
find_package(SDL3 REQUIRED)
target_link_libraries(GRABStest PRIVATE SDL3::SDL3)
