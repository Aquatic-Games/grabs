cmake_minimum_required(VERSION 3.29)
project(grabs)

set(CMAKE_CXX_STANDARD 20)

add_library(grabs STATIC
        include/grabs/Instance.h
        src/Instance.cpp
        include/grabs/Common.h)

target_include_directories(grabs PUBLIC include/)
