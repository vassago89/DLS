#pragma once

#include <cuda_runtime.h>
#include <device_launch_parameters.h>
#include <cmath>
#include <assert.h>

#ifndef DLS_UNMANAGED_CUDA_HEADER
#define DLS_UNMANAGED_CUDA_HEADER

namespace dlsunmanaged {
	namespace cuda {
		__host__ __forceinline__ void checkCudaError(cudaError_t err)
		{
			assert(err == cudaSuccess);
		}

		void CudaInit();

		void CudaRelease();

		template <class T>
		T* CudaAlloc(int width, int height)
		{
			T* buf = nullptr;
			auto len = width * height * sizeof(T);

			checkCudaError(cudaMalloc(&buf, width * height * sizeof(T)));

			return buf;
		}

		template <class T>
		void CudaFree(T *buf)
		{
			checkCudaError(cudaFree(buf));
		}

		template <class T>
		void CudaCopyToMemory(T* source, T *dest, int length)
		{
 			checkCudaError(cudaMemcpy(dest, source, length, cudaMemcpyDeviceToHost));
		}

		template <class T>
		void CudaCopyToDev(T* source, T *dest, int length)
		{
			checkCudaError(cudaMemcpy(dest, source, length, cudaMemcpyHostToDevice));
		}

		template <class T>
		void CudaCopyDevToDev(T* source, T *dest, int length)
		{
			checkCudaError(cudaMemcpy(dest, source, length, cudaMemcpyDeviceToDevice));
		}

		void Nv2ToRgbFunc(
			unsigned char* srcY,
			unsigned char* srcUV,
			int srcPitch,
			unsigned char* dest,
			int destPitch,
			int width,
			int height);
	}
}

#endif DLS_UNMANAGED_CUDA_HEADER