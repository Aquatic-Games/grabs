cmake_minimum_required(VERSION 3.29)
project(grabs)

set(CMAKE_CXX_STANDARD 20)

set(GS_BUILD_TEST ON)
set(GS_COMPILE_VK ON)
set(GS_COMPILE_D3D11 OFF)

if (WIN32)
    set(GS_COMPILE_D3D11 ON)
endif()

file(GLOB GS_SRC CONFIGURE_DEPENDS include/*.cpp src/*.cpp src/*.h)

if (GS_COMPILE_VK)
    file(GLOB GS_VK_SRC CONFIGURE_DEPENDS src/Vk/*.h src/Vk/*.cpp)
    list(APPEND GS_SRC ${GS_VK_SRC})
endif()

if (GS_COMPILE_D3D11)
    file(GLOB GS_D3D11_SRC CONFIGURE_DEPENDS src/D3D11/*.h, src/D3D11/*.cpp)
    list(APPEND GS_SRC ${GS_D3D11_SRC})
endif()

message(${GS_SRC})

add_library(grabs STATIC ${GS_SRC})
target_include_directories(grabs PUBLIC include/)

# ---------- Vulkan ----------
if (GS_COMPILE_VK)
    find_package(Vulkan REQUIRED)
    target_link_libraries(grabs PUBLIC ${Vulkan_LIBRARIES})
    target_include_directories(grabs PUBLIC ${Vulkan_INCLUDE_DIR})
    add_compile_definitions(GS_ENABLE_VK)
endif()

# ---------- D3D11 ----------
if (GS_COMPILE_D3D11)
    target_link_libraries(grabs PUBLIC d3d11 dxgi d3dcompiler)
    add_subdirectory(lib/SPIRV-Cross)
    target_link_libraries(grabs PUBLIC spirv-cross-cpp spirv-cross-hlsl)
    add_compile_definitions(GS_ENABLE_D3D11)
endif()

# ----- Test -----
if (GS_BUILD_TEST)
    add_executable(grabsTest test/main.cpp)
    target_link_libraries(grabsTest PUBLIC grabs)
    find_package(Vulkan REQUIRED)
    target_include_directories(grabsTest PUBLIC ${Vulkan_INCLUDE_DIR})
    target_link_libraries(grabsTest PUBLIC C:/VulkanSDK/1.3.296.0/Lib/SDL2main.lib)
    target_link_libraries(grabsTest PUBLIC C:/VulkanSDK/1.3.296.0/Lib/SDL2.lib)
    file(COPY test/Shaders/Test_v.spv DESTINATION ${CMAKE_CURRENT_BINARY_DIR}/)
    file(COPY test/Shaders/Test_p.spv DESTINATION ${CMAKE_CURRENT_BINARY_DIR}/)
endif()