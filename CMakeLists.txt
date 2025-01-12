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
        include/grabs/Surface.h
        src/Vulkan/VulkanSurface.cpp
        src/Vulkan/VulkanSurface.h
)

target_include_directories(GRABS PUBLIC include)

# ---------- VULKAN ----------
find_package(Vulkan REQUIRED)
target_link_libraries(GRABS PUBLIC ${Vulkan_LIBRARIES})
target_include_directories(GRABS PUBLIC ${Vulkan_INCLUDE_DIR})

if (WIN32)
    add_compile_definitions(GS_OS_WINDOWS)
elseif (UNIX)
    find_package(X11 REQUIRED)
    target_link_libraries(GRABS PUBLIC ${X11_LIBRARIES})
    target_include_directories(GRABS PUBLIC ${X11_INCLUDE_DIR})
    add_compile_definitions(GS_OS_LINUX)
endif ()


# ----- TEST -----
add_executable(GRABStest test/main.cpp)
target_link_libraries(GRABStest PRIVATE GRABS)
#find_package(SDL3 REQUIRED)
add_subdirectory(lib/SDL)
target_link_libraries(GRABStest PRIVATE SDL3::SDL3)

if (WIN32)
    add_custom_command(
            TARGET GRABStest POST_BUILD
            COMMAND "${CMAKE_COMMAND}" -E copy $<TARGET_FILE:SDL3::SDL3-shared> $<TARGET_FILE_DIR:GRABStest>
            VERBATIM
    )
endif ()
