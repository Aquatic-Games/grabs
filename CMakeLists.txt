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
        src/Vk/VkUtils.cpp
        include/grabs/Device.h
        src/Vk/VulkanDevice.cpp
        src/Vk/VulkanDevice.h
        include/grabs/Surface.h
        include/grabs/Swapchain.h
        src/Vk/VulkanSwapchain.cpp
        src/Vk/VulkanSwapchain.h)

target_include_directories(grabs PUBLIC include/)

# ---------- Vulkan ----------
find_package(Vulkan REQUIRED)
target_link_libraries(grabs PUBLIC ${Vulkan_LIBRARIES})
target_include_directories(grabs PUBLIC ${Vulkan_INCLUDE_DIR})

# ----- Test -----
add_executable(grabsTest test/main.cpp)
target_link_libraries(grabsTest PUBLIC grabs)
target_link_libraries(grabsTest PUBLIC C:/VulkanSDK/1.3.296.0/Lib/SDL2main.lib)
target_link_libraries(grabsTest PUBLIC C:/VulkanSDK/1.3.296.0/Lib/SDL2.lib)